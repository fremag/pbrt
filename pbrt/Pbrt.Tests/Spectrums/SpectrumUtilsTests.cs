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
    }
}