using pbrt.Cameras;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public class WhittedIntegratorConfig : AbstractIntegratorConfig
{
    public int MaxDepth { get; set; } = 5;

    public override Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera)
    {
        return new WhittedIntegrator(MaxDepth, sampler, camera, NbThreads, TileSize);
    }
}