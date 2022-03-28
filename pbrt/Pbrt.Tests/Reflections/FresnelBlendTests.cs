using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class FresnelBlendTests
    {
        private readonly Spectrum rd = new Spectrum(2f);
        private readonly Spectrum rs = new Spectrum(3f);
        private readonly BeckmannDistribution distribution = new  BeckmannDistribution(0.5f, 0.7f);
        private FresnelBlend fresnelBlend;

        [SetUp]
        public void SetUp()
        {
            fresnelBlend = new FresnelBlend(rd, rs, distribution);
            Check.That(fresnelBlend.Rd).IsSameReferenceAs(rd);
            Check.That(fresnelBlend.Rs).IsSameReferenceAs(rs);
            Check.That(fresnelBlend.Distribution).IsSameReferenceAs(distribution);
            Check.That(fresnelBlend.BxdfType).IsEqualTo(BxDFType.BSDF_GLOSSY | BxDFType.BSDF_REFLECTION);
        }

        [Test]
        public void Pow5Test()
        {
            Check.That(FresnelBlend.Pow5(2)).IsEqualTo(2*2*2*2*2);
        }

        [Test]
        public void SchlickFresnelTest()
        {
            Check.That(fresnelBlend.SchlickFresnel(1)).IsEqualTo(new Spectrum(3f));
            Check.That(fresnelBlend.SchlickFresnel(0)).IsEqualTo(new Spectrum(1f));
        }

        [Test]
        public void F_Test()
        {
            Vector3F wo = new Vector3F(0, 0, 1);
            Vector3F wi = new Vector3F(0, 0, -1);
            var spectrum = fresnelBlend.F(wo, wi);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            wo = new Vector3F(0.5f, 0, 1);
            wi = new Vector3F(0.5f, 0, -1);
            spectrum = fresnelBlend.F(wo, wi);
            Check.That(spectrum).IsEqualTo(new Spectrum(-1.45466769f));
        }

        [Test]
        public void PdfTest()
        {
            Vector3F wo = new Vector3F(-1, -1, -1);
            // not same hemisphere
            Vector3F wi = new Vector3F( 1,  1,  1);
            var pdf = fresnelBlend.Pdf(wo, wi);
            Check.That(pdf).IsEqualTo(0f);
            
            // Same hemisphere
            wi = new Vector3F( -1,  -1,  -0.5f);
            pdf = fresnelBlend.Pdf(wo, wi);
            Check.That(pdf).IsCloseTo(0.0796286091f, 1e-7);
        }

        [Test]
        public void Sample_F_Test()
        {
            Vector3F wo = new Vector3F(0, 0, -1);
            Point2F u = new Point2F(0, 0);
            var spectrum = fresnelBlend.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            Check.That(wi).IsEqualTo(new Vector3F(-0.707106769f, -0.707106769f, -0.000172633489f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0.187419236f));
            Check.That(pdf).IsEqualTo(0.0222251881f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY);
        }

        [Test]
        public void Sample_F_Bis_Test()
        {
            // Same hemisphere
            Vector3F wo = new Vector3F(0, 0, -1);
            Point2F u = new Point2F(0.75f, 0);
            var spectrum = fresnelBlend.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            Check.That(wi).IsEqualTo(new Vector3F(0.709591746f, 0f, -0.70461297f));
            Check.That(spectrum).IsEqualTo(new Spectrum(-0.82189554f));
            Check.That(pdf).IsCloseTo(0.1903901f, 1e-7);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY);

            // NOT Same hemisphere
            wo = new Vector3F(1, 0.5f, -1);
            spectrum = fresnelBlend.Sample_f(wo, out wi, u, out pdf, out sampledType);
            Check.That(wi).IsEqualTo(new Vector3F(-0.785873592f, 1.19873559f, 0.442080736f));
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(pdf).IsCloseTo(0f, 1e-7);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY);
        }
    }
}