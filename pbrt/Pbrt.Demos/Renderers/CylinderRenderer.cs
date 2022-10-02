using System;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class CylinderRenderer : AbstractRenderer
    {
        public override string FileName => "Cylinder.png";
        public CylinderRenderer() : base("Cylinder")
        {
            Camera = GetCam((-2f, 1, -2f), (0, 0, 1));
            Sampler = new PixelSampler(2, 1, 0);
            Integrator = new WhittedIntegrator(5, Sampler, Camera, Environment.ProcessorCount/2);
            Scene = new CylinderScene();
        }
    }
}