using pbrt.Cameras;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public class DirectLightingConfig : AbstractIntegratorConfig
{
    public LightStrategy Strategy { get; set; }
    public int MaxDepth { get; set; } = 5;

    public override Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera)
    {
        return new DirectLightingIntegrator(sampler, camera, Strategy, MaxDepth, NbThreads, TileSize);
    }
}