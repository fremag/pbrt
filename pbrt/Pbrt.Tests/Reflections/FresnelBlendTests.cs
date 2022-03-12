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
    }
}