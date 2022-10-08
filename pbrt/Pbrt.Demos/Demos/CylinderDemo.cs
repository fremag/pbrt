using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class CylinderDemo : AbstractDemo
    {
        public override string FileName => "Cylinder.png";

        public CylinderDemo() : base("Cylinder")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-2f, 1, -2f),
                    LookAt = (0, 0, 1)
                }
            };
            Scene = new CylinderScene();
        }
    }
}