using System;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelDielectric : Fresnel
    {
        public float EtaI { get; }
        public float EtaT { get; }

        public FresnelDielectric(float etaI, float etaT)
        {
            EtaI = etaI;
            EtaT = etaT;
        }

        public Spectrum Evaluate(float cosI) => new Spectrum(SampledSpectrum.NSpectralSamples, BxDF.FrDielectric(MathF.Abs(cosI), EtaI, EtaT));
    }
}