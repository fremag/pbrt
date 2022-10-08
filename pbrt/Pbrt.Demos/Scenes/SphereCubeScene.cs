namespace Pbrt.Demos.Scenes
{
    public class SphereCubeScene : DemoScene
    {
        public SphereCubeScene(int maxI = 5, int maxJ = 5, int maxK = 5, float radius = 0.5f)
        {
            Floor();
            var materialGreen = MatteMaterialGreen();

            for (int i = 0; i < maxI; i++)
            {
                for (int j = 0; j < maxJ; j++)
                {
                    for (int k = 0; k < maxK; k++)
                    {
                        Sphere(i, j + radius, k, radius, materialGreen);
                    }
                }
            }

            PointLight(10, 10, -10, 1000f);
        }
    }
}