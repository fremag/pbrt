using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class LensFocalSamplerTestDemo : AbstractDemo
    {
        public override string FileName => $"{nameof(LensFocalSamplerTestDemo)}.png";

        public LensFocalSamplerTestDemo() : base(nameof(LensFocalSamplerTestDemo))
        {
            var perspectiveCameraConfig = new PerspectiveCameraConfig
            {
                Position = (2f, 2f, -0.5f),
                LookAt = (0.5f, 0.5f, 2f),
                LensRadius = 0.05f,
                FocalDistance = 1.5f
            };
            CameraConfig = new CameraConfig
            {
                Config = perspectiveCameraConfig
            };
            SamplerConfig.Config.SamplesPerPixel = 32;
            Text = $"{nameof(LensFocalSamplerTestDemo)} lensRadius: {perspectiveCameraConfig.LensRadius} focalDistance: {perspectiveCameraConfig.FocalDistance} samplesPerPixel: {SamplerConfig.Config.SamplesPerPixel}";

            Scene = new SphereCubeScene(1, 1, 10);
        }
    }
}