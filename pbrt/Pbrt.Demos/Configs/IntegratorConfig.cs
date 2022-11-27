namespace Pbrt.Demos.Configs;

public enum IntegratorType
{
    DirectLighting,
    Whitted,
    Path
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
                case IntegratorType.Path:
                    Config = new PathIntegratorConfig();
                    break;
            }
        }
    }

    public AbstractIntegratorConfig Config { get; set; }
}