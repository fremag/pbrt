using System;
using System.Drawing;
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
        public AreaLightRenderer(int samplesPerPixel=16) : base($"Area light {nameof(SamplesPerPixel)}={samplesPerPixel}", Brushes.White)
        {
            SamplesPerPixel = samplesPerPixel;
            Camera = GetCam((-0f, 3, -2), (0, 0, 1));
            Sampler = new PixelSampler(samplesPerPixel, 1);
            //Sampler = new StratifiedSampler(2, 2, true, 1);
            Scene = new AreaLightScene(SamplesPerPixel);
            Scene.Init();

            var integrator = new DirectLightingIntegrator(Sampler, Camera, LightStrategy.UniformSampleAll, 5)
            {
                NbThreads = Environment.ProcessorCount
//               NbThreads = 1
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