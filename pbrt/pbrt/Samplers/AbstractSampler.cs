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
        public List<int> Samples1DArraySizes { get; }  = new List<int>();
        public List<int> Samples2DArraySizes { get; } = new List<int>();
        public List<float[]> SampleArray1D { get; } = new List<float[]>();
        public List<Point2F[]> SampleArray2D { get; } = new List<Point2F[]>();
        
        public int Array1DOffset { get; private set; }
        public int Array2DOffset { get; private set; }

        protected AbstractSampler() : this(1)
        {
        }

        public AbstractSampler(int samplesPerPixel)
        {
            SamplesPerPixel = samplesPerPixel;
        }

        public void StartPixel(Point2I p)
        {
            CurrentPixel = p;
            CurrentPixelSampleIndex = 0;
            
            // Reset array offsets for next pixel sample 
            Array1DOffset = 0;
            Array2DOffset = 0;            
        }
        public abstract float Get1D();
        public abstract Point2F Get2D();
        public abstract AbstractSampler Clone(int seed);

        public CameraSample GetCameraSample(Point2I pRaster)
        {
            var pFilm = Get2D();
            var time = Get1D();
            var pLens = Get2D();

            var cs = new CameraSample
            {
                PFilm = (Point2F)pRaster + pFilm,
                Time = time,
                PLens = pLens
            };

            return cs;
        }

        public void Request1DArray(int n)
        {
            Samples1DArraySizes.Add(n);
            SampleArray1D.Add(new float[n * SamplesPerPixel]);            
        }

        public void Request2DArray(int n)
        {
            Samples2DArraySizes.Add(n);
            SampleArray2D.Add(new Point2F[n * SamplesPerPixel]);            
        }
        
        public virtual int RoundCount(int n)
        {
            return n;
        }

        public float[] Get1DArray(int n)
        {
            if (Array1DOffset == SampleArray1D.Count)
            {
                return null;
            }

            var floats = SampleArray1D[Array1DOffset++];
            return floats;
        }

        public Point2F[] Get2DArray(int n)
        {
            if (Array2DOffset == SampleArray2D.Count)
            {
                return null;
            }

            var points = SampleArray2D[Array2DOffset++];
            return points;
        }
        
        public virtual bool StartNextSample()
        {
            Array1DOffset = 0;
            Array2DOffset = 0;
            return ++CurrentPixelSampleIndex < SamplesPerPixel;
        }
        
        public virtual bool SetSampleNumber(int sampleNum)
        {
            // Reset array offsets for next pixel sample 
            Array1DOffset = 0;
            Array2DOffset = 0;

            CurrentPixelSampleIndex = sampleNum;
            return CurrentPixelSampleIndex < SamplesPerPixel;
        }
    }
}