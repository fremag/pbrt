using System;
using System.Drawing;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class CloverRenderer : AbstractRenderer
    {
        public override string FileName => "Clover.png";

        public CloverRenderer() : base("Clover", Brushes.White)
        {
            Camera = GetCam((0f, 2, -2f), (0, 2, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new CloverScene();
        }
    }
}