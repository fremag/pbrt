using System;
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
        public void Sample_FTest()
        {
            Vector3F wo = new Vector3F(1, 1, 1);
            Point2F sample = Point2F.Zero;
            Check.ThatCode(() => fresnelSpecular.Sample_f(wo, out _, sample, out _, out _)).Throws<NotImplementedException>();
        }
    }
}