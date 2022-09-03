using System;
using System.Drawing;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class HelloWorldRenderer : AbstractRenderer
    {
        public override string FileName => "HelloWorld.png";
        public HelloWorldRenderer() : base("Hello world !", Brushes.White)
        {
            Camera = GetOrthoCam((0, 0, -6));
            Sampler = new PixelSampler(1, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount/2);
            Scene = new HelloWorldScene();
        }
    }
}