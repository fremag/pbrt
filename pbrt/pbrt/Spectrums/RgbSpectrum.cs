using System;

namespace pbrt.Spectrums
{
    public class RgbSpectrum : CoefficientSpectrum
    {
        public RgbSpectrum(float[] rgb) : base(rgb)
        {
        }

        public RgbSpectrum() : this(0f)
        {
        }
        
        public RgbSpectrum(float v = 0f) : base(3, v)
        {
        }

        public RgbSpectrum(CoefficientSpectrum cs) : base(cs)
        {
            if (cs.NSpectrumSamples != 3)
            {
                throw new ArgumentException($"{cs.NSpectrumSamples} != 3", nameof(cs.NSpectrumSamples));
            }
        }
        
        public float[] ToRgb() => new float[] { C[0], C[1], C[2] };

        public static RgbSpectrum FromRGB(float[] rgb, SpectrumType type = SpectrumType.Reflectance) => new RgbSpectrum(rgb); 
        
        public float[] ToXyz() => SpectrumUtils.RGBToXYZ(C); 
        
        public static RgbSpectrum FromXYZ(float[] xyz, SpectrumType type = SpectrumType.Reflectance) => new RgbSpectrum(SpectrumUtils.XYZToRGB(xyz));
        
        public static RgbSpectrum FromSampled(float[] lambdas, float[] values, int n) 
        {
            // Sort samples if unordered, use sorted for returned spectrum>> 
            float[] xyz = { 0, 0, 0 };
            for (int i = 0; i < XyzUtils.nCIESamples; ++i) 
            {
                float val = SpectrumUtils.InterpolateSpectrumSamples(lambdas, values, n, XyzUtils.CIE_lambda[i]);
                xyz[0] += val * XyzUtils.CIE_X[i];
                xyz[1] += val * XyzUtils.CIE_Y[i];
                xyz[2] += val * XyzUtils.CIE_Z[i];
            }
            float scale = (XyzUtils.CIE_lambda[XyzUtils.nCIESamples-1] - XyzUtils.CIE_lambda[0]) / (XyzUtils.CIE_Y_integral * XyzUtils.nCIESamples);
            xyz[0] *= scale;
            xyz[1] *= scale;
            xyz[2] *= scale;
            return FromXYZ(xyz);    
        }

        public void AddMul(float f, RgbSpectrum rgbSpectrum)
        {
            C[0] += f * rgbSpectrum.C[0];
            C[1] += f * rgbSpectrum.C[1];
            C[2] += f * rgbSpectrum.C[2];
        }
        
        public float Y() 
        {
            float yy = 0f;
            for (int i = 0; i < SampledSpectrum.NSpectralSamples; ++i)
            {
                yy += XyzUtils.Y.C[i] * C[i];
            }

            return yy * (SampledSpectrum.SampledLambdaEnd - SampledSpectrum.SampledLambdaStart) / (XyzUtils.CIE_Y_integral * SampledSpectrum.NSpectralSamples);
        }        
    }
}