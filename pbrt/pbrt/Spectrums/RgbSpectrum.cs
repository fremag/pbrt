using System;

namespace pbrt.Spectrums
{
    public class RgbSpectrum : CoefficientSpectrum
    {
        public RgbSpectrum(float[] rgb) : base(rgb)
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
    }
}