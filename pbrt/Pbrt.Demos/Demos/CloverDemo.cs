using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class CloverDemo : AbstractDemo
    {
        public override string FileName => "Clover.png";

        public CloverDemo() : base("Clover")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (0f, 2, -2f),
                    LookAt = (0, 2, 0)
                }
            };

            Scene = new CloverScene();
        }
    }
}