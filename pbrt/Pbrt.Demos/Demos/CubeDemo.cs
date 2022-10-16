using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class CubeDemo : AbstractDemo
    {
        public override string FileName => "Cube.png";

        public CubeDemo() : base("Cube")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { Position = (3f, 3, -3f) }
            };
            Scene = new CubeScene();
        }
    }
}