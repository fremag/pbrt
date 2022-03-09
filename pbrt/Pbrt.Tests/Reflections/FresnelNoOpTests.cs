using NFluent;
using NUnit.Framework;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class FresnelNoOpTests
    {
        [Test]
        [TestCase(0f, 1f, 1f, 1f)]
        [TestCase(-0.707f, 1f, 1f, 1f)]
        [TestCase(0.5f, 0.5f, 1f, 1f)]
        [TestCase(0f, 0.5f, 1f, 1f)]
        public void EvaluateTest(float cosThetaI, float etaI, float etaT, float expectedValue)
        {
            var fresnelNoOp = new FresnelNoOp();
            var spectrum = fresnelNoOp.Evaluate(cosThetaI);
            foreach (var c in spectrum.C)
            {
                Check.That(c).IsCloseTo(expectedValue, 1e-4);
            }
        }
    }
}