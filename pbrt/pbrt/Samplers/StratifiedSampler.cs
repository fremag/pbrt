using System;
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
                StratifiedSample1D(Samples1D[i], 0, XPixelSamples * YPixelSamples, Rng, JitterSamples);
                Shuffle(Samples1D[i], 0, XPixelSamples * YPixelSamples, 1, Rng);
            }
            
            for (var i = 0; i < Samples2D.Count; ++i) 
            {
                StratifiedSample2D(Samples2D[i], XPixelSamples, YPixelSamples, Rng, JitterSamples);
                Shuffle(Samples2D[i], 0, XPixelSamples * YPixelSamples, 1, Rng);
            }            
            
            // Generate arrays of stratified samples for the pixel 
            for (var i = 0; i < Samples1DArraySizes.Count; ++i)
            {
                for (var j = 0; j < SamplesPerPixel; ++j) 
                {
                    int count = Samples1DArraySizes[i];
                    StratifiedSample1D(SampleArray1D[i], j * count, count, Rng, JitterSamples);
                    Shuffle(SampleArray1D[i], j * count, count, 1, Rng);
                }
            }

            for (var i = 0; i < Samples2DArraySizes.Count; ++i)
            {
                for (var j = 0; j < SamplesPerPixel; ++j) 
                {
                    int count = Samples2DArraySizes[i];
                    LatinHypercube(SampleArray2D[i], j * count, Rng);
                }
            }

            base.StartPixel(p);
        }
        
        public void LatinHypercube(Point2F[] samples, int nSamples, Random rng) 
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
        
        public void StratifiedSample1D(float[] samp, int start, int nSamples, Random rng, bool jitter) 
        {
            var invNSamples = 1f / nSamples;
            for (int i = start; i < nSamples; ++i) 
            {
                var delta = jitter ? (float)rng.NextDouble() : 0.5f;
                samp[i] = MathF.Min((i + delta) * invNSamples, OneMinusEpsilon);
            }
        }    
        
        public void StratifiedSample2D(Point2F[] samp, int nx, int ny, Random rng, bool jitter)
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
        
        public static void Shuffle<T>(T[] samp, int start, int count, int nDimensions, Random rng) 
        {
            for (int i = start; i < count; ++i) 
            {
                int other = i + rng.Next(count - i);
                for (int j = 0; j < nDimensions; ++j)
                {
                    var item1 = samp[nDimensions * i + j];
                    var item2 = samp[nDimensions * other + j];
                    samp[nDimensions * i + j] = item2;
                    samp[nDimensions * other + j] = item1;
                }
            }
        }
        
        public override AbstractSampler Clone(int seed)
        {
            var sampler = MemberwiseClone();
            return (AbstractSampler)sampler;
        }
    }
}