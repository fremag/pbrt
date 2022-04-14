using System;
using System.Drawing;
using pbrt.Core;
using Pbrt.Demos.scenes;
using pbrt.Filters;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class DiskRenderer : AbstractRenderer
    {
        public override string FileName => "Disk.png";
        public DiskRenderer() : base("Disk", Brushes.White)
        {
            Camera = GetCam((-2f, 2, -2f), (0, 1, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount/2);
            Scene = new DiskScene();
        }
    }
}