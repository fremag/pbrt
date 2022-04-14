using System.Collections.Generic;
using pbrt.Accelerators;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.scenes
{
    public class DiskScene : Scene
    {
        public DiskScene()
        {
            Texture<float> sigma = new ConstantTexture<float>(50);
            Texture<Spectrum> kdRed = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 0, 0 })));
            Texture<Spectrum> kdGreen = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 1, 0 })));
            Texture<Spectrum> kdBlue = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 0, 1 })));
            Texture<Spectrum> kdBlack = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 0, 0 })));
            Texture<Spectrum> kdWhite = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 1, 1 })));
            Texture<Spectrum> kdGray = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0.5f, 0.5f, 0.5f })));
            TextureMapping2D mapping = new PlanarMapping2D(new Vector3F(1, 0, 0), new Vector3F(0, 0, 1), 1, 1);
            var kdChecker = new Checkerboard2DTexture<Spectrum>(mapping, kdGray, kdWhite);
            var uvTexture = new UvTexture(new UVMapping2D(0.5f, 0.5f, 0.5f, 0.5f));
            Texture<Spectrum> ks = new ConstantTexture<Spectrum>(new Spectrum(50f));
            IMaterial materialRed = new PlasticMaterial(kdRed, ks, new ConstantTexture<float>(0.001f), null, false);
            IMaterial materialGreen = new MatteMaterial(kdGreen, sigma, null);
            IMaterial materialBlue = new MatteMaterial(kdBlue, sigma, null);
            
            IMaterial mirrorMaterial = new MirrorMaterial(new ConstantTexture<Spectrum>(new Spectrum(1f)), null);

            Texture<Spectrum> kr = new ConstantTexture<Spectrum>(new Spectrum(0.6f));;
            Texture<Spectrum> kt = new ConstantTexture<Spectrum>(new Spectrum(1f));
            Texture<float> uRoughness = new ConstantTexture<float>(0f);
            Texture<float> vRoughness = new ConstantTexture<float>(0f);
            Texture<float> index = new ConstantTexture<float>(3f);
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, null, false);

            MediumInterface mediumInterface = new MediumInterface(HomogeneousMedium.Default());

            var disk1 = new Disk((Transform.RotateY(90) * Transform.RotateX(0) ) * Transform.Translate(1f, 1f, 2f), 0.75f);
            var diskPrimitive1 = new GeometricPrimitive(disk1, mirrorMaterial, null, mediumInterface);

            var disk2 = new Disk(Transform.RotateX(90f), 50);
            var diskPrimitive2 = new GeometricPrimitive(disk2, new MatteMaterial(kdChecker, sigma, null), null, mediumInterface);
           
            var disk3 = new Disk(Transform.RotateY(0) * Transform.Translate(0f, 1f, 0f) );
            var uvTextureMaterial = new MatteMaterial(new UvTexture(new CylindricalMapping2D(Transform.RotateZ(45)* Transform.Translate(0f, -1f, 0f))), sigma, null);
            var diskPrimitive3 = new GeometricPrimitive(disk3, uvTextureMaterial, null, mediumInterface);
            
            BvhAccel bvh = new BvhAccel(new List<IPrimitive>
            {
                diskPrimitive1,
                diskPrimitive2,
                diskPrimitive3 
            }, 5, SplitMethod.Middle);

            MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(10, 10, -10), medium, new Spectrum(1000f)),
            };

            Init(bvh, lights);
        }
    }
}