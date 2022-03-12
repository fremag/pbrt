using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class MicrofacetReflectionTests
    {
        private MicrofacetReflection microfacetReflection;
        public Fresnel fresnel = new FresnelNoOp();
        public MicrofacetDistribution distrib = new BeckmannDistribution(0.5f, 1f);
        public Spectrum r  = new Spectrum(1.5f);

        [SetUp]
        public void SetUp()
        {
            microfacetReflection = new MicrofacetReflection(r, distrib, fresnel);
            Check.That(microfacetReflection.R).IsSameReferenceAs(r);
            Check.That(microfacetReflection.Fresnel).IsSameReferenceAs(fresnel);
            Check.That(microfacetReflection.Distribution).IsSameReferenceAs(distrib);
            Check.That(microfacetReflection.BxdfType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY);
        }

        [Test]
        public void F_Test()
        {
            var spectrum = microfacetReflection.F(Vector3F.Zero, new Vector3F(1, 1, 0.5f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            spectrum = microfacetReflection.F(new Vector3F(-1, 1, 0.25f), new Vector3F(1, -1, -0.25f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            spectrum = microfacetReflection.F(new Vector3F(-1, 1, 0.25f), new Vector3F(1, -1, 0.25f));
            Check.That(spectrum).IsEqualTo(new Spectrum(1.48850453f));
        }
    }
}