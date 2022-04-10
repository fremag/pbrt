using System.Collections.Generic;
using System.Drawing;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Integrators;
using pbrt.Media;
using pbrt.Reflections;
using pbrt.Samplers;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Tests.Integrators
{
    [TestFixture]
    public class SamplerIntegratorTests
    {
        private DummySamplerIntegrator samplerIntegrator;

        private class DummySamplerIntegrator : SamplerIntegrator
        {
            internal List<RayDifferential> rays = new List<RayDifferential>();
            internal float spectrumValue = 1.23f;

            public DummySamplerIntegrator(AbstractSampler sampler, AbstractCamera camera, int nbThreads = 1, int tileSize=16) : base(sampler, camera, nbThreads, tileSize)
            {
            }

            public override Spectrum Li(RayDifferential ray, IScene scene, AbstractSampler sampler, int depth = 0)
            {
                rays.Add(ray);
                return new Spectrum(spectrumValue);
            }
        }
        
        [SetUp]
        public void SetUp()
        {
            AbstractSampler sampler = new PixelSampler(1, 1);
            Film film = new Film(10, 10);
            var ratio = (float)film.FullResolution.X / film.FullResolution.Y;

            var cameraToWorld = Transform.LookAt(new Point3F(-5, 0, 0), Point3F.Zero, new Vector3F(0, 1, 0)).Inverse();
            var screenWindow = new Bounds2F(new Point2F(-ratio, -1), new Point2F(ratio, 1));
            Medium medium = HomogeneousMedium.Default();
            
            AbstractCamera camera = new OrthographicCamera(cameraToWorld, screenWindow, film, medium);
            samplerIntegrator = new DummySamplerIntegrator(sampler, camera, 1, 4);
        }

        [Test]
        public void BasicTest()
        {
            Check.That(samplerIntegrator.NbThreads).IsEqualTo(1);
            IScene scene = new Scene();
            int n = 0;
            samplerIntegrator.TileRendered += (i, j, t) => n++;
            var img = samplerIntegrator.Render(scene);
            Check.That(samplerIntegrator.rays).CountIs(144);
            Check.That(n).IsEqualTo(9);
            
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Check.That(img.GetPixel(i, j)).IsEqualTo(Color.FromArgb(255, 255, 255, 255));
                }
            }
        }
        
        [Test]
        [TestCase(float.PositiveInfinity)]
        [TestCase(float.NaN)]
        [TestCase(-1f)]
        public void ErrorsTest(float value)
        {
            samplerIntegrator.spectrumValue = value;
            IScene scene = new Scene();
            var img = samplerIntegrator.Render(scene);
            Check.That(samplerIntegrator.rays).CountIs(144);
             
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    Check.That(img.GetPixel(i, j)).IsEqualTo(Color.FromArgb(255, 0, 0, 0));
                }
            }
        }

        [Test]
        public void SpecularReflectTest()
        {
            RayDifferential rayDiff = new RayDifferential(Point3F.Zero, new Vector3F(1, 0, 0));
            rayDiff.HasDifferentials = true;
            rayDiff.RxOrigin = Point3F.Zero;
            rayDiff.RyOrigin = Point3F.Zero;
            rayDiff.RxDirection = new Vector3F(1, 0, 0);
            rayDiff.RyDirection = new Vector3F(1, 0, 0);
            
            IScene scene = Substitute.For<IScene>();
            AbstractSampler sampler = Substitute.For<AbstractSampler>();
            sampler.Get2D().Returns(Point2F.Zero);
            AbstractShape shape = Substitute.For<AbstractShape>(Transform.Translate(0,0,0), Transform.Translate(0,0,0), false);

            var wo = new Vector3F(1, 0, 0);
            Vector3F dpdu = new Vector3F(0, 0, 1);
            Vector3F dpdv = new Vector3F(0, 1, 0);
            var dndu = new Normal3F(0, 0, 1);
            var dndv = new Normal3F(0, 1, 0);
            var isect = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, wo, dpdu, dpdv, dndu, dndv, 1, shape);
            isect.MediumInterface = new MediumInterface(HomogeneousMedium.Default());
            isect.DpDx = Vector3F.Zero;
            isect.DpDy = Vector3F.Zero;
            
            BSDF bsdf = new BSDF(isect);
            bsdf.Add(new SpecularReflection(new Spectrum(1f), new FresnelNoOp()));
            isect.Bsdf = bsdf;
            var f = samplerIntegrator.SpecularReflect(rayDiff, isect, scene, sampler, 1);
            Check.That(f).IsEqualTo(new Spectrum(1.23f));

            isect.Wo = Vector3F.Zero;
            f = samplerIntegrator.SpecularReflect(rayDiff, isect, scene, sampler, 1);
            Check.That(f).IsEqualTo(new Spectrum(0f));
        }
        
        [Test]
        public void SpecularTransmitTest()
        {
            RayDifferential rayDiff = new RayDifferential(Point3F.Zero, new Vector3F(1, 0, 0));
            rayDiff.HasDifferentials = true;
            rayDiff.RxOrigin = Point3F.Zero;
            rayDiff.RyOrigin = Point3F.Zero;
            rayDiff.RxDirection = new Vector3F(1, 0, 0);
            rayDiff.RyDirection = new Vector3F(1, 0, 0);
            
            IScene scene = Substitute.For<IScene>();
            AbstractSampler sampler = Substitute.For<AbstractSampler>();
            sampler.Get2D().Returns(Point2F.Zero);
            AbstractShape shape = Substitute.For<AbstractShape>(Transform.Translate(0,0,0), Transform.Translate(0,0,0), false);

            var wo = new Vector3F(1, 0, 0);
            Vector3F dpdu = new Vector3F(0, 0, 1);
            Vector3F dpdv = new Vector3F(0, 1, 0);
            var dndu = new Normal3F(0, 0, 1);
            var dndv = new Normal3F(0, 1, 0);
            var isect = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, wo, dpdu, dpdv, dndu, dndv, 1, shape);
            isect.MediumInterface = new MediumInterface(HomogeneousMedium.Default());
            isect.DpDx = Vector3F.Zero;
            isect.DpDy = Vector3F.Zero;
            
            BSDF bsdf = new BSDF(isect);
            bsdf.Add(new SpecularTransmission(new Spectrum(1f), 1.5f, 1, TransportMode.Radiance));
            isect.Bsdf = bsdf;
            var f = samplerIntegrator.SpecularTransmit(rayDiff, isect, scene, sampler, 1);
            Check.That(f).IsEqualTo(new Spectrum(1.1808f));
        }
    }
}