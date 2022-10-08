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

    public AbstractIntegratorConfig Config { get; set; }
}