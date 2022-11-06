using pbrt.Core;
using Pbrt.Demos.Configs;
using Pbrt.Demos.Scenes;
using pbrt.Materials;
using pbrt.Textures;

namespace Pbrt.Demos.Demos
{
    public class ImageTextureDemo : AbstractDemo
    {
        public override string FileName => "image_texture.png";

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
            Scene = new ImageTextureScene();
            //IntegratorConfig.Config.NbThreads = 1;
        }
    }

    public class ImageTextureScene : DemoScene
    {
        public ImageTextureScene()
        {
            var transform =Translate(tY: 1f) *  RotateY(-90f) * RotateX(90f);
            
            string earthTexturePath = @"E:\Projects\pbrt\pbrt\Pbrt.Demos\Textures\earth.jpg";
            TextureMapping2D mapping = new UVMapping2D(1f, 1f, 0, 0);
            var earthTexture = new ImageTexture(mapping, earthTexturePath, true, 1, ImageWrap.Repeat, 1, true);

            IMaterial earthMaterial = new MatteMaterial(earthTexture, MakeTexture(1f), null);
            Sphere(0, 0, 0, 0.5f, earthMaterial, transform: transform);

            var plane = Plane(-3, -3, 3, 3);
            string planeTexturePath = @"E:\Projects\pbrt\pbrt\Pbrt.Demos\Textures\uv_mapper.jpg";
            var planeTexture = new ImageTexture(mapping, planeTexturePath, true, 1, ImageWrap.Repeat, 1, true);
            IMaterial planeMaterial = new MatteMaterial(planeTexture, MakeTexture(1f), null);
            AddPrimitives(plane, planeMaterial);
            
            PointLight(0, 10, -10, 300f);
            PointLight(0, 10, 0, 200f);
        }
    }
}