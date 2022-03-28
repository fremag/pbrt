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

        [Test]
        public void Sample_F_Test()
        {
            Vector3F wo = new Vector3F(0, 0, 1);
            Point2F u = Point2F.Zero;
            Spectrum spectrum = microfacetReflection.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(0.238732398f));
            Check.That(wi).IsEqualTo(new Vector3F(0,0,1));
            Check.That(pdf).IsEqualTo(0.15915494f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_GLOSSY |  BxDFType.BSDF_REFLECTION);
        }
        
        [Test]
        public void Sample_F_NotSameHemisphere_Test()
        {
            Vector3F wo = new Vector3F(0, 1, 1);
            Point2F u = Point2F.Zero;
            Spectrum spectrum = microfacetReflection.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(wi).IsEqualTo(new Vector3F(-0.97609365f, 0.9521873f, -0.37494123f));
            Check.That(pdf).IsEqualTo(0f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_GLOSSY |  BxDFType.BSDF_REFLECTION);
        }

        [Test]
        public void PdfTest()
        {
            Vector3F wo = new Vector3F(-1, -1, -1);
            // not same hemisphere
            Vector3F wi = new Vector3F( 1,  1,  1);
            var pdf = microfacetReflection.Pdf(wo, wi);
            Check.That(pdf).IsEqualTo(0f);
            
            // Same hemisphere
            wi = new Vector3F( -1,  -1,  -0.5f);
            pdf = microfacetReflection.Pdf(wo, wi);
            Check.That(pdf).IsCloseTo(0.00045551f, 1e-7);
        }
    }
}