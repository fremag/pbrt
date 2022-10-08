using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class GlassDemo : AbstractDemo
    {
        public override string FileName => "Glass.png";

        public GlassDemo() : base("Glass")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-1f, 1, -1),
                    LookAt = (0, 0, 1)
                }
            };
            Scene = new GlassScene();
        }
    }
}