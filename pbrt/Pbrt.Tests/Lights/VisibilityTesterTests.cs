using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace Pbrt.Tests.Lights
{
    [TestFixture]
    public class VisibilityTesterTests
    {
        private Interaction interaction0;
        private Interaction interaction1;
        private VisibilityTester visibilityTester;

        [SetUp]
        public void SetUp()
        {
            interaction0 = new SurfaceInteraction
            {
                P = Point3F.Zero, 
                N = new Normal3F(0, 1, 1),
                PError = Vector3F.Zero,
                MediumInterface =new MediumInterface(HomogeneousMedium.Default())
            };
            
            interaction1 = new SurfaceInteraction
            {
                P = new Point3F(1, 0, 0),
                N = new Normal3F(0, 1, 1),
                PError = Vector3F.Zero,
                MediumInterface = new MediumInterface(HomogeneousMedium.Default())
            };
            
            visibilityTester = new VisibilityTester(interaction0, interaction1);
        }
        
        [Test]
        public void UnoccludedTest()
        {
            IScene scene = Substitute.For<IScene>();
            Ray ray = null;
            scene.IntersectP(Arg.Do<Ray>(r => ray = r)).Returns(true);
            var r = visibilityTester.Unoccluded(scene);
            Check.That(r).IsFalse();
            Check.That(ray.O).IsEqualTo(new Point3F(0,0,0));
            Check.That(ray.D).IsEqualTo(new Vector3F(1,0,0));

            scene.IntersectP(Arg.Do<Ray>(r => ray = r)).Returns(false);
            r = visibilityTester.Unoccluded(scene);
            Check.That(r).IsTrue();
        }

        [Test]
        public void Tr_NoHit_Test()
        {
            IScene scene = Substitute.For<IScene>();
            AbstractSampler sampler = Substitute.For<AbstractSampler>();
            var spectrum = visibilityTester.Tr(scene, sampler);
            Check.That(spectrum).IsEqualTo(new Spectrum(1f));
        }

        [Test]
        public void Tr_Hit_WithMaterial_Test()
        {
            IScene scene = Substitute.For<IScene>();
            Ray ray = null;
            SurfaceInteraction surfaceInteraction = new SurfaceInteraction()
            {
                Primitive = new GeometricPrimitive(null, new MatteMaterial(null, null, null), null, null)
            };
            scene.Intersect(Arg.Do<Ray>(r => ray = r), out _).Returns(info =>
            {
                info[1] = surfaceInteraction;
                return true;
            });
            
            AbstractSampler sampler = Substitute.For<AbstractSampler>();
            var spectrum = visibilityTester.Tr(scene, sampler);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(ray.O).IsEqualTo(new Point3F(0,0,0));
            Check.That(ray.D).IsEqualTo(new Vector3F(1,0,0));
        }

        [Test]
        public void Tr_Hit_NoMaterial_Test()
        {
            IScene scene = Substitute.For<IScene>();
            Ray ray = null;
            SurfaceInteraction surfaceInteraction = new SurfaceInteraction()
            {
                Primitive = new GeometricPrimitive(null, null, null, null),
                P = Point3F.Zero,
                PError = Vector3F.Zero,
                N = new Normal3F(0, 1, 0),
                MediumInterface = new MediumInterface(HomogeneousMedium.Default())
            };
            scene.Intersect(Arg.Do<Ray>(r => ray = r), out _).Returns(info =>
            {
                info[1] = surfaceInteraction;
                return true;
            });
            
            AbstractSampler sampler = Substitute.For<AbstractSampler>();
            var spectrum = visibilityTester.Tr(scene, sampler);
            Check.That(spectrum).IsEqualTo(new Spectrum(1f));
            Check.That(ray.O).IsEqualTo(new Point3F(0,0,0));
            Check.That(ray.D).IsEqualTo(new Vector3F(1,0,0));
        }
    }
}