using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class CheckerPlaneDemo : AbstractDemo
    {
        public override string FileName => "CheckerPlane.png";

        public CheckerPlaneDemo() : base("Checker plane")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-1f, 1, -1),
                    LookAt = (0, 0, 1)
                }
            };
            Scene = new CheckerPlaneScene();
        }
    }
}