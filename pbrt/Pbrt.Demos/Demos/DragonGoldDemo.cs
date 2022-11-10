using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;

namespace Pbrt.Demos.Demos
{
    public class DragonGoldDemo : AbstractDemo
    {
        public override string FileName => "DragonGold.png";

        public DragonGoldDemo() : base("DragonGold")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { 
                    Position = (0f, 10, -12f), 
                    Width = 1280,
                    Height = 1024}
            };
            
            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Halton,
                Config = new HaltonSamplerConfig { SamplesPerPixel = 16 }
            };

            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.DirectLighting,
                Config = new DirectLightingConfig
                {
                    Strategy = LightStrategy.UniformSampleAll
                }
            };
            
            Scene = new DragonGoldScene();
        }
    }
}