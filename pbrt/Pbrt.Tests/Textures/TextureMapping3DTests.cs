using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class TextureMapping3DTests
    {
        [Test]
        public void MapTest()
        {
            Transform transform = Transform.Translate(1.23f, 2.34f, 3.45f) ;
            var textureMapping3D = new TextureMapping3D(transform);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(0, 0, 0),
                DpDx = new Vector3F(5f, 6f, 7f),
                DpDy = new Vector3F(8f, 9f, 10f),
                DuDx = -0.1f,
                DuDy = 0.1f,
                DvDx = 0.2f,
                DvDy = -0.2f,
            };
            var p = textureMapping3D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(1.23f, 1e-4f);
            Check.That(p.Y).IsCloseTo(2.34f, 1e-4f);
            Check.That(p.Z).IsCloseTo(3.45f, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(5f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(6f, 1e-4f);
            Check.That(dstdx.Z).IsCloseTo(7f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(8f, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(9f, 1e-4f);
            Check.That(dstdy.Z).IsCloseTo(10f, 1e-4f);
        }
    }
}