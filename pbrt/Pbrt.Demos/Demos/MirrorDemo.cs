using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class MirrorDemo : AbstractDemo
    {
        public override string FileName => "Mirror.png";

        public MirrorDemo() : base("Mirror")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig
                {
                    Position = (-1f, 1, -1),
                    LookAt = (0, 0, 1)
                }
            };
            Scene = new MirrorScene();
        }
    }
}