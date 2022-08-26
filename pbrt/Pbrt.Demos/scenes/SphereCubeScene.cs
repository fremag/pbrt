using System;
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
    public class SphereCubeScene : Scene
    {
        public SphereCubeScene(int maxI = 5,  int maxJ = 5, int maxK = 5, float radius = 0.5f)
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
            
            IMaterial mirrorMaterial = new MirrorMaterial(new ConstantTexture<Spectrum>(new Spectrum(1f)), null);
            
            MediumInterface mediumInterface = new MediumInterface(HomogeneousMedium.Default());
           
            var planeTransform = Transform.Scale(100f, 0.1f, 100f);
            var plane = new Sphere(planeTransform, 1f);
            var planePrimitive = new GeometricPrimitive(plane, new MatteMaterial(kdChecker, sigma, null), null, mediumInterface);

            var primitives = new List<IPrimitive>
            {
                planePrimitive 
            };

            for(int i=0; i < maxI; i ++)
            {
                for(int j=0; j < maxJ; j ++)
                {
                    for (int k = 0; k < maxK; k++)
                    {
                        var translate = Transform.Translate(i, j+radius, k);
                        var sphere = new Sphere(translate, radius);
                        var spherePrimitive = new GeometricPrimitive(sphere, materialGreen, null, mediumInterface);
                        primitives.Add(spherePrimitive); 
                    }
                }
            }

            BvhAccel bvh = new BvhAccel(primitives, 5, SplitMethod.Middle);

            MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(10, 10, -10), medium, new Spectrum(1000f))
            };

            Init(bvh, lights);
        }
    }
}