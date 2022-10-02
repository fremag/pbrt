using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Lights;
using pbrt.Samplers;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Demos.renderers
{
    public class AreaLightRenderer : AbstractRenderer
    {
        public override string FileName => $"AreaLight_NbSample={SamplesPerPixel}.png";
        private int SamplesPerPixel { get; }

        public AreaLightRenderer() : this(16, 1)
        {
            
        }
        
        public AreaLightRenderer(int samplesPerPixel=16,int nbThreads=1) : base($"Area light {nameof(SamplesPerPixel)}={samplesPerPixel}")
        {
            SamplesPerPixel = samplesPerPixel;
            Camera = GetCam((-0f, 3, -2), (0, 0, 1));
            //Sampler = new PixelSampler(samplesPerPixel, 1);
            // int n = (int)MathF.Sqrt(samplesPerPixel);
            // Sampler = new StratifiedSampler(n, n, true, 1);
            Sampler = new HaltonSampler(samplesPerPixel, Camera.Film.GetSampleBounds());
            Scene = new AreaLightScene(1);
            Scene.Init();

            var integrator = new DirectLightingIntegrator(Sampler, Camera, LightStrategy.UniformSampleAll, 5)
            {
               NbThreads = nbThreads
            };
            
            integrator.Preprocess(Scene, Sampler);
            Integrator = integrator;
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