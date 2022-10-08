using pbrt.Films;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public abstract class AbstractSamplerConfig
{
    public int SamplesPerPixel { get; set; } = 1;
    public abstract AbstractSampler BuildSampler(Film film);
}