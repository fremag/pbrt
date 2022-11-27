using pbrt.Cameras;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public class PathIntegratorConfig : AbstractIntegratorConfig
{
    public int MaxDepth { get; set; } = 5;
    public override Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera)
    {
        return new PathIntegrator(MaxDepth, sampler, camera, NbThreads, TileSize);
    }
}