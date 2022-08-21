using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using pbrt.Accelerators;
using pbrt.Core;
using Pbrt.Demos.Mesh;
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
    public class CloverRenderer : AbstractRenderer
    {
        public override string FileName => "Clover.png";

        public CloverRenderer() : base("Clover", Brushes.White)
        {
            Camera = GetCam((0f, 2, -2f), (0, 2, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new CloverScene();
        }
    }

    public class CloverScene  : Scene
    {
        Texture<float> sigma = new ConstantTexture<float>(30);
        MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
        Texture<Spectrum> kdGreen = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 1, 0 })));
        Texture<Spectrum> kdWhite = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 1, 1 })));
        Texture<Spectrum> kdGray = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0.5f, 0.5f, 0.5f })));

        public CloverScene()
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
            
            primitives.AddRange(BuildClover());
            
            BvhAccel bvh = new BvhAccel(primitives, 5, SplitMethod.Middle);
            var lights = new Light[]
            {
                new PointLight(Transform.Translate( 0, 2, -2), medium, new Spectrum(8f))
            };

            Init(bvh, lights);
        }

        private IEnumerable<IPrimitive> BuildClover()
        {
            const int n = 360;
            const int m = 20;
            const double r1 = 0.5;
            const double r2 = 0.25;

            void CloverPath(double t, out double x, out double y, out double z)
            {
                var alpha = 2 * MathF.PI * t;
                x = r1 * (Math.Cos(alpha) + 2 * Math.Cos(3 * alpha));
                y = r1 * (Math.Sin(alpha) - 2 * Math.Sin(3 * alpha));
                z = 2 * r2 * Math.Sin(4 * alpha);
            }

            void CircleCurve(double u, double v, out double x, out double y)
            {
                x = r2 * Math.Cos(2 * MathF.PI* v);
                y = r2 * Math.Sin(2 * MathF.PI* v);
            }

            CurveSweepMesh cloverMesh = new CurveSweepMesh(n, m, CloverPath, CircleCurve);
            Point3F[] points = new Point3F[n*m];

            var vertexIndices = new List<int>();
            int Idx(int i, int j) => ((i % n)*m)+ (j%m); 
            for (int i = 0; i < n; i++)
            {
                var cloverMeshPoint = cloverMesh.Points[i];

                for (int j = 0; j < m; j++)
                {
                    var index = i * m + j;
                    points[index] = cloverMeshPoint[j];
                    
                    var idx = Idx(i, j);
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
            var materialGreen = new MatteMaterial(kdGreen, sigma, null);
            var transform = Transform.Translate(0, 2, 0);
            var mesh = new TriangleMesh(transform, nbTriangles, vertexIndices.ToArray(), vertexIndices.Count, points, null, null, null, null, null, null);
            
            var triangles =  mesh
                .GetTriangles()
                .Select(shape => new GeometricPrimitive(shape, materialGreen, null, medium))
                .ToArray();
            return triangles;
        }
    }
}