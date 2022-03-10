using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class SpecularTransmissionTests
    {
        private readonly Spectrum t = new Spectrum(2f);
        private SpecularTransmission specularTransmission;
        private readonly float etaA = 1f;
        private readonly float etaB = 1.5f;
        private readonly TransportMode transportMode = new TransportMode();

        [SetUp]
        public void SetUp()
        {
            specularTransmission = new SpecularTransmission(t, etaA, etaB, transportMode);
        }
        
        [Test]
        public void FTest()
        {
            var fresnel = specularTransmission.Fresnel as FresnelDielectric; 
            Check.That(fresnel.EtaI).IsEqualTo(etaA);
            Check.That(fresnel.EtaT).IsEqualTo(etaB);
            Check.That(specularTransmission.T).IsEqualTo(t);
            Check.That(specularTransmission.BxdfType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
            Check.That(specularTransmission.F(null, null)).IsEqualTo(new Spectrum(0f));
            Check.That(specularTransmission.Mode).IsEqualTo(transportMode);
        }

        [Test]
        public void Sample_FTest()
        {
            Vector3F wo = new Vector3F(1, 1, 1);
            Point2F sample = Point2F.Zero;
            var spec = specularTransmission.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spec).IsEqualTo(new Spectrum(1.92f));
            Check.That(wi).Check((-2f/3, -2f/3, -1));
            Check.That(pdf).IsEqualTo(1f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
        }

        [Test]
        public void NoTransmission_Sample_FTest()
        {
            Vector3F wo = new Vector3F(1f, 1f, 0f);
            Point2F sample = Point2F.Zero;
            var spec = specularTransmission.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spec).IsEqualTo(new Spectrum(0f));
            Check.That(wi).IsNull();
            Check.That(pdf).IsEqualTo(1f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
        }
    }
}