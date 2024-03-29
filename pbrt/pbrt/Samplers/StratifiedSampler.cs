using System;
using System.Linq;
using pbrt.Core;

namespace pbrt.Samplers
{
    public class StratifiedSampler : PixelSampler
    {
        public int XPixelSamples { get; set; }
        public int YPixelSamples { get; set; }
        public bool JitterSamples { get; set; }
        
        public StratifiedSampler(int xPixelSamples, int yPixelSamples, bool jitterSamples, int nSampledDimensions, int seed=0)
            : base(xPixelSamples * yPixelSamples, nSampledDimensions, seed)
        {
            XPixelSamples = xPixelSamples;
            YPixelSamples = yPixelSamples;
            JitterSamples = jitterSamples;
        }
        
        public override void StartPixel(Point2I p) 
        {
            // Generate single stratified samples for the pixel
            for (var i = 0; i < Samples1D.Count; ++i) 
            {
                var sample1D = Samples1D[i];
                StratifiedSample1D(sample1D, 0, XPixelSamples * YPixelSamples, Rng, JitterSamples);
                MathUtils.Shuffle(sample1D, 0, XPixelSamples * YPixelSamples, 1, Rng);
            }
            
            for (var i = 0; i < Samples2D.Count; ++i) 
            {
                var sample2D = Samples2D[i];
                StratifiedSample2D(sample2D, XPixelSamples, YPixelSamples, Rng, JitterSamples);
                MathUtils.Shuffle(sample2D, 0, XPixelSamples * YPixelSamples, 1, Rng);
            }            
            
            // Generate arrays of stratified samples for the pixel 
            for (var i = 0; i < Samples1DArraySizes.Count; ++i)
            {
                for (var j = 0; j < SamplesPerPixel; ++j) 
                {
                    int count = Samples1DArraySizes[i];
                    var sampleArray1D = SampleArray1D[i];
                    StratifiedSample1D(sampleArray1D, j * count, count, Rng, JitterSamples);
                    MathUtils.Shuffle(sampleArray1D, j * count, count, 1, Rng);
                }
            }

            for (var i = 0; i < Samples2DArraySizes.Count; ++i)
            {
                for (var j = 0; j < SamplesPerPixel; ++j) 
                {
                    int count = Samples2DArraySizes[i];
                    var sampleArray2D = SampleArray2D[i];
                    LatinHypercube(sampleArray2D, j * count, Rng);
                }
            }

            base.StartPixel(p);
        }
        
        public static void LatinHypercube(Point2F[] samples, int nSamples, Random rng) 
        {
            // Generate LHS samples along diagonal 
            float invNSamples = 1f / nSamples;
            for (int i = 0; i < nSamples; ++i)
            {
                float sX = (i + ((float)rng.NextDouble())) * invNSamples;
                float sY = (i + ((float)rng.NextDouble())) * invNSamples;
                samples[i].X = MathF.Min(sX, OneMinusEpsilon);
                samples[i].Y = MathF.Min(sY, OneMinusEpsilon);
            }
            
            // Permute LHS samples in each dimension
            for (int i = 0; i < nSamples; ++i) 
            {
                int other = rng.Next(nSamples - i);
                var item1 = samples[i];
                var item2 = samples[other+i];
                samples[i] = item2;
                samples[other + i] = item1;
            }
        }        
        
        public static void StratifiedSample1D(float[] samp, int start, int nSamples, Random rng, bool jitter) 
        {
            var invNSamples = 1f / nSamples;
            for (int i = start; i < nSamples; ++i) 
            {
                var delta = jitter ? (float)rng.NextDouble() : 0.5f;
                samp[i] = MathF.Min((i + delta) * invNSamples, OneMinusEpsilon);
            }
        }    
        
        public static void StratifiedSample2D(Point2F[] samp, int nx, int ny, Random rng, bool jitter)
        {
            var dx = 1f / nx;
            var dy = 1f / ny;
            int k = 0;
            for (var y = 0; y < ny; ++y)
            {
                for (var x = 0; x < nx; ++x) 
                {
                    var jx = jitter ? (float)rng.NextDouble() : 0.5f;
                    var jy = jitter ? (float)rng.NextDouble() : 0.5f;
                    
                    samp[k].X = MathF.Min((x + jx) * dx, OneMinusEpsilon);
                    samp[k].Y = MathF.Min((y + jy) * dy, OneMinusEpsilon);
                    ++k;
                }
            }
        }
        
        public override AbstractSampler Clone(int seed)
        {
            var sampler = new StratifiedSampler(XPixelSamples, YPixelSamples, JitterSamples, NSampledDimensions, seed);
            sampler.Samples1DArraySizes.AddRange(Samples1DArraySizes);
            sampler.Samples2DArraySizes.AddRange(Samples2DArraySizes);
            foreach (var sampleArray1D in SampleArray1D)
            {
                var copy = sampleArray1D.ToArray();
                sampler.SampleArray1D.Add(copy);
            }
            foreach (var sampleArray2D in SampleArray2D)
            {
                var copy = sampleArray2D.ToArray();
                sampler.SampleArray2D.Add(copy);
            }

            return sampler;
        }
    }
}