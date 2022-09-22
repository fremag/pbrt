using pbrt.Core;

namespace pbrt.Samplers
{
    public abstract class GlobalSampler : AbstractSampler
    {
        public int Dimension { get; private set; }
        ulong intervalSampleIndex;
        const int ArrayStartDim = 5;
        int arrayEndDim;

        public abstract ulong GetIndexForSample(ulong sampleNum);
        public abstract float SampleDimension(ulong index, int dimension);
        
        public GlobalSampler(int samplesPerPixel) : base(samplesPerPixel)
        {
            
        }
        
        public override void StartPixel(Point2I p) 
        {
            base.StartPixel(p);
            Dimension = 0;
            intervalSampleIndex = GetIndexForSample(0);
            // Compute arrayEndDim for dimensions used for array samples 
            arrayEndDim = ArrayStartDim + SampleArray1D.Count + 2 * SampleArray2D.Count;
            
            // Compute 1D array samples for GlobalSampler 
            for (var i = 0; i < Samples1DArraySizes.Count; ++i) {
                var nSamples = Samples1DArraySizes[i] * SamplesPerPixel;
                for (var j = 0; j < nSamples; ++j) 
                {
                    var index = GetIndexForSample((ulong)j);
                    SampleArray1D[i][j] = SampleDimension(index, ArrayStartDim + i);
                }
            }            
            
            // Compute 2D array samples for GlobalSampler
            var dim = ArrayStartDim + Samples1DArraySizes.Count;
            for (var i = 0; i < Samples2DArraySizes.Count; ++i) 
            {
                var nSamples = Samples2DArraySizes[i] * SamplesPerPixel;
                for (var j = 0; j < nSamples; ++j) 
                {
                    var idx = GetIndexForSample((ulong)j);
                    SampleArray2D[i][j] = new Point2F(SampleDimension(idx, dim), SampleDimension(idx, dim + 1));
                }
                dim += 2;
            }                
        }        
        
        public override bool StartNextSample() 
        {
            Dimension = 0;
            intervalSampleIndex = GetIndexForSample((ulong)CurrentPixelSampleIndex + 1);
            return base.StartNextSample();
        }

        public override bool SetSampleNumber(int sampleNum)
        {
            Dimension = 0;
            intervalSampleIndex = GetIndexForSample((ulong)sampleNum);
            return base.SetSampleNumber(sampleNum);
        }

        public override float Get1D() 
        {
            if (Dimension >= ArrayStartDim && Dimension < arrayEndDim)
            {
                Dimension = arrayEndDim;
            }

            return SampleDimension(intervalSampleIndex, Dimension++);
        }

        public override Point2F Get2D() 
        {
            if (Dimension + 1 >= ArrayStartDim && Dimension < arrayEndDim)
            {
                Dimension = arrayEndDim;
            }

            var sampleDimension = SampleDimension(intervalSampleIndex, Dimension);
            var sampleDimension2 = SampleDimension(intervalSampleIndex, Dimension + 1);
            Point2F p = new Point2F(sampleDimension, sampleDimension2);
            Dimension += 2;
            return p;
        }
   }
}