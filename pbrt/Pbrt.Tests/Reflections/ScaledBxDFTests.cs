using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class ScaledBxDFTests
    {
        private DummyBxDF bxdf;
        private Spectrum scale;
        private ScaledBxDF scaled;

        private class DummyBxDF : BxDF
        {
            private Spectrum spectrum;

            public DummyBxDF() : base(BxDFType.BSDF_SPECULAR)
            {
                spectrum = new Spectrum(SampledSpectrum.NSpectralSamples, 0.5f);
            }

            public override Spectrum F(Vector3F wo, Vector3F wi) => spectrum;

            public override Spectrum Sample_f(Vector3F wo, Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
            {
                pdf = 1.23f;
                sampledType = BxDFType.BSDF_DIFFUSE;
                return spectrum;
            }

            public override Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples) => spectrum;
        }

        [SetUp]
        public void SetUp()
        {
            bxdf = new DummyBxDF();
            scale = new Spectrum(SampledSpectrum.NSpectralSamples, 0.1f);
            scaled = new ScaledBxDF(bxdf, scale);
        }
        
        [Test]
        public void FTest()
        {
            var spectrum = scaled.F(null, null);
            Check.That(spectrum[0]).IsEqualTo(0.1f * 0.5f);
        }
        
        [Test]
        public void Sample_fTest()
        {
            var spectrum = scaled.Sample_f(null, null, Point2F.Zero, out var pdf, out var sampledType);
            Check.That(spectrum[0]).IsEqualTo(0.1f * 0.5f);
            Check.That(pdf).IsEqualTo(1.23f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_DIFFUSE);
        }
        
        [Test]
        public void RhoTest()
        {
            var spectrum = scaled.Rho(null, 1, null);
            Check.That(spectrum[0]).IsEqualTo(0.1f * 0.5f);
        }

        [Test]
        public void MatchesFlagsTest()
        {
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_ALL)).IsFalse();
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_SPECULAR)).IsTrue();
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_DIFFUSE)).IsFalse();
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_GLOSSY)).IsFalse();
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_REFLECTION)).IsFalse();
            Check.That(scaled.MatchesFlags(BxDFType.BSDF_TRANSMISSION)).IsFalse();
        }
    }
}