using pbrt.Core;
using pbrt.Materials;

namespace Pbrt.Demos.Scenes
{
    public class MirrorScene : DemoScene
    {
        public MirrorScene()
        {
            IMaterial mirrorMaterial = new MirrorMaterial(MakeSpectrumTexture(1f), null);
            
            Sphere(-0.5f, 0f, 0f, 0.5f, PlasticMaterialRed);
            Sphere(0.5f, 0.5f, 0f, 0.5f, mirrorMaterial);
            Floor();
            PointLight(10, 10, -10, 1000f);
        }
    }
}