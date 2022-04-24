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
    public class TriangleRenderer : AbstractRenderer
    {
        public override string FileName => "Triangle.png";

        public TriangleRenderer() : base("Triangle", Brushes.White)
        {
            Camera = GetCam((0f, 3, -3f), (0, 0, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new TriangleScene();
        }
    }

    public class TriangleScene  : Scene
    {
        Texture<float> sigma = new ConstantTexture<float>(30);
        MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
        Texture<Spectrum> kdGreen = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 1, 0 })));
        Texture<Spectrum> kdWhite = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 1, 1 })));
        Texture<Spectrum> kdGray = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0.5f, 0.5f, 0.5f })));

        public TriangleScene()
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
            primitives.AddRange(BuildTriangles());
            
            BvhAccel bvh = new BvhAccel(primitives, 5, SplitMethod.Middle);
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(5, 15, -10), medium, new Spectrum(500f))
            };

            Init(bvh, lights);
        }

        private IEnumerable<IPrimitive> BuildTriangles()
        {
            IMaterial materialGreen = new MatteMaterial(kdGreen, sigma, null);
            Transform transform = Transform.Translate(0, 1, 0);
 
            var vertexIndices = new List<int>();
            var points = new List<Point3F>();

            vertexIndices.AddRange(new int[]{0, 1, 2});
            points.Add((-1,0,1));
            points.Add((0,1,1));
            points.Add((1,0,1));

            vertexIndices.AddRange(new int[]{3, 4, 5});
            points.Add((-3,0,0));
            points.Add((-1,1,0));
            points.Add((-2,0,0));

            vertexIndices.AddRange(new int[]{6, 7, 8});
            points.Add((1,1,-2f));
            points.Add((0,0,-2f));
            points.Add((-1,1,-2f));

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