using System.Collections.Generic;
using System.Drawing;
using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Integrators;
using pbrt.Media;
using pbrt.Samplers;
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

            public DummySamplerIntegrator(AbstractSampler sampler, AbstractCamera camera, int nbThreads = 1) : base(sampler, camera, nbThreads)
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
            samplerIntegrator = new DummySamplerIntegrator(sampler, camera);
        }

        [Test]
        public void BasicTest()
        {
            Check.That(samplerIntegrator.NbThreads).IsEqualTo(1);
            IScene scene = new Scene();
            var img = samplerIntegrator.Render(scene);
            Check.That(samplerIntegrator.rays).CountIs(144);
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
    }
}