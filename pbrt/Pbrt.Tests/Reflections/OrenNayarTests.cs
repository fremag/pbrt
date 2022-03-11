using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class OrenNayarTests
    {
        private readonly Spectrum r = new Spectrum(2f);

        [Test]
        public void BasicTest()
        {
            var orenNayar = new OrenNayar(r, 0);
            Check.That(orenNayar.A).IsEqualTo(1f);
            Check.That(orenNayar.B).IsEqualTo(0f);
            Check.That(orenNayar.R).IsEqualTo(r);
            Check.That(orenNayar.BxdfType).IsEqualTo(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE);

            orenNayar = new OrenNayar(r, 1f);
            Check.That(orenNayar.A).IsEqualTo(0.9995389f);
            Check.That(orenNayar.B).IsEqualTo(0.0015179493f);
        }

        [Test]
        public void F_1_Test()
        {
            var orenNayar = new OrenNayar(r, 1f);
            Vector3F wo = new Vector3F(1, 1, 1);
            Vector3F wi = new Vector3F(1, 1, -1);
            var s = orenNayar. F(wo, wi);
            Check.That(s).IsEqualTo(r * 0.318163097f);
        }

        [Test]
        public void F_2_Test()
        {
            var orenNayar = new OrenNayar(r, 1f);
            Vector3F wo = new Vector3F(1, 1, -MathF.Sqrt(2)/3);
            Vector3F wi = new Vector3F(1, 1, MathF.Sqrt(2)/2);
            var s = orenNayar. F(wo, wi);
            Check.That(s).IsEqualTo(r * 0.319015354f);
        }
    }
}