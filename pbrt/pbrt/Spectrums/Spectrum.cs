namespace pbrt.Spectrums
{
    public class Spectrum : SampledSpectrum
//    public class Spectrum : RgbSpectrum
    {
        public Spectrum(int nSpectrumSamples, float v) : base(nSpectrumSamples, v)
        {
        }

        public Spectrum(CoefficientSpectrum cs) : base(cs)
        {
        }
    }
}