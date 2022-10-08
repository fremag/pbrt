using System;
using pbrt.Cameras;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public abstract class AbstractIntegratorConfig
{
    public int NbThreads { get; set; } = Environment.ProcessorCount - 1;
    public int TileSize { get; set; } = 16;

    public abstract Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera);
}