using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class Checkerboard2DTextureTests
    {
        [Test]
        [TestCase(0, 0, 0, 1.23f)]
        [TestCase(1.5f, 0, 0, 2.34f)]
        [TestCase(1.5f, 1.5f, 0, 1.23f)]
        public void MapTest(float x, float y, float z, float expectedT)
        {
            TextureMapping2D mapping = new PlanarMapping2D(new Vector3F(1f, 0, 0), new Vector3F(0, 1f, 0), 0, 0);
            Texture<float> tex1 = new ConstantTexture<float>(1.23f);
            Texture<float> tex2 = new ConstantTexture<float>(2.34f);
            Checkerboard2DTexture<float> texture = new Checkerboard2DTexture<float>(mapping, tex1, tex2);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(x, y, z),
                DpDx = Vector3F.Zero,
                DpDy = Vector3F.Zero
            };

            var t = texture.Evaluate(si);
            Check.That(t).IsEqualTo(expectedT);
        }
    }
}