using pbrt.Materials;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes
{
    public class CubeScene : DemoScene
    {
        public CubeScene()
        {
            Floor();
            Axis();

            var transform = Translate(tY: 1f)*RotateY(45);
            
            var cube = Cube(transform);
            Texture<Spectrum> kdTex1 =  MakeSpectrumTexture(Rgb(19, 185, 85));
            Texture<Spectrum> kdTex2 =  MakeSpectrumTexture(Rgb( 173, 30, 149));
            float s = 2+1e-5f;
            TextureMapping3D mapping = new TextureMapping3D(transform*Scale(s, s, s));
            var kdChecker = new Checkerboard3DTexture<Spectrum>(mapping, kdTex2, kdTex1);
            var material = new MatteMaterial(kdChecker, MakeTexture(50f), null);

            AddPrimitives(BuildTriangles(cube, material));
            
            PointLight(5, 15, -10, 500f);
        }

    }
}