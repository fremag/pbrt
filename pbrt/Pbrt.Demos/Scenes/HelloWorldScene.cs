namespace Pbrt.Demos.Scenes
{
    public class HelloWorldScene : DemoScene
    {
        public HelloWorldScene()
        {
            Sphere(0, 0, 0, 0.5f, MatteMaterialGreen());
            PointLight(10, 10, -10, 1000f);
        }
    }
}