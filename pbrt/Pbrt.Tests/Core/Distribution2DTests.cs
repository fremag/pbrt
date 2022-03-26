using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Distribution2DTests
    {
        Distribution2D distribution2D = new Distribution2D(new float[]
        {
            4, 0, 0, 1,
            0, 3, 1, 1,
        }, 4, 2);

        [Test]
        public void BasicTest()
        {
            Check.That(distribution2D.PMarginal.Func).ContainsExactly(1.25f, 1.25f);
            Check.That(distribution2D.PConditionalV[0].Func).ContainsExactly(4, 0, 0, 1);
            Check.That(distribution2D.PConditionalV[1].Func).ContainsExactly(0, 3 ,1, 1);
        }

        [Test]
        public void SampleContinuousTest()
        {
            Random r = new Random(1337*42);
            int n = 10_000;
            var count = new int[][] { new[] {0, 0, 0, 0}, new [] {0, 0, 0, 0}};

            for (int i = 0; i < n; i++)
            {
                var pUV = new Point2F((float)r.NextDouble(), (float)r.NextDouble());
                var p = distribution2D.SampleContinuous(pUV, out _);
                var nY = (int)(2 * p.Y).Clamp(0, 1);
                var nX = (int)(4 * p.X).Clamp(0, 3);
                count[nY][nX]++;
            }

            Check.That(count[0][0]/(float)n).IsCloseTo(4/10f, 1e-2);
            Check.That(count[0][1]/(float)n).IsCloseTo(0/10f, 1e-2);
            Check.That(count[0][2]/(float)n).IsCloseTo(0/10f, 1e-2);
            Check.That(count[0][3]/(float)n).IsCloseTo(1/10f, 1e-2);
            
            Check.That(count[1][0]/(float)n).IsCloseTo(0/10f, 1e-2);
            Check.That(count[1][1]/(float)n).IsCloseTo(3/10f, 1e-2);
            Check.That(count[1][2]/(float)n).IsCloseTo(1/10f, 1e-2);
            Check.That(count[1][3]/(float)n).IsCloseTo(1/10f, 1e-2);
        }

        [Test]
        [TestCase(0, 0, 3.2f)]
        [TestCase(0.1f, 0.6f, 2.4f)]
        [TestCase(0.3f, 0.4f, 2.4f)]
        [TestCase(0.6f, 0.7f, 0.8f)]
        [TestCase(0.99f, 0.499f, 0.8f)]
        public void SampleContinuous_PdfTest(float u, float v, float expectedPdf)
        {
            distribution2D.SampleContinuous( new Point2F(u, v), out var pdf);
            Check.That(pdf).IsCloseTo(expectedPdf, 1);
        }

        [Test]
        [TestCase(0, 0, 3.2f)]
        [TestCase(0.1f, 0.6f, 0f)]
        [TestCase(0.3f, 0.4f, 0f)]
        [TestCase(0.6f, 0.7f, 0.8f)]
        [TestCase(0.99f, 0.499f, 0.8f)]
        public void PdfTest(float u, float v, float expectedPdf)
        {
            var pdf = distribution2D.Pdf(new Point2F(u, v));
            Check.That(pdf).IsCloseTo(expectedPdf, 1e-5f);
        }
    }
}