using System;
using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class RgbSpectrumTests
    {
        [Test]
        public void EmptyConstructorTest()
        {
            RgbSpectrum rgbSpectrum = new RgbSpectrum();
            Check.That(rgbSpectrum.C).ContainsExactly(0f, 0f, 0f);
        }

        [Test]
        public void RgbConstructorTest()
        {
            float[] rgb = new []{1.23f, 2.34f, 3.45f};
            RgbSpectrum rgbSpectrum = new RgbSpectrum(rgb);
            Check.That(rgbSpectrum.C).ContainsExactly(1.23f, 2.34f, 3.45f);
        }
        
        [Test]
        public void CoefficientSpectrumConstructorTest()
        {
            CoefficientSpectrum coefficientSpectrum = new CoefficientSpectrum( new []{1.23f, 2.34f, 3.45f});
            RgbSpectrum rgbSpectrum = new RgbSpectrum(coefficientSpectrum);
            Check.That(rgbSpectrum.C).ContainsExactly(1.23f, 2.34f, 3.45f);

            coefficientSpectrum = new CoefficientSpectrum( new []{1.23f, 2.34f, 3.45f, 4.56f});
            Check.ThatCode( () => new RgbSpectrum(coefficientSpectrum)).Throws<ArgumentException>().WithMessage("4 != 3 (Parameter 'NSpectrumSamples')");
        }
        
        [Test]
        public void ToRgbTest()
        {
            float[] rgb = new []{1.23f, 2.34f, 3.45f};
            RgbSpectrum rgbSpectrum = new RgbSpectrum(rgb);
            Check.That(rgbSpectrum.ToRgb()).ContainsExactly(1.23f, 2.34f, 3.45f);
        }
        
        [Test]
        public void FromRgbTest()
        {
            float[] rgb = new []{1.23f, 2.34f, 3.45f};
            RgbSpectrum rgbSpectrum = RgbSpectrum.FromRGB(rgb);
            Check.That(rgbSpectrum.C).ContainsExactly(1.23f, 2.34f, 3.45f);
        }
        
        [Test]
        public void ToXyzTest()
        {
            float[] rgb = new []{1.23f, 2.34f, 3.45f};
            RgbSpectrum rgbSpectrum = RgbSpectrum.FromRGB(rgb);
            Check.That(rgbSpectrum.ToXyz()).ContainsExactly(1.9665136f, 2.1840427f, 3.5809758f);
        }
        
        [Test]
        public void FromXyzTest()
        {
            var xyz = new[] { 1.9665136f, 2.1840427f, 3.5809758f };
            var rgbSpectrum = RgbSpectrum.FromXYZ(xyz);
            Check.That(rgbSpectrum.C[0]).IsCloseTo(1.23f, 1e-3);
            Check.That(rgbSpectrum.C[1]).IsCloseTo(2.34f, 1e-3);
            Check.That(rgbSpectrum.C[2]).IsCloseTo(3.45f, 1e-3);
        }
    }
}