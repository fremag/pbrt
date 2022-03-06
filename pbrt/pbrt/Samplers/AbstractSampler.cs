using pbrt.Cameras;
using pbrt.Core;

namespace pbrt.Samplers
{
    public abstract class AbstractSampler
    {
        public static readonly float OneMinusEpsilon = 1 - float.Epsilon;

        public int SamplesPerPixel { get; }
        public Point2I CurrentPixel { get; private set; }
        public int CurrentPixelSampleIndex { get; private set; }

        // Sampler Protected Data 
        public int Array1DOffset { get; private set; }
        public int Array2DOffset { get; private set; }

        public AbstractSampler(int samplesPerPixel)
        {
            SamplesPerPixel = samplesPerPixel;
        }

        public abstract float Get1D();
        public abstract Point2F Get2D();
        public abstract AbstractSampler Clone(int seed);

        public CameraSample GetCameraSample(Point2I pRaster)
        {
            var cs = new CameraSample
            {
                PFilm = (Point2F)pRaster + Get2D(),
                Time = Get1D(),
                PLens = Get2D()
            };

            return cs;
        }

        public int RoundCount(int n)
        {
            return n;
        }

        public virtual bool SetSampleNumber(int sampleNum)
        {
            // Reset array offsets for next pixel sample 
            Array1DOffset = 0;
            Array2DOffset = 0;

            CurrentPixelSampleIndex = sampleNum;
            return CurrentPixelSampleIndex < SamplesPerPixel;
        }

        public virtual bool StartNextSample()
        {
            // Reset array offsets for next pixel sample 
            Array1DOffset = 0;
            Array2DOffset = 0;

            return ++CurrentPixelSampleIndex < SamplesPerPixel;
        }

        public void StartPixel(Point2I p)
        {
            CurrentPixel = p;
            CurrentPixelSampleIndex = 0;

            // Reset array offsets for next pixel sample 
            Array1DOffset = 0;
            Array2DOffset = 0;
        }
    }
}