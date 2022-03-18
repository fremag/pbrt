using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class PlanarMapping2DTests
    {
        [Test]
        public void MapTest()
        {
            Vector3F vs = new Vector3F(0,0,0);
            Vector3F vt = new Vector3F(1,1,1);
            float ds = 1.23f;
            float dt = 2.34f;
            PlanarMapping2D mapping2D = new PlanarMapping2D(vs, vt, ds, dt);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(0, 0, 0),
                DpDx = new Vector3F(0f, 0f, 5f),
                DpDy = new Vector3F(0f, 0f, -5f),
                DuDx = -0.1f,
                DuDy = 0.1f,
                DvDx = 0.2f,
                DvDy = -0.2f,
                Uv = new Point2F(1.23f, 2.34f)
            };
            var p = mapping2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(1.23f, 1e-4f);
            Check.That(p.Y).IsCloseTo(2.34f, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(5f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(-5f, 1e-4f);
        }
    }
}