using System;
using pbrt.Core;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Media
{
    public class HomogeneousMedium : Medium
    {
        public static HomogeneousMedium Default() => new HomogeneousMedium(new Spectrum(0f), new Spectrum(0f), 1f);
        public Spectrum SigmaS { get; }
        public Spectrum SigmaA { get; }
        public Spectrum SigmaT { get; set; }
        public float G { get; }

        public HomogeneousMedium(Spectrum sigma_a, Spectrum sigma_s, float g)
        {
            SigmaS = sigma_s;
            SigmaA = sigma_a;
            G = g;
            SigmaT = (sigma_s + sigma_a);
        }
        
        public override Spectrum Tr(Ray ray, ISampler sampler)
        {
            var min = MathF.Min(ray.TMax * ray.D.Length, float.MaxValue);
            var spectrum = -SigmaT * min;
            spectrum.Exp();
            return spectrum;
        }        
    }
}