using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Tests.Lights
{
    [TestFixture]
    public class PointLightTests
    {
        private readonly Transform transform = Transform.Translate(0, 1, 0);
        private readonly Spectrum spectrum = new Spectrum(1f);
        private MediumInterface mediumInterface;
        private PointLight light;

        [SetUp]
        public void SetUp()
        {
            mediumInterface = new MediumInterface(HomogeneousMedium.Default());
            light = new PointLight(transform, mediumInterface, spectrum);
            light.Preprocess(null);
            Check.That(light.PLight).IsEqualTo(new Point3F(0, 1, 0));
        } 
        
        [Test]
        public void PowerTest()
        {
            var power = light.Power();
            Check.That(power).IsEqualTo(new Spectrum(4*MathF.PI));
        }

        [Test]
        public void Sample_LiTest()
        {
            var translate = Transform.Translate(0, 0, 0);
            AbstractShape sphere = new Sphere(translate, translate.Inverse(), false, 1, -1, 1);
            Interaction interaction = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, new Vector3F(-1, 0, 0), Vector3F.Zero, Vector3F.Zero, new Normal3F(-1, 1, 0), new Normal3F(0, 0, 0), 1000, sphere);
            Point2F u = Point2F.Zero;
            var li = light.Sample_Li(interaction, u, out var wi, out var pdf, out var visibilityTester);
            Check.That(wi).IsEqualTo(new Vector3F(0, 1, 0));
            Check.That(visibilityTester.P0).IsSameReferenceAs(interaction);
            Check.That(visibilityTester.P1.N).IsNull();
            Check.That(visibilityTester.P1.P).IsEqualTo(new Point3F(0, 1, 0));
            Check.That(visibilityTester.P1.MediumInterface).IsSameReferenceAs(mediumInterface);
            
            Check.That(pdf).IsEqualTo(1f);
            Check.That(li).IsEqualTo(new  Spectrum(1f));
        }

        [Test]
        public void LeTest()
        {
            Check.That(light.Le(null)).IsEqualTo(new Spectrum(0f));
        }
 
        [Test]
        public void Pdf_Li_Test()
        {
            Check.That(light.Pdf_Li(null, null)).IsEqualTo(0f);
        }
    }
}