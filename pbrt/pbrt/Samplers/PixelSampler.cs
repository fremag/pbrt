using System;
using System.Collections.Generic;
using System.Linq;
using pbrt.Core;

namespace pbrt.Samplers
{
    public class PixelSampler : AbstractSampler
    {
        public int NSampledDimensions { get; }
        public List<float[]> Samples1D { get; }= new List<float[]>();
        public List<Point2F[]> Samples2D { get; }= new List<Point2F[]>();
        
        public int Current1DDimension { get; private set; }= 0;
        public int  Current2DDimension { get; private set; }= 0;
        public Random Rng { get; }
        
        public PixelSampler(int samplesPerPixel, int nSampledDimensions, int seed = 0) : base(samplesPerPixel)
        {
            Rng = new Random(seed);
            NSampledDimensions = nSampledDimensions;
            for (int i = 0; i < nSampledDimensions; ++i) 
            {
                Samples1D.Add(new float[samplesPerPixel]);
                var point2Fs = Enumerable.Range(0, samplesPerPixel).Select(i=> new Point2F()).ToArray();
                Samples2D.Add(point2Fs);
            }
        }
        
        public override bool StartNextSample() 
        {
            Current1DDimension = 0;
            Current2DDimension = 0;
            return base.StartNextSample();
        }

        public override bool SetSampleNumber(int sampleNum) 
        {
            Current1DDimension = 0;
            Current2DDimension = 0;
            return base.SetSampleNumber(sampleNum);
        }

        public override float Get1D()
        {
            if (Current1DDimension < Samples1D.Count)
            {
                var floats = Samples1D[Current1DDimension++];
                return floats[CurrentPixelSampleIndex];
            }

            return (float)Rng.NextDouble();
        }

        public override Point2F Get2D()
        {
            if (Current2DDimension < Samples2D.Count)
            {
                var point2Fs = Samples2D[Current2DDimension++];
                return point2Fs[CurrentPixelSampleIndex];
            }

            return new Point2F((float)Rng.NextDouble(), (float)Rng.NextDouble());
        }

        public override AbstractSampler Clone(int seed)
        {
            var sampler = new PixelSampler(SamplesPerPixel, NSampledDimensions, seed);
            return sampler;
        }
    }
}