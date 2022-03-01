using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class SpectrumUtilsTests
    {
        [Test]
        public void RgbToXyzTest()
        {
            var rgb = new float[] { 1, 1, 1};
            var xyz = SpectrumUtils.RGBToXYZ(rgb);
            Check.That(xyz[0]).IsCloseTo(0.950456023f, 1e-5);
            Check.That(xyz[1]).IsCloseTo(1f, 1e-5);
            Check.That(xyz[2]).IsCloseTo(1.08875406f, 1e-5);
        }
        
        [Test]
        public void XyzToRgbTest()
        {
            var xyz = new float[]
            {
                0.950456023f, 1f, 1.088754061f
            };
            var rgb = SpectrumUtils.XYZToRGB(xyz);
            Check.That(rgb[0]).IsCloseTo(1f, 1e-5);
            Check.That(rgb[1]).IsCloseTo(1f, 1e-5);
            Check.That(rgb[2]).IsCloseTo(1f, 1e-5);
        }

        [Test]
        [TestCase(-1f, 0)]
        [TestCase(10f, 6)]
        [TestCase(3f, 2)]
        [TestCase(6.5f, 5)]
        public void FindIntervalTest(float value, int expectedIndex)
        {
            var values = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var index = SpectrumUtils.FindInterval(values.Length, i => values[i] <= value);
            Check.That(index).IsEqualTo(expectedIndex);
        }

        [Test]
        [TestCase(0f, 1f)]
        [TestCase(100f, 1f)]
        [TestCase(150f, 1.5f)]
        [TestCase(650f, 6.5f)]
        [TestCase(999f, 8f)]
        public void InterpolateSpectrumSamplesTest(float lambda, float expectedValue)
        {
            var lambdas = new float[] { 100, 200, 300, 400, 500, 600, 700, 800 };
            var values = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            float value = SpectrumUtils.InterpolateSpectrumSamples(lambdas, values, lambdas.Length, lambda);
            Check.That(value).IsCloseTo(expectedValue, 1e-3);
        }
    }
}