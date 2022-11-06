using pbrt.Core;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Integrators;
using pbrt.Lights;
using pbrt.Materials;
using pbrt.Shapes;
using pbrt.Spectrums;

namespace Pbrt.Demos.Demos
{
    public class ImageTextureDemo : AbstractDemo
    {
        public override string FileName => "Image_texture.png";

        public ImageTextureDemo() : base("Image Textures")
        {
            CameraConfig = new CameraConfig
            {
                Camera = Configs.Camera.Perspective,
                Config = new PerspectiveCameraConfig
                {
                    Position = (1, 2.5f, -1f),
                    LookAt = (0, 1f, 0),
                    Width = 600,
                    Height = 400
                }
            };
            SamplerConfig = new SamplerConfig
            {
                Sampler = Configs.Sampler.Halton,
                Config = new HaltonSamplerConfig { SamplesPerPixel = 16 }
            };

            IntegratorConfig = new IntegratorConfig
            {
                Integrator = IntegratorType.DirectLighting,
                Config = new DirectLightingConfig
                {
                    Strategy = LightStrategy.UniformSampleAll
                }
            };
            Scene = new ImageTextureScene();
        }
    }

    public class ImageTextureScene : DemoScene
    {
        public ImageTextureScene()
        {
            var transform =Translate(tY: 1f) *  RotateY(-90f) * RotateX(90f);
            
            string earthTexturePath = @"Pbrt.Demos.Textures.earth.jpg";
            var earthTexture = GetTexture(earthTexturePath);

            IMaterial earthMaterial = new MatteMaterial(earthTexture, MakeTexture(1f), null);
            Sphere(0, 0, 0, 0.75f, earthMaterial, transform: transform);

            var plane = Plane(-3, -3, 3, 3);
            string planeTexturePath = @"Pbrt.Demos.Textures.uv_mapper.jpg";
            var planeTexture = GetTexture(planeTexturePath);
            IMaterial planeMaterial = new MatteMaterial(planeTexture, MakeTexture(1f), null);
            AddPrimitives(plane, planeMaterial);
            
            var lightToWorld = Translate(tY: 20) * RotateX(90);
            IShape shape = new Disk(lightToWorld, 2f);
            var light = new DiffuseAreaLight(lightToWorld, DefaultMediumInterface, new Spectrum(100f), 1, shape);
            AllLights.Add(light);
        }
   }
}