using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class BSDFTests
    {
        [Test]
        public void TrigonometryTest()
        {
            Check.That(BSDF.CosTheta(new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BSDF.CosTheta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BSDF.CosTheta(new Vector3F(-1, 1, 0.5f))).IsEqualTo(0.5f);

            Check.That(BSDF.Cos2Theta(new Vector3F(1, 1, -1))).IsEqualTo(1);
            Check.That(BSDF.Cos2Theta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BSDF.Cos2Theta(new Vector3F(-1, 1, 0.5f))).IsEqualTo(0.25f);

            Check.That(BSDF.AbsCosTheta(new Vector3F(1, 1, -1))).IsEqualTo(1);
            Check.That(BSDF.AbsCosTheta(new Vector3F(1, 1, 0))).IsEqualTo(0);
            Check.That(BSDF.AbsCosTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(0.5f);

            Check.That(BSDF.Sin2Theta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BSDF.Sin2Theta(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BSDF.Sin2Theta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(0.75f);

            Check.That(BSDF.SinTheta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BSDF.SinTheta(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BSDF.SinTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(MathF.Sqrt(0.75f));

            Check.That(BSDF.TanTheta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BSDF.TanTheta(new Vector3F(1, 1, 0))).Not.IsFinite();
            Check.That(BSDF.TanTheta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(-MathF.Sqrt(3));

            Check.That(BSDF.Tan2Theta(new Vector3F(1, 1, -1))).IsEqualTo(0);
            Check.That(BSDF.Tan2Theta(new Vector3F(1, 1, 0))).Not.IsFinite();
            Check.That(BSDF.Tan2Theta(new Vector3F(-1, 1, -0.5f))).IsEqualTo(3f);

            Check.That(BSDF.Cos2Phi(new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BSDF.Cos2Phi(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BSDF.Cos2Phi(new Vector3F(1, 1, 0.5f))).IsEqualTo(1);

            Check.That(BSDF.Sin2Phi(new Vector3F(1, 1, 1))).IsEqualTo(0);
            Check.That(BSDF.Sin2Phi(new Vector3F(1, 1, 0))).IsEqualTo(1);
            Check.That(BSDF.Sin2Phi(new Vector3F(1, 1, 0.5f))).IsEqualTo(1);

            Check.That(BSDF.CosDPhi(new Vector3F(1, 1, 1), new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BSDF.CosDPhi(new Vector3F(1, 1, 0), new Vector3F(1, 1, 1))).IsEqualTo(1);
            Check.That(BSDF.CosDPhi(new Vector3F(1, 1, 0.5f), new Vector3F(1, 1, 1))).IsEqualTo(1);
        }

        [Test]
        public void ReflectTest()
        {
            Vector3F wo = new Vector3F(1, 1, 0);
            Vector3F n = new Vector3F(0, 1, 0);
            var reflected = BSDF.Reflect(wo, n);
            Check.That(reflected).Check((-1, 1, 0));
        }

        [Test]
        public void RefractTest()
        {
            Vector3F wi = new Vector3F(1, 1, 0);
            Normal3F n = new Normal3F(0, 1, 0);
            var refracted = BSDF.Refract(wi, n, 1f/2, out var wt);
            Check.That(refracted).IsTrue();
            Check.That(wt).Check((-0.5f, -1f, 0));
        }

        [Test]
        public void Reflected_RefractTest()
        {
            Vector3F wi = new Vector3F(1, 1, 0).Normalized();
            Normal3F n = new Normal3F(0, 1, 0);
            var refracted = BSDF.Refract(wi, n, 2, out var wt);
            Check.That(refracted).IsFalse();
            Check.That(wt).IsNull();
        }
    }
}