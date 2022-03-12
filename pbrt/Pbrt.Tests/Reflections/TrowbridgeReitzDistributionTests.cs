using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class TrowbridgeReitzDistributionTests
    {
        TrowbridgeReitzDistribution distrib = new TrowbridgeReitzDistribution(0.5f, 1);

        [Test]
        public void BasicTest()
        {
            Check.That(distrib.AlphaX).IsEqualTo(0.5f);
            Check.That(distrib.AlphaY).IsEqualTo(1);
            Check.That(distrib.SampleVisibleArea).IsTrue();
        }

        [Test]
        public void D_Test()
        {
            Check.That(distrib.D(Vector3F.Zero)).IsEqualTo(0f);
            Check.That(distrib.D(new Vector3F(1, 1, 1))).IsEqualTo(0.63661975f);
            Check.That(distrib.D(new Vector3F(-1, 0, 0.5f))).IsEqualTo(0.06027169f);
        }

        [Test]
        public void Lambda_Test()
        {
            Check.That(distrib.Lambda(Vector3F.Zero)).IsEqualTo(0f);
            Check.That(distrib.Lambda(new Vector3F(1, 1, 1))).IsEqualTo(0f);
            Check.That(distrib.Lambda(new Vector3F(-1, 0, 0.5f))).IsEqualTo(0.16143781f);
        }

        [Test]
        public void RoughnessToAlphaTest()
        {
            Check.That(TrowbridgeReitzDistribution.RoughnessToAlpha(1)).IsEqualTo(1.62142f);
        }
    }
}