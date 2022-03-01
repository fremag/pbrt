using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class RgbUtilsTests
    {
        // Not a lot of things to test in this class
        [Test]
        public void RgbReflTest()
        {
            Check.That(RgbUtils.rgbRefl2SpectWhite.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectCyan.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectMagenta.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectYellow.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectRed.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectGreen.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbRefl2SpectBlue.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
        }

        [Test]
        public void RgbIllumTest()
        {
            Check.That(RgbUtils.rgbIllum2SpectWhite.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectCyan.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectMagenta.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectYellow.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectRed.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectGreen.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
            Check.That(RgbUtils.rgbIllum2SpectBlue.NSpectrumSamples).IsEqualTo(SampledSpectrum.NSpectralSamples);
        }

        [Test]
        public void RGBRefl2SpectTest()
        {
            Check.That(RgbUtils.RGB2SpectLambda).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectWhite).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectCyan).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectMagenta).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectYellow).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectRed).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectGreen).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBRefl2SpectBlue).CountIs(RgbUtils.nRGB2SpectSamples);
        }
        [Test]
        public void RGBIllum2SpectTest()
        {
            Check.That(RgbUtils.RGBIllum2SpectWhite ).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectCyan).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectMagenta).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectYellow ).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectRed ).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectGreen).CountIs(RgbUtils.nRGB2SpectSamples);
            Check.That(RgbUtils.RGBIllum2SpectBlue).CountIs(RgbUtils.nRGB2SpectSamples);
        }

        [Test]
        public void FromSampledTest()
        {
            var lambdas = new float[] { 100, 200, 300, 400, 500, 600, 700, 800 };
            var values = new float[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var rgbSpectrum = RgbSpectrum.FromSampled(lambdas, values, lambdas.Length);
            Check.That(rgbSpectrum.C).ContainsExactly(7.6374006f, 5.144754f, 3.9691753f);
        }
    }
}