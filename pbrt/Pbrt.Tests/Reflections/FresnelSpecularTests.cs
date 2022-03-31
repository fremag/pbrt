using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class FresnelSpecularTests
    {
        private FresnelSpecular fresnelSpecular;
        private readonly Spectrum r = new Spectrum(2f);
        private readonly Spectrum t = new Spectrum(3f);
        private readonly float etaA = 1f;
        private readonly float etaB = 1.5f;
        private readonly TransportMode transportMode = new TransportMode();

        [SetUp]
        public void SetUp()
        {
            fresnelSpecular = new FresnelSpecular(r, t, etaA, etaB, transportMode);
        }
        
        [Test]
        public void FTest()
        {
            var fresnel = fresnelSpecular.Fresnel as FresnelDielectric;
            Check.That(fresnel.EtaI).IsEqualTo(fresnel.EtaI);
            Check.That(fresnel.EtaT).IsEqualTo(fresnel.EtaT);
            Check.That(fresnelSpecular.R).IsEqualTo(r);
            Check.That(fresnelSpecular.T).IsEqualTo(t);
            Check.That(fresnelSpecular.BxdfType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
            Check.That(fresnelSpecular.Mode).IsEqualTo(transportMode);
            Check.That(fresnelSpecular.F(null, null)).IsEqualTo(new Spectrum(0f));
        }

        [Test]
        public void Sample_F_Reflection_Test()
        {
            Vector3F wo = new Vector3F(1, 1, 1);
            Point2F sample = Point2F.Zero;
            var spectrum = fresnelSpecular.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(0.0800000057f));
            Check.That(wi).IsEqualTo(new Vector3F(-1, -1, 1));
            Check.That(pdf).IsEqualTo(0.0400000028f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR);
        }
        
        [Test]
        public void Sample_F_Transmission_Test()
        {
            Vector3F wo = new Vector3F(1, 1, 1);
            Point2F sample = new Point2F(0.5f, 0f);
            var spectrum = fresnelSpecular.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(1.28f));
            Check.That(wi).IsEqualTo(new Vector3F(-2f/3, -2f/3, -1));
            Check.That(pdf).IsEqualTo(0.959999979f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
        }

        [Test]
        public void Sample_F_SpecularTransmission_Test()
        {
            fresnelSpecular = new FresnelSpecular(r, t, 0.7f, 0.5f, transportMode);
            Vector3F wo = new Vector3F(0.2f, 0.2f, 0.4f);
            Point2F sample = new Point2F(1f, 0.2f);
            var spectrum = fresnelSpecular.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(wi).IsNull();
            Check.That(pdf).IsEqualTo(0f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_NONE);
        }
    }
}