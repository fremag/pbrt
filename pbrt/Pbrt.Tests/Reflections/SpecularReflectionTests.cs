using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class SpecularReflectionTests
    {
        private readonly Fresnel fresnel = new FresnelNoOp();
        private readonly Spectrum r = new Spectrum(2f);
        private SpecularReflection specRef;

        [SetUp]
        public void SetUp()
        {
            specRef = new SpecularReflection(r, fresnel);
        }
        
        [Test]
        public void FTest()
        {
            Check.That(specRef.Fresnel).IsEqualTo(fresnel);
            Check.That(specRef.R).IsEqualTo(r);
            Check.That(specRef.BxdfType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR);
            Check.That(specRef.F(null, null)).IsEqualTo(new Spectrum(0f));
        }

        [Test]
        public void Sample_FTest()
        {
            Vector3F wo = new Vector3F(1, 1, 1);
            Point2F sample = Point2F.Zero;
            var spec = specRef.Sample_f(wo, out var wi, sample, out var pdf, out var sampledType);
            Check.That(spec).IsEqualTo(new Spectrum(2f));
            Check.That(wi).Check((-1, -1, 1));
            Check.That(pdf).IsEqualTo(1f);
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR);
        }
    }
}