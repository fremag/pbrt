using pbrt.Core;
using pbrt.Materials;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes
{
    public class CylinderScene : DemoScene
    {
        public CylinderScene()
        {
            IMaterial mirrorMaterial = new MirrorMaterial(MakeSpectrumTexture(1f), null);

            Texture<Spectrum> kr = MakeSpectrumTexture(0.6f);
            Texture<Spectrum> kt = MakeSpectrumTexture(1f);
            Texture<float> uRoughness = MakeTexture(0f);
            Texture<float> vRoughness = MakeTexture(0f);
            Texture<float> index = MakeTexture(3f);
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, null, false);

            Cylinder(Translate(-0.5f, 0f, 1f) * RotateX(-90), mirrorMaterial, zMax: 2);
            Cylinder(Translate(0f, 0f, -1f) * RotateX(-90), glassMaterial);
            Cylinder(Translate(0.5f, 0.5f, 0f) * RotateX(-90), MatteMaterialBlue());

            Floor();
            PointLight(10, 10, -10, 1000f);
            PointLight(-10, 20, 0, 1000f);
        }
    }
}