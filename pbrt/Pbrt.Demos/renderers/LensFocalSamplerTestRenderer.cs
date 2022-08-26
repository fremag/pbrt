using System;
using System.Drawing;
using Pbrt.Demos.scenes;
using pbrt.Integrators;
using pbrt.Samplers;

namespace Pbrt.Demos.renderers
{
    public class LensFocalSamplerTestRenderer : AbstractRenderer
    {
        public override string FileName => $"{nameof(LensFocalSamplerTestRenderer)}.png";
        
        public LensFocalSamplerTestRenderer() : base(nameof(LensFocalSamplerTestRenderer), Brushes.White)
        {
            int samplesPerPixel = 32;
            var lensRadius = 0.05f;
            var focalDistance = 1.5f;

            Text = $"{nameof(LensFocalSamplerTestRenderer)} lensRadius: {lensRadius} focalDistance: {focalDistance} samplesPerPixel: {samplesPerPixel}";
            Camera = GetCam((2f, 2f, -0.5f), (0.5f, 0.5f, 2f), lensRadius: lensRadius, focalDistance: focalDistance);
            Sampler = new PixelSampler(samplesPerPixel, 1);

            var singleCore = false;
            Integrator = new WhittedIntegrator(5, Sampler, Camera, singleCore ? 1 : Environment.ProcessorCount);
            Scene = new SphereCubeScene(1,1,10);
        }
    }
}