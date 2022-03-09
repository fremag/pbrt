using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelNoOp : Fresnel
    {
        public Spectrum Evaluate(float cosI) => new Spectrum(SampledSpectrum.NSpectralSamples, 1f);
    }
}