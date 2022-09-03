using System;
using System.Drawing;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class TorusRenderer : AbstractRenderer
    {
        public override string FileName => "Torus.png";

        public TorusRenderer() : base("Torus", Brushes.White)
        {
            Camera = GetCam((4f, 4, -4f), (0, 0, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new TorusScene();
        }
    }
}