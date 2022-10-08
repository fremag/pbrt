using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class TorusDemo : AbstractDemo
    {
        public override string FileName => "Torus.png";

        public TorusDemo() : base("Torus")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { Position = (4f, 4, -4f) }
            };
            Scene = new TorusScene();
        }
    }
}