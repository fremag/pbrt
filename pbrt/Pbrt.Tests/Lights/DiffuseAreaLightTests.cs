using System;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Tests.Lights
{
    [TestFixture]
    public class DiffuseAreaLightTests
    {
        private readonly Transform transform =  Transform.Translate(1, 0, 0);
        private readonly Spectrum lEmit = new Spectrum(1.23f);
        private readonly IShape shape = Substitute.For<IShape>();
        private MediumInterface mediumInterface;
        private DiffuseAreaLight light;

        [SetUp]
        public void SetUp()
        {
            mediumInterface = new MediumInterface(HomogeneousMedium.Default());
            shape.Area.Returns(42f);
            light = new DiffuseAreaLight(transform, mediumInterface, lEmit, 1, shape);
        }
        
        [Test]
        public void PowerTest()
        {
            var spectrum = light.Power();
            Check.That(spectrum.C[0]).IsCloseTo(MathF.PI * 42 * 1.23f, 1e-4);
        }

        [Test]
        public void LTest()
        {
            Interaction interaction = new Interaction {N = new Normal3F(0, 1, 0)};
            var s = light.L(interaction, new Vector3F(0, 1, 0));
            Check.That(s).IsEqualTo(new Spectrum(1.23f));
            s = light.L(interaction, new Vector3F(0, -1, 0));
            Check.That(s).IsEqualTo(new Spectrum(0f));
        }

        [Test]
        public void Pdf_Li_Test()
        {
            shape.Pdf(Arg.Any<Interaction>(), Arg.Any<Vector3F>()).Returns(1.23f);
            var pdfLi = light.Pdf_Li(null, null);
            Check.That(pdfLi).IsEqualTo(1.23f);
        }

        [Test]
        public void SampleLi_Test()
        {
            Interaction interactionShape = new Interaction
            { 
                P = new Point3F(1, 2, 3),
                MediumInterface = new MediumInterface(null),
                N = new Normal3F(0, -1, 0)
            };
            shape.Sample(Arg.Any<Interaction>(), Arg.Any<Point2F>()).Returns(interactionShape);
            shape.Pdf(Arg.Any<Interaction>(), Arg.Any<Vector3F>()).Returns(2.34f);
            
            Interaction interaction = new Interaction
            { 
                P = new Point3F(1, 1, 1),
                MediumInterface = new MediumInterface(null)
            };
            var sampleLi = light.Sample_Li(interaction, new Point2F(0, 0), out var wi, out var pdf, out var visibilityTester);
            Check.That(sampleLi).IsEqualTo(new Spectrum(1.23f));
            Check.That(pdf).IsEqualTo(2.34f);
            Check.That(visibilityTester.P0).IsSameReferenceAs(interaction);
            Check.That(visibilityTester.P1).IsSameReferenceAs(interactionShape);
            Check.That(wi).IsEqualTo(new Vector3F(0,1/MathF.Sqrt(5),2/MathF.Sqrt(5)));
        }
    }
}