using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class UVMapping2DTests
    {
        [Test]
        public void MapTest()
        {
            UVMapping2D uvMap2D = new UVMapping2D(0.1f, 0.2f, 0.3f, 0.4f);
            SurfaceInteraction si = new SurfaceInteraction();
            si.DuDx = -0.1f;
            si.DuDy = 0.1f;
            si.DvDx = 0.2f;
            si.DvDy = -0.2f;
            si.Uv = new Point2F(1.23f, 2.34f);
            var p = uvMap2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsEqualTo(0.423f);
            Check.That(p.Y).IsCloseTo(0.868f, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(-0.01f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(0.04f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0.01f, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(-0.04f, 1e-4f);
        }
    }
}