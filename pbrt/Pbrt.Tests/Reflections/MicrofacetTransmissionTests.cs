using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class MicrofacetTransmissionTests
    {
        private MicrofacetTransmission microfacetTransmission;
        public MicrofacetDistribution distrib = new BeckmannDistribution(0.5f, 1f);
        public Spectrum t  = new Spectrum(1.5f);

        [SetUp]
        public void SetUp()
        {
            microfacetTransmission = new MicrofacetTransmission(t, distrib, 0.5f, 1f, TransportMode.Radiance);
            Check.That(microfacetTransmission.T).IsSameReferenceAs(t);
            var fresnelDielectric = microfacetTransmission.Fresnel as FresnelDielectric;
            Check.That(fresnelDielectric.EtaI).IsEqualTo(0.5f);
            Check.That(fresnelDielectric.EtaT).IsEqualTo(1f);
            Check.That(microfacetTransmission.Distribution).IsSameReferenceAs(distrib);
            Check.That(microfacetTransmission.BxdfType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_GLOSSY);
        }

        [Test]
        public void F_Test()
        {
            // same hemisphere
            var spectrum = microfacetTransmission.F(new Vector3F(-1, -1, 0.5f), new Vector3F(1, 1, 0.5f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            // cos Theta == 0
            spectrum = microfacetTransmission.F(new Vector3F(-1, -1, 0f), new Vector3F(1, 1, 0.5f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            // Same side
            spectrum = microfacetTransmission.F(new Vector3F(1, 0, 0.25f), new Vector3F(1, 0, -0.25f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0));

            spectrum = microfacetTransmission.F(new Vector3F(-1, 1, 0.25f), new Vector3F(1, -1, -0.25f));
            Check.That(spectrum).IsEqualTo(new Spectrum(1.040232E-31f));
        }

        [Test]
        public void Sample_F_Test()
        {
            Vector3F wo = new Vector3F(1, 0, 1);
            var u = new Point2F(0, 0);
            var spectrum = microfacetTransmission.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            Check.That(wi).IsEqualTo(new Vector3F(-0.750409126f, -0.500818193f, -0.660353899f));
            Check.That(pdf).IsCloseTo(0f, 1e-5);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_GLOSSY);
            Check.That(spectrum.C[0]).IsCloseTo(3.5493593E-07f, 1e-5);

        }

        [Test]
        public void Sample_F_Refract_Test()
        {
            Vector3F wo = new Vector3F(0, -1, 1).Normalized();
            var u = new Point2F(0, 0);

            microfacetTransmission = new MicrofacetTransmission(t, distrib, 1f, 0.5f, TransportMode.Radiance);
            var spectrum = microfacetTransmission.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            
            Check.That(wi).IsNull();
            Check.That(pdf).IsCloseTo(0f, 1e-5);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_GLOSSY);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
        }

        [Test]
        public void PdfTest()
        {
            Vector3F wo = new Vector3F(0, -1, 1).Normalized();
            Vector3F wi = new Vector3F(0, -1, -1).Normalized();
            var pdf = microfacetTransmission.Pdf(wo, wi);
            Check.That(pdf).IsCloseTo(0.00347223f, 1e-6f);
        }

        [Test]
        public void Pdf_SameHemisphere_Test()
        {
            Vector3F wo = new Vector3F(0, -1, -1).Normalized();
            Vector3F wi = new Vector3F(0, -1, -2).Normalized();
            var pdf = microfacetTransmission.Pdf(wo, wi);
            Check.That(pdf).IsCloseTo(0f, 1e-6f);
        }
    }
}