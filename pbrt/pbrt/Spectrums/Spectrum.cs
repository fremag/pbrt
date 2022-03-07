namespace pbrt.Spectrums
{
    public class Spectrum : SampledSpectrum
//    public class Spectrum : RgbSpectrum
    {
        public Spectrum(int nSpectrumSamples, float v) : base(nSpectrumSamples, v)
        {
        }

        public Spectrum(float[] values) : base(values)
        {
            
        }

        public Spectrum(CoefficientSpectrum cs) : base(cs)
        {
        }

        public static Spectrum FromSampledSpectrum(float[] lambdas, float[] values, int n) => new Spectrum(FromSampled(lambdas, values, n));

        public static Spectrum Lerp(float t, Spectrum spec1, Spectrum spec2)
        {
            var cs = new Spectrum(spec1);
            cs.Mul(1 - t);
            var csBis = new Spectrum(spec2);
            csBis.Mul(t);
            cs.Add(csBis);
            return cs;
        }

        public static Spectrum operator +(Spectrum spec1, Spectrum spec2)
        {
            var spec = new Spectrum(spec1);
            spec.Add(spec2);
            return spec;
        }
        
        public static Spectrum operator -(Spectrum spec1, Spectrum spec2)
        {
            var spec = new Spectrum(spec1);
            spec.Sub(spec2);
            return spec;
        }
        
        public static Spectrum operator *(Spectrum spec1, Spectrum spec2)
        {
            var spec = new Spectrum(spec1);
            spec.Mul(spec2);
            return spec;
        }
        
        public static Spectrum operator /(Spectrum spec1, Spectrum spec2)
        {
            var spec = new Spectrum(spec1);
            spec.Div(spec2);
            return spec;
        }
        
        public static Spectrum operator /(float a, Spectrum spec1)
        {
            var spec = new Spectrum(spec1.NSpectrumSamples, a);
            spec.Div(spec1);
            return spec;
        }
        
        public static Spectrum operator /(Spectrum spec1, float a)
        {
            var spec = new Spectrum(spec1);
            spec.Div(a);
            return spec;
        }
        
        public static Spectrum operator *(float a, Spectrum spec1)
        {
            var spec = new Spectrum(spec1);
            spec.Mul(a);
            return spec;
        }

        public static Spectrum operator *(Spectrum spec1, float a) => a * spec1;
        public static Spectrum operator -(Spectrum spec) => -1 * spec;
    }
}