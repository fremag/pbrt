using System;
using System.Drawing;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class TriangleRenderer : AbstractRenderer
    {
        public override string FileName => "Triangle.png";

        public TriangleRenderer() : base("Triangle", Brushes.White)
        {
            Camera = GetCam((0f, 3, -3f), (0, 0, 0));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount );
            Scene = new TriangleScene();
        }
    }
}