using System.Collections.Generic;
using pbrt.Spectrums;

namespace pbrt.Core
{
    public class Distribution1D
    {
        public List<float> Func { get; } // piecewise-constant function
        public List<float> Cdf { get; } // cumulative distribution function 
        public float FuncInt { get; } // func integral [0, 1]
        public float Count => Func.Count;
        
        public Distribution1D(float[] f)
        {
            int n = f.Length;
            Func = new List<float>(f);
            Cdf = new List<float>();
            
            // Compute integral of step function at xi
            Cdf.Add(0);
            for (int i = 1; i < n + 1; ++i)
            {
                var cdfValue = Cdf[i - 1] + Func[i - 1] / n;
                Cdf.Add( cdfValue);
            }

            // Transform step function integral into CDF 
            FuncInt = Cdf[n];
            if (FuncInt == 0) 
            {
                for (int i = 1; i < n + 1; ++i)
                {
                    Cdf[i] = ((float)i) / n;
                }
            }
            else 
            {
                for (int i = 1; i < n + 1; ++i)
                {
                    Cdf[i] /= FuncInt;
                }
            }            
        }
        
        public float SampleContinuous(float u, out float pdf, out int offset) 
        {
            //Find surrounding CDF segments and offset>> 
            offset = SpectrumUtils.FindInterval(Cdf.Count, i =>  Cdf[i] <= u);
            
            // Compute offset along CDF segment 
            float du = u - Cdf[offset];
            if ((Cdf[offset + 1] - Cdf[offset]) > 0)
            {
                du /= (Cdf[offset + 1] - Cdf[offset]);
            }

            // Compute PDF for sampled offset 
            pdf = Func[offset] / FuncInt;
            
            // Return x in  [0, 1) corresponding to sample
            return (offset + du) / Count;
        }
        
        public int SampleDiscrete(float u, out float pdf, out float uRemapped) 
        {
            // Find surrounding CDF segments and offset>>
            var offset = SpectrumUtils.FindInterval(Cdf.Count, i =>  Cdf[i] <= u);

            pdf = Func[offset] / (FuncInt * Count);
            uRemapped = (u - Cdf[offset]) / (Cdf[offset + 1] - Cdf[offset]);
            return offset;
        }
        
        public float DiscretePDF(int index) 
        {
            return Func[index] / (FuncInt * Count);
        }        
    }
}