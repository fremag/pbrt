using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;

namespace Pbrt.Demos.Demos
{
    public class HelloWorldDemo : AbstractDemo
    {
        public override string FileName => "HelloWorld.png";

        public HelloWorldDemo() : base("Hello world !")
        {
            CameraConfig = new CameraConfig
            {
                Camera = Configs.Camera.Orthographic,
                Config = new OrthographicCameraConfig
                {
                    Position = (0, 0, -1)
                }
            };
            Scene = new HelloWorldScene();
        }
    }
}