using System;
using System.Collections.Generic;
using pbrt.Core;

namespace pbrt.Spectrums
{
    public class SampledSpectrum : CoefficientSpectrum
    {
        public static readonly int SampledLambdaStart = 400;
        public static readonly int SampledLambdaEnd = 700;
        public static readonly int NSpectralSamples = 60;
        
        public SampledSpectrum(int nSpectrumSamples, float v=0) : base(nSpectrumSamples, v)
        {
        }

        public SampledSpectrum(CoefficientSpectrum cs) : base(cs)
        {
        }

        public override CoefficientSpectrum Copy() => new SampledSpectrum(this);
        
        public static bool SpectrumSamplesSorted(float[] lambdas, int n) {
            for (int i = n - 1 - 1; i >= 0; --i)
            {
                if (lambdas[i] > lambdas[i + 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static void SortSpectrumSamples(float[] lambdas, float[] values, int n, out float[] sortedLambdas, out float[] sortedValues)
        {
            SortedList<float, float> sorted = new SortedList<float, float>();
            for (int i = 0; i < n; i++)
            {
                var lambda = lambdas[i];
                var value = values[i];
                sorted[lambda] = value;
            }

            sortedLambdas = new float[n];
            sortedValues = new float[n];
            
            for (int i = 0; i < n; i++)
            {
                lambdas[i] = sorted.Keys[i];
                values[i] = sorted.Values[i];
            }
        }
        
        public static SampledSpectrum FromSampled(float[] lambdas, float[] values, int n) 
        {
            if (!SpectrumSamplesSorted(lambdas, n)) 
            {
                SortSpectrumSamples(lambdas, values, n, out var sortedLambdas, out var sortedValues);
                return FromSampled(sortedLambdas, sortedValues, n);
            }        

            SampledSpectrum r = new SampledSpectrum(NSpectralSamples);
            for (int i = 0; i < NSpectralSamples; ++i) 
            {
              // Compute average value of given SPD over ith sample’s range
              float lambda0 = MathUtils.Lerp(((float)i) / NSpectralSamples, SampledLambdaStart, SampledLambdaEnd);
              float lambda1 = MathUtils.Lerp((float)(i + 1) / NSpectralSamples, SampledLambdaStart, SampledLambdaEnd);
              r.C[i] = AverageSpectrumSamples(lambdas, values, n, lambda0, lambda1);              
            }
            return r;
        }

        public static float AverageSpectrumSamples(float[] lambdas, float[] values, int n, float lambdaStart, float lambdaEnd)
        {
            // Handle cases with out-of-bounds range or single sample only
            if (lambdaEnd   <= lambdas[0])
            {
                return values[0];
            }

            if (lambdaStart >= lambdas[n - 1])
            {
                return values[n - 1];
            }
            if (n == 1)
            {
                return values[0];
            }

            float sum = 0;  
            // Add contributions of constant segments before/after samples
            if (lambdaStart < lambdas[0])
            {
                sum += values[0] * (lambdas[0] - lambdaStart);
            }

            if (lambdaEnd > lambdas[n-1])
            {
                sum += values[n - 1] * (lambdaEnd - lambdas[n - 1]);
            }
            // Advance to first relevant wavelength segment 
            int i = 0;
            while (lambdaStart > lambdas[i + 1])
            {
                ++i;
            }
            
            // Loop over wavelength sample segments and add contributions
            float Interp (float w, int j) {
                return MathUtils.Lerp((w - lambdas[j]) / (lambdas[j + 1] - lambdas[j]), values[j], values[j + 1]);
            };
            
            for (; i+1 < n && lambdaEnd >= lambdas[i]; i++) 
            {
                float segLambdaStart = MathF.Max(lambdaStart, lambdas[i]);
                float segLambdaEnd =   MathF.Min(lambdaEnd,   lambdas[i + 1]);
                sum += 0.5f * (Interp(segLambdaStart, i) + Interp(segLambdaEnd, i)) * (segLambdaEnd - segLambdaStart);
            }                
            return sum / (lambdaEnd - lambdaStart);
        }        
    }
}