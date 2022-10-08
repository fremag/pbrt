using pbrt.Films;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public class PixelSamplerConfig : AbstractSamplerConfig
{
    public int SampledDimensions { get; set; } = 1;
    public int Seed { get; set; } = 0;

    public override AbstractSampler BuildSampler(Film film)
    {
        return new PixelSampler(SamplesPerPixel, SampledDimensions, Seed);
    }
}