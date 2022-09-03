using System;
using System.Drawing;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class CheckerPlaneRenderer : AbstractRenderer
    {
        public override string FileName => "CheckerPlane.png";
        public CheckerPlaneRenderer() : base("Checker plane", Brushes.White)
        {
            Camera = GetCam((-1f, 1, -1), (0, 0, 1));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount/2);
            Scene = new CheckerPlaneScene();
        }
    }
}