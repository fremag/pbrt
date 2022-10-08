namespace Pbrt.Demos.Configs;

public enum Sampler
{
    Pixel,
    Halton
}

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