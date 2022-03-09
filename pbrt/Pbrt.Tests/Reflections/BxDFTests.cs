using NFluent;
using NUnit.Framework;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class BxDFTests
    {
        [Test]
        [TestCase(0f, 1f, 1f, 1f)]
        [TestCase(-0.707f, 1f, 1f, 0f)]
        [TestCase(0.5f, 0.5f, 1f, 0.1613f)]
        [TestCase(0f, 0.5f, 1f, 1f)]
        public void FrDielectricTest(float cosThetaI, float etaI, float etaT, float expectedValue)
        {
            var f = BxDF.FrDielectric(cosThetaI, etaI, etaT);
            Check.That(f).IsCloseTo(expectedValue, 1e-4);
        }
        
        [Test]
        public void FrConductorTest()
        {
            var cosThetaI = 0.707f;
            var etaI = new Spectrum(new float [] {0.25f, 0.5f});
            var etaT = new Spectrum(new float [] {0.2f, 0.4f});
            var k = new Spectrum(new float [] {0.1f, 0.2f});
            var expected = new Spectrum(new float [] {0.108254164f, 0.108254164f});
            var f = BxDF.FrConductor(cosThetaI, etaI, etaT, k);
            Check.That(f).IsEqualTo(expected);
        }
    }
}