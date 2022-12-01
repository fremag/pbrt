using pbrt.Core;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Media
{
    public abstract class Medium
    {
        public abstract Spectrum Tr(Ray ray, ISampler sampler);
    }
}