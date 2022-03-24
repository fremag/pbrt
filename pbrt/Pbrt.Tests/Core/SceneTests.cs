using System;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Samplers;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class SceneTests
    {
        private readonly Light light = Substitute.For<Light>();
        private Scene scene;
        private readonly ConstantTexture<Spectrum> kd= new ConstantTexture<Spectrum>(new Spectrum(0f));
        private readonly Texture<float> sigma = new ConstantTexture<float>(0f);
        private MatteMaterial material;
        private IPrimitive aggregate;
        private IShape shape;

        [SetUp]
        public void SetUp()
        {
            material = new MatteMaterial(kd, sigma, null);
            shape = Create();
            aggregate = new GeometricPrimitive(shape, material,  null, new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default()));
            scene = new Scene(aggregate, new [] { light });
        }
        
        [Test]
        public void BasicTest()
        {
            Check.That(scene.WorldBound.PMin).IsEqualTo(new Point3F(-1, -1, -1));
            Check.That(scene.WorldBound.PMax).IsEqualTo(new Point3F(1, 1, 1));
            light.Received(1).Preprocess(scene);
        }

        [Test]
        public void IntersectPTest()
        {
            Ray ray = new Ray(Point3F.Zero, new Vector3F(1, 0, 0), 1000)
            {
                Medium = HomogeneousMedium.Default()
            };
            var result = scene.IntersectP(ray);
            Check.That(result).IsTrue();
            
            ray.O = new Point3F(0, 5, 0);
            result = scene.IntersectP(ray);
            Check.That(result).IsFalse();
        }
        
        [Test]
        public void IntersectTrTest()
        {
            Ray ray = new Ray(Point3F.Zero, new Vector3F(1, 0, 0), 1000)
            {
                Medium = HomogeneousMedium.Default()
            };
            AbstractSampler sampler = new PixelSampler(1, 1);
            var result = scene.IntersectTr(ray, sampler, out var surfaceInteraction, out var spectrum);
            Check.That(result).IsTrue();
            Check.That(surfaceInteraction.P).IsEqualTo(new Point3F(1, 0, 0));
            Check.That(spectrum).IsEqualTo(new Spectrum(1f));
        }

        [Test]
        public void IntersectTr_NoHit_Test()
        {
            Ray ray = new Ray(new Point3F(0, 5, 0), new Vector3F(1, 0, 0), 1000)
            {
                Medium = HomogeneousMedium.Default()
            };
            AbstractSampler sampler = new PixelSampler(1, 1);
            var result = scene.IntersectTr(ray, sampler, out var surfaceInteraction, out var spectrum);
            Check.That(result).IsFalse();
            Check.That(surfaceInteraction).IsNull();
            Check.That(spectrum).IsEqualTo(new Spectrum(1f));
        }

        [Test]
        public void IntersectTr_HitNoMaterial_Test()
        {
            aggregate = new GeometricPrimitive(shape, null, null, new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default()));
            scene = new Scene(aggregate, new [] { light });

            Ray ray = new Ray(new Point3F(-5, 0, 0), new Vector3F(1, 0, 0), 1000)
            {
                Medium = HomogeneousMedium.Default()
            };

            AbstractSampler sampler = new PixelSampler(1, 1);
            var result = scene.IntersectTr(ray, sampler, out var surfaceInteraction, out var spectrum);
            Check.That(result).IsFalse();
            Check.That(surfaceInteraction).IsNull();
            Check.That(spectrum).IsEqualTo(new Spectrum(1f));
        }

        public static IShape Create()
        {
            var transform = Transform.Translate(0, 0, 0);
            return new Sphere(transform, transform.Inverse(), false, 1, -1, 1, MathF.PI * 2) ;
        }
    }
}