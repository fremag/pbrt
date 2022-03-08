using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;

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
    }
}