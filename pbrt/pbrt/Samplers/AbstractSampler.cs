using System.Collections.Generic;
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
        public List<int> Samples1DArraySizes { get; } = new List<int>();
        public List<int> Samples2DArraySizes { get; } = new List<int>();
        public List<List<float>> SampleArray1D { get; } = new List<List<float>>();
        public List<List<Point2F>> SampleArray2D { get; } = new List<List<Point2F>>();
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

        public virtual void Request1DArray(int n)
        {
            Samples1DArraySizes.Add(n);
            SampleArray1D.Add(new List<float>(n * SamplesPerPixel));
        }

        public virtual void Request2DArray(int n)
        {
            Samples2DArraySizes.Add(n);
            SampleArray2D.Add(new List<Point2F>(n * SamplesPerPixel));
        }

        public int RoundCount(int n)
        {
            return n;
        }

        public virtual float Get1DArray(int n)
        {
            if (Array1DOffset == SampleArray1D.Count)
            {
                return float.NaN;
            }

            var floats = SampleArray1D[Array1DOffset++];
            return floats[CurrentPixelSampleIndex * n];
        }

        public virtual Point2F Get2DArray(int n)
        {
            if (Array2DOffset == SampleArray2D.Count)
                return null;
            List<Point2F> point2Fs = SampleArray2D[Array2DOffset++];
            return point2Fs[CurrentPixelSampleIndex * n];
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