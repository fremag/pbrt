using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class TriangleDemo : AbstractDemo
    {
        public override string FileName => "Triangle.png";

        public TriangleDemo() : base("Triangle")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { Position = (0f, 3, -3f) }
            };
            Scene = new TriangleScene();
        }
    }
}