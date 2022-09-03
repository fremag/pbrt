using pbrt.Core;
using pbrt.Materials;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes
{
    public class DiskScene : DemoScene
    {
        public DiskScene()
        {
            Texture<float> sigma = MakeTexture(50);
            TextureMapping2D mapping = new PlanarMapping2D(VX, VZ, 1, 1);
            var kdChecker = new Checkerboard2DTexture<Spectrum>(mapping, MakeGraySpectrumTexture(0.5f), MakeGraySpectrumTexture(1));
            Disk(RotateX(90f), new MatteMaterial(kdChecker, sigma, null), 50);
            
            IMaterial mirrorMaterial = new MirrorMaterial(new ConstantTexture<Spectrum>(new Spectrum(1f)), null);
            Disk(RotateY(90) * RotateX(0)  * Translate(1f, 1f, 2f), mirrorMaterial, 0.75f);
           
            var uvTextureMaterial = new MatteMaterial(new UvTexture(new CylindricalMapping2D(RotateZ(45)* Translate(0f, -1f, 0f))), sigma, null);
            Disk(RotateY(0) * Translate(0f, 1f, 0f),  uvTextureMaterial);

            PointLight(10, 10, -10, 1000f);
        }
    }
}