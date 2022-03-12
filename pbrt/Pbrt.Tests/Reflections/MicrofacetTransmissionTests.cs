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
    }
}