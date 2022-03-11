using System;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class LambertianTransmissionTests
    {
        [Test]
        public void BasicTest()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.T).IsEqualTo(t);
            Check.That(lambTrans.BxdfType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_DIFFUSE);
        }

        [Test]
        public void F_Test()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.F(null, null)).IsEqualTo(new Spectrum(3f/MathF.PI));
        }
        
        [Test]
        public void SampleF_Test()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.ThatCode(() => lambTrans.Sample_f(null, out _, null, out _, out _)).Throws<NotImplementedException>();
        }

        [Test]
        public void Rho_Test()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.Rho(null, 0, null)).IsEqualTo(t);
        }
    }
}