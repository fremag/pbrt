using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Samplers;

namespace Pbrt.Tests.Samplers
{
    [TestFixture]
    public class StratifiedSamplerTests
    {
        private readonly StratifiedSampler sampler = new StratifiedSampler(2, 2, true, 1);

        [Test]
        public void BasicTest()
        {
            Check.That(sampler.JitterSamples).IsTrue();
            Check.That(sampler.XPixelSamples).IsEqualTo(2);
            Check.That(sampler.YPixelSamples).IsEqualTo(2);
            Check.That(sampler.NSampledDimensions).IsEqualTo(1);
        }

        [Test]
        public void Stratified1D_NoJitterTest()
        {
            int n = 10;
            var rng = new Random(0);
            float[] samples = new float[n];
            StratifiedSampler.StratifiedSample1D(samples, 0, n, rng, false);

            for (int i = 0; i < n; i++)
            {
                Check.That(samples[i]).IsCloseTo((i+0.5f)/n, 1e-3);
            }
        }

        [Test]
        public void Stratified1D_JitterTest()
        {
            int m = 10000;
            int n = 10;
            var rng = new Random(0);
            float[] samples = new float[n];
            float[] sumSamples = new float[n];

            for (int j = 0; j < m; j++)
            {
                StratifiedSampler.StratifiedSample1D(samples, 0, n, rng, true);
                for (int i = 0; i < n; i++)
                {
                    sumSamples[i] += samples[i];
                }
            }

            for (int i = 0; i < n; i++)
            {
                Check.That(sumSamples[i]/m).IsCloseTo((i + 0.5f) / n, 1e-3);
            }        
        }
        
        [Test]
        public void Stratified2D_NoJitterTest()
        {
            int nx = 3;
            int ny = 4;
            var rng = new Random(0);
            var samples = Enumerable.Range(0, nx * ny).Select(_ => new Point2F()).ToArray();
            StratifiedSampler.StratifiedSample2D(samples, nx, ny, rng, false);

            int k = 0;
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    var point2F = samples[k];
                    var expectedX = (i+0.5f)/nx;
                    Check.That(point2F.X).IsCloseTo(expectedX, 1e-3);
                    var expectedY = (j+0.5f)/ny;
                    Check.That(point2F.Y).IsCloseTo(expectedY, 1e-3);
                    k++;
                }
            }
        }

        [Test]
        public void Stratified2D_JitterTest()
        {
            int m = 100_000;
            int nx = 2;
            int ny = 1;
            var rng = new Random(0);
            var samples = Enumerable.Range(0, nx * ny).Select(_ => new Point2F()).ToArray();
            StratifiedSampler.StratifiedSample2D(samples, nx, ny, rng, false);
            Point2F[] sumSamples = Enumerable.Range(0, nx * ny).Select(_ => new Point2F()).ToArray();;

            for (int j = 0; j < m; j++)
            {
                StratifiedSampler.StratifiedSample2D(samples, nx, ny, rng, true);
                for (int i = 0; i < nx*ny; i++)
                {
                    sumSamples[i] += samples[i];
                }
            }

            int k = 0;
            for (int j = 0; j < ny; j++)
            {
                for (int i = 0; i < nx; i++)
                {
                    var point2F = sumSamples[k];
                    var expectedX = (i+0.5f)/nx;
                    Check.That(point2F.X / m).IsCloseTo(expectedX, 1e-3);
                    var expectedY = (j+0.5f)/ny;
                    Check.That(point2F.Y/ m).IsCloseTo(expectedY, 1e-3);
                    k++;
                }
            }
        }

        [Test]
        public void ShuffleTest()
        {
            var rng = new Random(0);
            int n = 10;
            int[] values = Enumerable.Range(1, n).ToArray();
            int[] sumValues = new int[n];
            int m = 1_000_000;
            
            for (int i = 0; i < m; i++)
            {
                StratifiedSampler.Shuffle(values, 0, n, 1, rng);
                for (int j = 0; j < n; j++)
                {
                    sumValues[j] += values[j];
                }
            }

            float expectedValue = m*(n + 1) / 2f; 
            for (int j = 0; j < n; j++)
            {
                var sumValue = (float)sumValues[j];
                var ratio = sumValue / expectedValue; 
                Check.That(ratio).IsCloseTo(1, n*MathF.Sqrt(m));
            }
        }
    }
}