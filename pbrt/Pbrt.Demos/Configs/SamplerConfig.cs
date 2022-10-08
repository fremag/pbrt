using pbrt.Films;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public enum Sampler {Pixel, Halton}
public class SamplerConfig
{
    private Sampler sampler;

    public Sampler Sampler
    {
        get => sampler;
        set
        {
            sampler = value;
            switch (sampler)
            {
                case Sampler.Halton:
                    Config = new HaltonSamplerConfig();
                    break;
                case Sampler.Pixel:
                    Config = new PixelSamplerConfig();
                    break;
            }
        }
    }

    public AbstractSamplerConfig Config { get; set; }
}

public abstract class AbstractSamplerConfig
{
    public int SamplesPerPixel { get; set; } = 1;
    public abstract AbstractSampler BuildSampler(Film film);
}

public class PixelSamplerConfig : AbstractSamplerConfig
{
    public int SampledDimensions { get; set; } = 1;
    public int Seed { get; set; } = 0;
    
    public override AbstractSampler BuildSampler(Film film)
    {
        return new PixelSampler(SamplesPerPixel, SampledDimensions, Seed);
    }

}

public class HaltonSamplerConfig : AbstractSamplerConfig
{
    public override AbstractSampler BuildSampler(Film film)
    {
        return new HaltonSampler(SamplesPerPixel, film.GetSampleBounds());
    }
}