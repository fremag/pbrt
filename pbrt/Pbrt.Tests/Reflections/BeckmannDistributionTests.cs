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

        [Test]
        [TestCase(0.5f, 0.1f,0.2f, -1.326897f, -0.5951161f)]
        [TestCase(1f, 0.1f,0.2f, 0.100304715f, 0.3087062f)]
        [TestCase(-1f, 0.1f, 0.2f, float.NaN, -0.5951161f)]
        public void BeckmannSample11Test(float cosThetaI, float u1, float u2, float expectedSlopeX, float expectedSlopeY)
        {
            BeckmannDistribution.BeckmannSample11(cosThetaI, u1, u2, out var slopeX, out var slopeY);
            if (float.IsNaN(expectedSlopeX))
            {
                Check.That(slopeX).IsNaN();
            }
            else
            {
                Check.That(slopeX).IsEqualTo(expectedSlopeX);
            }

            Check.That(slopeY).IsEqualTo(expectedSlopeY);
        }

        [Test]
        public void BeckmannSampleTest()
        {
            Vector3F wi = new Vector3F(1, -1, 1);
            float alphax = 0.1f;
            float alphaY = 0.2f;
            float u1 = 0.3f;
            float u2 = 0.4f;
            Vector3F x = BeckmannDistribution.BeckmannSample(wi, alphax, alphaY, u1, u2);
            Check.That(x).IsEqualTo(new Vector3F(0.03725957f, -0.06917048f, 0.99690884f));
        }
        
        [Test]
        public void Sample_wh_SampleVisibleArea_Test()
        {
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distrib.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0.42993078f, -0.85986155f, -0.27531376f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_1_Test()
        {
            BeckmannDistribution distribution = new BeckmannDistribution(0.5f, 1, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(-7.54979E-08f, 1f, -0f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_2_Test()
        {
            BeckmannDistribution distribution = new BeckmannDistribution(0.5f, 0.5f, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0f, 0f, -1f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_3_Test()
        {
            BeckmannDistribution distribution = new BeckmannDistribution(0.5f, 1f, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0.75f);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(1f, 3.2584137E-07f, 0f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_4_Test()
        {
            BeckmannDistribution distribution = new BeckmannDistribution(0.5f, 0.5f, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(1f, 1f);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0f, 0f, -1f));
        }
    }
}