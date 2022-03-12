using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class BxDFTests
    {
        [Test]
        [TestCase(0f, 1f, 1f, 1f)]
        [TestCase(-0.707f, 1f, 1f, 0f)]
        [TestCase(0.5f, 0.5f, 1f, 0.1613f)]
        [TestCase(0f, 0.5f, 1f, 1f)]
        public void FrDielectricTest(float cosThetaI, float etaI, float etaT, float expectedValue)
        {
            var f = BxDF.FrDielectric(cosThetaI, etaI, etaT);
            Check.That(f).IsCloseTo(expectedValue, 1e-4);
        }
        
        [Test]
        public void FrConductorTest()
        {
            var cosThetaI = 0.707f;
            var etaI = new Spectrum(new float [] {0.25f, 0.5f});
            var etaT = new Spectrum(new float [] {0.2f, 0.4f});
            var k = new Spectrum(new float [] {0.1f, 0.2f});
            var expected = new Spectrum(new float [] {0.108254164f, 0.108254164f});
            var f = BxDF.FrConductor(cosThetaI, etaI, etaT, k);
            Check.That(f).IsEqualTo(expected);
        }

        [Test]
        public void Vector_SameHemisphereTest()
        {
            Check.That(BxDF.SameHemisphere(new Vector3F(1, 1, 1), new Vector3F(2, -1, 2))).IsTrue();
            Check.That(BxDF.SameHemisphere(new Vector3F(0, 0, 1), new Vector3F(0, 0, 1))).IsTrue();
            Check.That(BxDF.SameHemisphere(new Vector3F(0, 0, 0), new Vector3F(2, -1, 2))).IsFalse();
            Check.That(BxDF.SameHemisphere(new Vector3F(1, 1, -1), new Vector3F(2, -1, 2))).IsFalse();
        }
        
        [Test]
        public void Normal_SameHemisphereTest()
        {
            Check.That(BxDF.SameHemisphere(new Vector3F(1, 1, 1), new Normal3F(2, -1, 2))).IsTrue();
            Check.That(BxDF.SameHemisphere(new Vector3F(0, 0, 1), new Normal3F(0, 0, 1))).IsTrue();
            Check.That(BxDF.SameHemisphere(new Vector3F(0, 0, 0), new Normal3F(2, -1, 2))).IsFalse();
            Check.That(BxDF.SameHemisphere(new Vector3F(1, 1, -1), new Normal3F(2, -1, 2))).IsFalse();
        }
        
        [Test]
        public void TrigonometryTest()
        {
            Check.That(BxDF.CosTheta(new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BxDF.CosTheta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BxDF.CosTheta(new Vector3F(-1, 1, 0.5f))).IsEqualTo(0.5f);

            Check.That(BxDF.Cos2Theta(new Vector3F(1, 1, -1))).IsEqualTo(1);
            Check.That(BxDF.Cos2Theta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BxDF.Cos2Theta(new Vector3F(-1, 1, 0.5f))).IsEqualTo(0.25f);

            Check.That(BxDF.AbsCosTheta(new Vector3F(1, 1, -1))).IsEqualTo(1);
            Check.That(BxDF.AbsCosTheta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BxDF.AbsCosTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(0.5f);

            Check.That(BxDF.Sin2Theta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BxDF.Sin2Theta(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BxDF.Sin2Theta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(0.75f);

            Check.That(BxDF.SinTheta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BxDF.SinTheta(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BxDF.SinTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(MathF.Sqrt(0.75f));

            Check.That(BxDF.TanTheta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BxDF.TanTheta(new Vector3F(1, 1, 0))).Not.IsFinite();
            Check.That(BxDF.TanTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(-MathF.Sqrt(3));

            Check.That(BxDF.Tan2Theta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BxDF.Tan2Theta(new Vector3F(1, 1, 0))).Not.IsFinite();
            Check.That(BxDF.Tan2Theta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(3f);

            Check.That(BxDF.Cos2Phi(new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BxDF.Cos2Phi(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BxDF.Cos2Phi(new Vector3F(1, 1, 0.5f))).IsEqualTo(1);

            Check.That(BxDF.Sin2Phi(new Vector3F(1, 1, 1))).IsEqualTo(0);
            Check.That(BxDF.Sin2Phi(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BxDF.Sin2Phi(new Vector3F(1, 1, 0.5f))).IsEqualTo(1);

            Check.That(BxDF.CosDPhi(new Vector3F(1, 1, 1), new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BxDF.CosDPhi(new Vector3F(1, 1, 0), new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BxDF.CosDPhi(new Vector3F(1, 1, 0.5f), new Vector3F(1, 1, 1))).IsEqualTo(1);
        }
        
    }
}