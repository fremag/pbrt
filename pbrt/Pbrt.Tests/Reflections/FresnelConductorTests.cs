using NFluent;
using NUnit.Framework;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class FresnelConductorTests
    {
        [Test]
        public void EvaluateTest()
        {
            var cosThetaI = 0.707f;
            var etaI = new Spectrum(new float [] {0.25f, 0.5f});
            var etaT = new Spectrum(new float [] {0.2f, 0.4f});
            var k = new Spectrum(new float [] {0.1f, 0.2f});
            var expected = new Spectrum(new float [] {0.108254164f, 0.108254164f});
            var fresnelConductor = new FresnelConductor(etaI, etaT, k);
            var f = fresnelConductor.Evaluate(cosThetaI);
            Check.That(f).IsEqualTo(expected);
        }
    }
}