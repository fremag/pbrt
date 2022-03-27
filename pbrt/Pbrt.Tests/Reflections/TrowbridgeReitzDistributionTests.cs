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

        [Test]
        public void TrowbridgeReitzSample11Test()
        {
            TrowbridgeReitzDistribution.TrowbridgeReitzSample11(0, 0, 0, out var slopeX, out var slopeY);
            Check.That(slopeX).IsNaN();
            Check.That(slopeY).IsNaN();
            
            TrowbridgeReitzDistribution.TrowbridgeReitzSample11(1, 0, 0, out slopeX, out slopeY);
            Check.That(slopeX).IsEqualTo(0);
            Check.That(slopeY).IsEqualTo(0);
            
            TrowbridgeReitzDistribution.TrowbridgeReitzSample11(0.1f, 1e-11f, 1, out slopeX, out slopeY);
            Check.That(slopeX).IsEqualTo(0);
            Check.That(slopeY).IsEqualTo(7.25636053f);
        }

        [Test]
        public void TrowbridgeReitzSampleTest()
        {
            Vector3F wi = new Vector3F(1, 0, -1);
            float alphaX = 0.1f;
            float alphaY = 0.2f;
            float u1 = 1;
            float u2 = 2;
            Vector3F x = TrowbridgeReitzDistribution.TrowbridgeReitzSample(wi, alphaX, alphaY, u1, u2);
            Check.That(x).IsEqualTo(new Vector3F(0.482704699f, -0.730736792f, 0.482721448f) );
        }

        [Test]
        public void Sample_wh_SampleVisibleArea_Test()
        {
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distrib.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0.964052379f, 0f, -0.26571238f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_1_Test()
        {
            TrowbridgeReitzDistribution distribution = new TrowbridgeReitzDistribution(0.5f, 1, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0f, 0f, -1f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_2_Test()
        {
            TrowbridgeReitzDistribution distribution = new TrowbridgeReitzDistribution(0.5f, 0.5f, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0f, 0f, -1f));
        }
        
        [Test]
        public void Sample_wh_No_SampleVisibleArea_3_Test()
        {
            TrowbridgeReitzDistribution distribution = new TrowbridgeReitzDistribution(0.5f, 1f, false);
            Vector3F wo = new Vector3F(0, -1, -1);
            Point2F u = new Point2F(0, 0.75f);
            Vector3F wh = distribution.Sample_wh(wo, u);
            Check.That(wh).IsEqualTo(new Vector3F(0f, 0f, -1f));
        }
    }
}