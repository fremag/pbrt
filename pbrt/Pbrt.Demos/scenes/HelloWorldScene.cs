using pbrt.Core;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.scenes
{
    public class HelloWorldScene : Scene
    {
        public HelloWorldScene()
        {
            Texture<float> sigma = new ConstantTexture<float>(50);
            Texture<Spectrum> kdRed = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 1, 0, 0 })));
            Texture<Spectrum> kdGreen = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 1, 0 })));
            Texture<Spectrum> kdBlue = new ConstantTexture<Spectrum>(new Spectrum(SampledSpectrum.FromRgb(new float[] { 0, 0, 1 })));
            IMaterial materialRed = new MatteMaterial(kdRed, sigma, null);
            IMaterial materialGreen = new MatteMaterial(kdGreen, sigma, null);
            IMaterial materialBlue = new MatteMaterial(kdBlue, sigma, null);

            var objectToWorld = Transform.Translate(0f, 0f, 0f);
            MediumInterface mediumInterface = new MediumInterface(HomogeneousMedium.Default());
            var sphere = new Sphere(objectToWorld, 0.5f);
            var primitive = new GeometricPrimitive(sphere, materialGreen, null, mediumInterface);
            MediumInterface medium = new MediumInterface(HomogeneousMedium.Default(), HomogeneousMedium.Default());
            var lights = new Light[]
            {
                new PointLight(Transform.Translate(10, 10, -10), medium, new Spectrum(1000f)),
            };

            Init(primitive, lights);
        }
    }
}