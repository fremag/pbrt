using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class DiskDemo : AbstractDemo
    {
        public override string FileName => "Disk.png";

        public DiskDemo() : base("Disk")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-2f, 2, -2f),
                    LookAt = (0, 1, 0)
                }
            };
            Scene = new DiskScene();
        }
    }
}