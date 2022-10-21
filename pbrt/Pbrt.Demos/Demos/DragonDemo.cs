using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class DragonDemo : AbstractDemo
    {
        public override string FileName => "Dragon.png";

        public DragonDemo() : base("Dragon")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { 
                    Position = (0f, 10, -12f), 
                    Width = 1280,
                    Height = 1024}
            };
            Scene = new DragonScene();
        }
    }
}