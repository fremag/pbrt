using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class Checkerboard3DTextureTests
    {
        [Test]
        [TestCase(0, 0, 0, 1.23f)]
        [TestCase(0, 0, 1.5f, 2.34f)]
        [TestCase(0, 1.5f, 0, 2.34f)]
        [TestCase(1.5f, 0, 0, 2.34f)]
        [TestCase(1.5f, 1.5f, 0, 1.23f)]
        public void MapTest(float x, float y, float z, float expectedT)
        {
            Transform transform = Transform.Translate(0,0,0);
            TextureMapping3D mapping = new TextureMapping3D(transform);
            Texture<float> tex1 = new ConstantTexture<float>(1.23f);
            Texture<float> tex2 = new ConstantTexture<float>(2.34f);
            var texture = new Checkerboard3DTexture<float>(mapping, tex1, tex2);
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