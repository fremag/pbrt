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
                wi = new Vector3F(1,1,1);
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
        public void Rho_W_Test()
        {
            Point2F[] u1 = new [] {new Point2F(0.25f, 0), new Point2F(0, 0.25f)};
            Point2F[] u2 = new [] {new Point2F(-0.25f, -0.25f), new Point2F(0, 0)};
            Spectrum spectrum =  scaled.Rho(2, u1, u2 );
            Check.That(spectrum).IsEqualTo(new Spectrum(0.0101626012f));
        }

        [Test]
        public void Rho_Test()
        {
            Point2F[] u = new [] {new Point2F(0.25f, 0), new Point2F(0, 0.25f)};
            Vector3F w = new Vector3F(1, 0, 1);
            Spectrum spectrum =  scaled.Rho(w, 2, u);
            Check.That(spectrum).IsEqualTo(new Spectrum(0.0406504087f));
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