using System;
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

            public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
            {
                pdf = 1.23f;
                wi = null;
                sampledType = BxDFType.BSDF_DIFFUSE;
                return spectrum;
            }
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
            var spectrum = scaled.Sample_f(null, out _, Point2F.Zero, out var pdf, out var sampledType);
            Check.That(spectrum[0]).IsEqualTo(0.1f * 0.5f);
            Check.That(pdf).IsEqualTo(1.23f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_DIFFUSE);
        }
        
        [Test]
        public void RhoTest()
        {
            Check.ThatCode(() => scaled.Rho(Vector3F.Zero, 0, null)).Throws<NotImplementedException>();
            Check.ThatCode(() => scaled.Rho(0, out _, out _ )).Throws<NotImplementedException>();
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