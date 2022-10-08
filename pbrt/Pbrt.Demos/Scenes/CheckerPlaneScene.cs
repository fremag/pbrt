namespace Pbrt.Demos.Scenes
{
    public class CheckerPlaneScene : DemoScene
    {
        public CheckerPlaneScene()
        {
            Sphere(-0.5f, 0f, 0f, 0.5f, PlasticMaterialRed);
            Sphere(0.5f, 0.5f, 0f, 0.5f, MatteMaterialBlue());
            Floor();

            PointLight(10, 10, -10, 1000f);
        }
    }
}