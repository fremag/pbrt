using System;
using System.Threading;
using pbrt.Cameras;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos
{
    public abstract class AbstractDemo
    {
        public CameraConfig CameraConfig { get; set; }
        public SamplerConfig SamplerConfig { get; set; }
        public IntegratorConfig IntegratorConfig { get; set; }

        public AbstractCamera Camera { get; protected set; }
        public AbstractSampler Sampler { get; protected set; }
        public Integrator Integrator { get; protected set; }
        public DemoScene Scene { get; protected set; }

        public abstract string FileName { get; }
        public string Text { get; protected set; }

        protected AbstractDemo(string text) : this()
        {
            Text = text;
        }

        protected AbstractDemo()
        {
            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Pixel,
                Config = new PixelSamplerConfig
                {
                    Seed = 0,
                    SampledDimensions = 1,
                    SamplesPerPixel = 1
                }
            };

            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.Whitted,
                Config = new WhittedIntegratorConfig
                {
                    MaxDepth = 5,
                    NbThreads = Environment.ProcessorCount
                }
            };
        }

        public void Init(Action<int, int, TimeSpan> onTileRendered = null)
        {
            if (CameraConfig != null)
            {
                Camera = CameraConfig.Config.BuildCamera();
            }

            if (SamplerConfig != null)
            {
                Sampler = SamplerConfig.Config.BuildSampler(Camera.Film);
            }

            Scene.Init();

            if (IntegratorConfig != null)
            {
                Integrator = IntegratorConfig.Config.BuildIntegrator(Sampler, Camera);
            }

            Integrator.Preprocess(Scene, Sampler);
            if (onTileRendered != null)
            {
                Integrator.TileRendered += onTileRendered;
            }
        }

        public float[] Render(CancellationToken cancellationToken)
        {
            return Integrator.Render(Scene, cancellationToken, cancellationToken);
        }
    }
}