using System;
using pbrt.Cameras;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.Configs;

public enum IntegratorType
{
    DirectLighting,
    Whitted
};

public class IntegratorConfig
{
    private IntegratorType integratorType;
    public IntegratorType Integrator
    {
        get => integratorType;
        set
        {
            integratorType = value;
            switch (value)
            {
                case IntegratorType.Whitted:
                    Config = new WhittedIntegratorConfig(); 
                    break;
                case IntegratorType.DirectLighting:
                    Config = new DirectLightingConfig();
                    break;
            }
        }
    }

    public AbstractIntegratorConfig Config {
        get;
        set;
    }

}

public class DirectLightingConfig : AbstractIntegratorConfig
{
    public LightStrategy Strategy { get; set; }
    public int MaxDepth { get; set; } = 5;
    
    public override Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera)
    {
        return new DirectLightingIntegrator(sampler, camera, Strategy, MaxDepth,  NbThreads, TileSize);
    }
}

public abstract class AbstractIntegratorConfig
{
    public int NbThreads { get; set; } = Environment.ProcessorCount-1;
    public int TileSize { get; set; } = 16;
    
    public abstract Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera);
}

public class WhittedIntegratorConfig : AbstractIntegratorConfig
{
    public int MaxDepth { get; set; } = 5;
    public override Integrator BuildIntegrator(AbstractSampler sampler, AbstractCamera camera)
    {
        return new WhittedIntegrator(MaxDepth, sampler, camera);
    }
}