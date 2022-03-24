using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Distribution1DTests
    {
        [Test]
        [TestCase(0, 0, 0.5f, 0)]
        [TestCase(0.1f, 0.2f, 0.5f, 0)]
        [TestCase(0.2f, 0.3666f, 1f, 1)]
        [TestCase(0.5f, 2f/3, 1.5f, 2)]
        [TestCase(0.6f, 0.7333f, 1.5f, 2)]
        public void SampleContinuousTest(float u, float expResult, float expPdf, int expOffset)
        {
            Distribution1D distrib = new Distribution1D(new[] {1f, 2f, 3f});
            var result = distrib.SampleContinuous(u, out var pdf, out int offset);
            Check.That(result).IsCloseTo(expResult, 1e-4);
            Check.That(pdf).IsCloseTo(expPdf, 1e-4);
            Check.That(offset).IsEqualTo(expOffset);
        }
        
        [Test]
        [TestCase(0, 0, 0)]
        [TestCase(0.1f, 0.1f,  0)]
        [TestCase(0.2f, 0.2f, 0)]
        [TestCase(0.5f, 0.5f,  1)]
        [TestCase(0.6f, 0.6f,  1)]
        [TestCase(0.67f, 0.67f,  2)]
        [TestCase(1f, 1f,  2)]
        public void SampleContinuous_FlatDistribution_Test(float u, float expResult, int expOffset)
        {
            Distribution1D distrib = new Distribution1D(new[] {0f, 0f, 0f});
            var result = distrib.SampleContinuous(u, out _, out int offset);
            Check.That(result).IsCloseTo(expResult, 1e-4);
            Check.That(offset).IsEqualTo(expOffset);
        }
        
        [Test]
        [TestCase(0, 0, 1f/6, 0)]
        [TestCase(0.1f, 0, 1f/6, 0.6f)]
        [TestCase(0.2f, 1, 1f/3, 0.1f)]
        [TestCase(0.5f, 2, 0.5f, 0)]
        [TestCase(0.6f, 2, 0.5f, 0.2f)]
        [TestCase(0.8f, 2, 0.5f, 0.6f)]
        public void SampleDiscreteTest(float u, int expResult, float expPdf, float expURemapped)
        {
            Distribution1D distrib = new Distribution1D(new[] {1f, 2f, 3f});
            var result = distrib.SampleDiscrete(u, out var pdf, out float uRemapped);
            Check.That(result).IsEqualTo(expResult);
            Check.That(pdf).IsCloseTo(expPdf, 1e-4);
            Check.That(uRemapped).IsCloseTo(expURemapped, 1e-4);
        }

        [Test]
        public void DiscretePDFTest()
        {
            Distribution1D distrib = new Distribution1D(new[] {1f, 2f, 3f});
            Check.That(distrib.DiscretePDF(0)).IsEqualTo(1f/6);
            Check.That(distrib.DiscretePDF(1)).IsEqualTo(2f/6);
            Check.That(distrib.DiscretePDF(2)).IsEqualTo(3f/6);
        }
    }
}