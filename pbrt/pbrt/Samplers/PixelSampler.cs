using System;
using System.Collections.Generic;
using pbrt.Core;

namespace pbrt.Samplers
{
    public class PixelSampler : AbstractSampler
    {
        public int NSampledDimensions { get; }
        public List<List<float>> Samples1D { get; }= new List<List<float>>();
        public List<List<Point2F>> Samples2D { get; }= new List<List<Point2F>>();
        
        public int Current1DDimension { get; private set; }= 0;
        public int  Current2DDimension { get; private set; }= 0;
        public Random Rng { get; private set; }= new Random(0);
        
        public PixelSampler(int samplesPerPixel, int nSampledDimensions) : base(samplesPerPixel)
        {
            NSampledDimensions = nSampledDimensions;
            for (int i = 0; i < nSampledDimensions; ++i) 
            {
                Samples1D.Add(new List<float>(samplesPerPixel));
                Samples2D.Add(new List<Point2F>(samplesPerPixel));
            }
        }
        
        public override bool StartNextSample() 
        {
            Current1DDimension = Current2DDimension = 0;
            return base.StartNextSample();
        }

        public override bool SetSampleNumber(int sampleNum) 
        {
            Current1DDimension = Current2DDimension = 0;
            return base.SetSampleNumber(sampleNum);
        }

        public override float Get1D()
        {
            if (Current1DDimension < Samples1D.Count)
            {
                return Samples1D[Current1DDimension++][CurrentPixelSampleIndex];
            }

            return (float)Rng.NextDouble();
        }

        public override Point2F Get2D()
        {
            if (Current2DDimension < Samples2D.Count)
            {
                return Samples2D[Current2DDimension++][CurrentPixelSampleIndex];
            }

            return new Point2F((float)Rng.NextDouble(), (float)Rng.NextDouble());
        }

        public override AbstractSampler Clone(int seed)
        {
            var sampler = new PixelSampler(SamplesPerPixel, NSampledDimensions);
            sampler.Rng = new Random(seed);
            return sampler;
        }
    }
}