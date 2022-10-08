using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Lights;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Demos.renderers
{
    public class AreaLightRenderer : AbstractRenderer
    {
        public override string FileName => $"AreaLight_NbSample={SamplesPerPixel}.png";
        private int SamplesPerPixel { get; }
        public int NbThreads { get; set; }

        public AreaLightRenderer() : this(16, 1)
        {
        }

        public AreaLightRenderer(int samplesPerPixel = 16, int nbThreads = 1) : base($"Area light {nameof(SamplesPerPixel)}={samplesPerPixel}")
        {
            SamplesPerPixel = samplesPerPixel;
            NbThreads = nbThreads;

            CameraConfig = new CameraConfig()
            {
                Position = (-0f, 3, -2),
                LookAt = (0, 0, 1)
            };

            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Halton,
                Config = new HaltonSamplerConfig { SamplesPerPixel = SamplesPerPixel }
            };
            
            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.DirectLighting,
                Config = new DirectLightingConfig
                {
                    Strategy = LightStrategy.UniformSampleAll
                }
            };
            Scene = new AreaLightScene(1);
        }
    }

    public class AreaLightScene : DemoScene
    {
        public AreaLightScene(int nbSamples)
        {
            Floor();
            Sphere(0, 2f, 0, 0.5f, MatteMaterialGreen());

            var lightToWorld = Translate(0, 4, 0f) * RotateX(90);
            IShape shape = new Disk(lightToWorld, 0.20f);
            var light = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(400f), nbSamples, shape);
            AllLights.Add(light);
        }
    }
}