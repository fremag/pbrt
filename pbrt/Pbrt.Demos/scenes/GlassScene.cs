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
    public class GlassScene : Scene
    {
        public GlassScene()
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
            Texture<Spectrum> ks = new ConstantTexture<Spectrum>(new Spectrum(50f));
            IMaterial materialRed = new PlasticMaterial(kdRed, ks, new ConstantTexture<float>(0.001f), null, false);
            IMaterial materialGreen = new MatteMaterial(kdGreen, sigma, null);

            Texture<Spectrum> kr = new ConstantTexture<Spectrum>(new Spectrum(0.6f));;
            Texture<Spectrum> kt = new ConstantTexture<Spectrum>(new Spectrum(1f));
            Texture<float> uRoughness = new ConstantTexture<float>(0f);
            Texture<float> vRoughness = new ConstantTexture<float>(0f);
            Texture<float> index = new ConstantTexture<float>(3f);
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, null, false);
            
            MediumInterface mediumInterface = new MediumInterface(HomogeneousMedium.Default());

            var sphere1 = new Sphere(Transform.Translate(-0.5f, 0f, 0f), 0.5f);
            var spherePrimitive1 = new GeometricPrimitive(sphere1, materialRed, null, mediumInterface);

            var sphere2 = new Sphere(Transform.Translate(0.5f, 0.5f, 0f), 0.5f);
            var glassSphere = new GeometricPrimitive(sphere2, glassMaterial, null, mediumInterface);
           
            var planeTransform = Transform.Scale(100f, 0.1f, 100f);
            var plane = new Sphere(planeTransform, 1f);
            var planePrimitive = new GeometricPrimitive(plane, new MatteMaterial(kdChecker, sigma, null), null, mediumInterface);
            
            BvhAccel bvh = new BvhAccel(new List<IPrimitive>
            {
                planePrimitive, spherePrimitive1, glassSphere 
            }, 5, SplitMethod.Middle);

            MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(10, 10, -10), medium, new Spectrum(1000f))
            };

            Init(bvh, lights);
        }
    }
}