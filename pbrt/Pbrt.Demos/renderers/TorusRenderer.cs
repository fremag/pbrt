using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using pbrt.Accelerators;
using pbrt.Core;
using pbrt.Integrators;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Samplers;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.renderers
{
    public class TorusRenderer : AbstractRenderer
    {
        public override string FileName => "Torus.png";

        public TorusRenderer() : base("Torus", Brushes.White)
        {
            Camera = GetCam((4f, 4, -4f), (0, 0, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new TorusScene();
        }
    }

    public class TorusScene  : Scene
    {
        Texture<float> sigma = new ConstantTexture<float>(30);
        MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
        Texture<Spectrum> kdGreen = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 1, 0 })));
        Texture<Spectrum> kdWhite = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 1, 1 })));
        Texture<Spectrum> kdGray = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0.5f, 0.5f, 0.5f })));

        public TorusScene()
        {
            TextureMapping2D mapping = new PlanarMapping2D(new Vector3F(1, 0, 0), new Vector3F(0, 0, 1), 1, 1);
            var kdChecker = new Checkerboard2DTexture<Spectrum>(mapping, kdGray, kdWhite);
            
            MediumInterface mediumInterface = new MediumInterface(HomogeneousMedium.Default());
            var planeTransform = Transform.Scale(100f, 0.1f, 100f);
            var plane = new Sphere(planeTransform, 1f);
            var planePrimitive = new GeometricPrimitive(plane, new MatteMaterial(kdChecker, sigma, null), null, mediumInterface);

            var primitives = new List<IPrimitive>
            {
                planePrimitive 
            };
            primitives.AddRange(BuildTorus());
            
            BvhAccel bvh = new BvhAccel(primitives, 5, SplitMethod.Middle);
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(00, 15, -10), medium, new Spectrum(500f))
            };

            Init(bvh, lights);
        }

        private IEnumerable<IPrimitive> BuildTorus()
        {
            IMaterial materialGreen = new MatteMaterial(kdGreen, sigma, null);
            Transform transform = Transform.Translate(0, 1, 0) * Transform.RotateX(90) * Transform.RotateZ(45);
            int n = 30;
            int m = 30;
            float r1 = 2;
            float r2 = 1;
 
            var vertexIndices = new List<int>();
            var points = new List<Point3F>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var theta = 2 * (MathF.PI / n) * i;
                    var phi = 2 * (MathF.PI / m) * j;
                    var x = (r1 + r2 * MathF.Cos(theta))*MathF.Cos(phi);
                    var y =  (r1 + r2 * MathF.Cos(theta))*MathF.Sin(phi);
                    var z =  r2 * MathF.Sin(theta);
                    
                    points.Add(new Point3F(x, y, z));
                }
            }

            int Idx(int i, int j) => ((i % n)*n)+ (j%m); 
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var idx = Idx(i,j);
                    var idx1 = Idx(i+1, j);
                    var idx2 = Idx(i, j+1);

                    vertexIndices.Add(idx);
                    vertexIndices.Add(idx1);
                    vertexIndices.Add(idx2);

                    idx = Idx(i+1,j+1);
                    idx1 = Idx(i+1, j);
                    idx2 = Idx(i, j+1);

                    vertexIndices.Add(idx);
                    vertexIndices.Add(idx1);
                    vertexIndices.Add(idx2);
                    
                }
            }

            int nbTriangles = vertexIndices.Count / 3;
            var mesh = new TriangleMesh(transform, nbTriangles, vertexIndices.ToArray(), vertexIndices.Count, points.ToArray(), null, null, null, null, null, null);
            
            var triangles =  mesh
                .GetTriangles()
                .Select(shape => new GeometricPrimitive(shape, materialGreen, null, medium))
                .ToArray();
            return triangles;
        }
    }
}