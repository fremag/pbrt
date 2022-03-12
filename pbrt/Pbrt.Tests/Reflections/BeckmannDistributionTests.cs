using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class BeckmannDistributionTests
    {
        BeckmannDistribution distrib = new BeckmannDistribution(0.5f, 1);

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
            Check.That(distrib.D(new Vector3F(-1, 0, 0.5f))).IsEqualTo(6.258443E-05f);
        }

        [Test]
        public void Lambda_Test()
        {
            Check.That(distrib.Lambda(Vector3F.Zero)).IsEqualTo(0f);
            Check.That(distrib.Lambda(new Vector3F(1, 1, 1))).IsEqualTo(0f);
            Check.That(distrib.Lambda(new Vector3F(-1, 0, 0.5f))).IsEqualTo(0.01061996f);
        }

        [Test]
        public void G_Test()
        {
            Check.That(distrib.G(Vector3F.Zero, new Vector3F(1, 0, 0))).IsEqualTo(1f);
            Check.That(distrib.G(new Vector3F(1, 0, 0.5f), new Vector3F(1, 0, 0.25f))).IsEqualTo(0.83344734f);
        }
        
        [Test]
        public void G1_Test()
        {
            Check.That(distrib.G1(new Vector3F(1, 0, 0))).IsEqualTo(1f);
        }
    }
}