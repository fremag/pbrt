using pbrt.Core;

namespace pbrt.Samplers
{
    public abstract class GlobalSampler : AbstractSampler
    {
        int dimension;
        long intervalSampleIndex;
        const int ArrayStartDim = 5;
        int arrayEndDim;

        public abstract int GetIndexForSample(long sampleNum);
        public abstract float SampleDimension(long index, int dimension);
        
        public GlobalSampler(int samplesPerPixel) : base(samplesPerPixel)
        {
            
        }
        
        public override void StartPixel(Point2I p) 
        {
            base.StartPixel(p);
            dimension = 0;
            intervalSampleIndex = GetIndexForSample(0);
            // Compute arrayEndDim for dimensions used for array samples 
            arrayEndDim = ArrayStartDim + SampleArray1D.Count + 2 * SampleArray2D.Count;
            
            // Compute 1D array samples for GlobalSampler 
            for (var i = 0; i < Samples1DArraySizes.Count; ++i) {
                var nSamples = Samples1DArraySizes[i] * SamplesPerPixel;
                for (var j = 0; j < nSamples; ++j) 
                {
                    var index = GetIndexForSample(j);
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
                    var idx = GetIndexForSample(j);
                    SampleArray2D[i][j] = new Point2F(SampleDimension(idx, dim), SampleDimension(idx, dim + 1));
                }
                dim += 2;
            }                
        }        
        
        public override bool StartNextSample() 
        {
            dimension = 0;
            intervalSampleIndex = GetIndexForSample(CurrentPixelSampleIndex + 1);
            return base.StartNextSample();
        }

        public override bool SetSampleNumber(int sampleNum) {
            dimension = 0;
            intervalSampleIndex = GetIndexForSample(sampleNum);
            return base.SetSampleNumber(sampleNum);
        }

        public override float Get1D() 
        {
            if (dimension >= ArrayStartDim && dimension < arrayEndDim)
            {
                dimension = arrayEndDim;
            }

            return SampleDimension(intervalSampleIndex, dimension++);
        }

        public override Point2F Get2D() 
        {
            if (dimension + 1 >= ArrayStartDim && dimension < arrayEndDim)
            {
                dimension = arrayEndDim;
            }

            var sampleDimension = SampleDimension(intervalSampleIndex, dimension);
            var sampleDimension2 = SampleDimension(intervalSampleIndex, dimension + 1);
            Point2F p = new Point2F(sampleDimension, sampleDimension2);
            dimension += 2;
            return p;
        }
   }
}