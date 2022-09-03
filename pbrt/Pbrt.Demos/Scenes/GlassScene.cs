using pbrt.Core;
using pbrt.Materials;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes
{
    public class GlassScene : DemoScene
    {
        public GlassScene()
        {
            Texture<Spectrum> kr = MakeSpectrumTexture(0.6f);
            Texture<Spectrum> kt = MakeSpectrumTexture(1f);
            Texture<float> uRoughness = MakeTexture(0f);
            Texture<float> vRoughness = MakeTexture(0f);
            Texture<float> index = MakeTexture(3f);
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, null, false);

            Sphere(-0.5f, 0f, 0f, 0.5f, PlasticMaterialRed);
            Sphere(0.5f, 0.5f, 0f, 0.5f, glassMaterial);
            Floor();
            PointLight(10, 10, -10, 1000f);
        }
    }
}