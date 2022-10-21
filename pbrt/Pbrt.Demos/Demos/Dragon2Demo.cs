using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class Dragon2Demo : AbstractDemo
    {
        public override string FileName => "Dragon2.png";

        public Dragon2Demo() : base("Dragon2")
        {
            CameraConfig = new CameraConfig
            {
                Config = new PerspectiveCameraConfig { 
                    Position = (0f, 10, -12f), 
                    Width = 1280,
                    Height = 1024}
            };
            Scene = new Dragon2Scene();
        }
    }
}