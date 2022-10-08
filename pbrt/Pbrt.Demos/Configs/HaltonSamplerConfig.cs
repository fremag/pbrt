using pbrt.Films;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public class HaltonSamplerConfig : AbstractSamplerConfig
{
    public override AbstractSampler BuildSampler(Film film)
    {
        return new HaltonSampler(SamplesPerPixel, film.GetSampleBounds());
    }
}