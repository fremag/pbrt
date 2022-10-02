using System;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class GlassRenderer : AbstractRenderer
    {
        public override string FileName => "Glass.png";
        public GlassRenderer() : base("Glass")
        {
            Camera = GetCam((-1f, 1, -1), (0, 0, 1));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera,Environment.ProcessorCount/2);
            Scene = new GlassScene();
        }
    }
}