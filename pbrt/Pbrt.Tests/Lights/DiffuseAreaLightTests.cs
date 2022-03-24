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
        Transform transform =  Transform.Translate(1, 0, 0);
        Spectrum lemit = new Spectrum(1.23f);
        IShape shape = Substitute.For<IShape>();
        private MediumInterface mediumInterface;
        private DiffuseAreaLight light;

        [SetUp]
        public void SetUp()
        {
            mediumInterface = new MediumInterface(HomogeneousMedium.Default());
            shape.Area.Returns(42f);
            light = new DiffuseAreaLight(transform, mediumInterface, lemit, 1, shape);

            Check.ThatCode(() => light.Sample_Li(null, null, out _, out _, out _)).Throws<NotImplementedException>();
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
    }
}