using System;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class LambertianReflectionTests
    {
        [Test]
        public void BasicTest()
        {
            Spectrum r = new Spectrum(3f);
            var lambTrans = new LambertianReflection(r);
            Check.That(lambTrans.R).IsEqualTo(r);
            Check.That(lambTrans.BxdfType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE);
        }

        [Test]
        public void F_Test()
        {
            Spectrum r = new Spectrum(3f);
            var lambTrans = new LambertianReflection(r);
            Check.That(lambTrans.F(null, null)).IsEqualTo(new Spectrum(3f/MathF.PI));
        }
        
        [Test]
        public void SampleF_Test()
        {
            Spectrum r = new Spectrum(3f);
            var lambTrans = new LambertianReflection(r);
            Check.ThatCode(() => lambTrans.Sample_f(null, out _, null, out _, out _)).Throws<NotImplementedException>();
        }

        [Test]
        public void Rho_Test()
        {
            Spectrum r = new Spectrum(3f);
            var lambTrans = new LambertianReflection(r);
            Check.That(lambTrans.Rho(null, 0, null)).IsEqualTo(r);
        }
    }
}