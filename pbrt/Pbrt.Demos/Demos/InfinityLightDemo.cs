using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;

namespace Pbrt.Demos.Demos;

public abstract class AbstractInfinityLightDemo : AbstractDemo
{
    public override string FileName => $"InfinityLight_{LightName}.png";
    public string LightName { get; }

    public AbstractInfinityLightDemo(string lightName, float rotYDeg) : base($"InfinityLight {lightName}")
    {
        LightName = lightName;
            
        CameraConfig = new CameraConfig
        {
            Camera = Configs.Camera.Perspective,
            Config = new PerspectiveCameraConfig()
            {
                Position = (3, 2f,-3f),
                LookAt = (0, 0.5f, 0)
            }
        };
            
        SamplerConfig = new SamplerConfig
        {
            Sampler = Configs.Sampler.Halton,
            Config = new HaltonSamplerConfig { SamplesPerPixel = 1 << 5 }
        };

        IntegratorConfig = new IntegratorConfig
        {
            Integrator = IntegratorType.DirectLighting,
            Config = new DirectLightingConfig
            {
                Strategy = LightStrategy.UniformSampleAll
            }
        };
        Scene = new InfinityLightScene(lightName, rotYDeg);
    }
}

public class InfinityLightMorningDemo : AbstractInfinityLightDemo
{
    public InfinityLightMorningDemo() : base("skylight-morn", 0)
    {
    }
}

public class InfinityLightDayDemo : AbstractInfinityLightDemo
{
    public InfinityLightDayDemo() : base("skylight-day", 180)
    {
    }
}

public class InfinityLightSunsetDemo : AbstractInfinityLightDemo
{
    public InfinityLightSunsetDemo() : base("skylight-sunset", 180)
    {
    }
}