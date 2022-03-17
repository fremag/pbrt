using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Textures
{
    [TestFixture]
    public class SphericalMapping2DTests
    {
        [Test]
        public void MapTest()
        {
            Transform transform = Transform.Translate(1, 2, 3);
            SphericalMapping2D sphericalMapping2D = new SphericalMapping2D(transform);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = Point3F.Zero,
                DpDx = new Vector3F(0, 1, 0),
                DpDy = new Vector3F(1, 0, 0),
                DuDx = -0.1f,
                DuDy = 0.1f,
                DvDx = 0.2f,
                DvDy = -0.2f,
                Uv = new Point2F(1.23f, 2.34f)
            };
            var p = sphericalMapping2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(0.20388459f, 1e-4f);
            Check.That(p.Y).IsCloseTo(0.1762082, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(0.060426593f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(0.030602813f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0.031465292f, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(-0.062381923f, 1e-4f);
        }
        
        [Test]
        public void Map_2_Test()
        {
            Transform transform = Transform.Translate(1, 0, 0);
            SphericalMapping2D sphericalMapping2D = new SphericalMapping2D(transform);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(0, 0, 0),
                DpDx = new Vector3F(5f, 5f, 0),
                DpDy = new Vector3F(5f, 5f, 0),
                DuDx = -0.1f,
                DuDy = 0.1f,
                DvDx = 0.2f,
                DvDy = -0.2f,
                Uv = new Point2F(1.23f, 2.34f)
            };
            var p = sphericalMapping2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(0.5f, 1e-4f);
            Check.That(p.Y).IsCloseTo(0, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(0.487918079f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(0.487918079f, 1e-4f);
        }
           
        [Test]
        public void Map_3_Test()
        {
            Transform transform = Transform.Translate(-1, 0, 0);
            SphericalMapping2D sphericalMapping2D = new SphericalMapping2D(transform);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(0, 0, 0),
                DpDx = new Vector3F(-5f, 5f, 0),
                DpDy = new Vector3F(5f, -5f, 0),
                DuDx = 0.1f,
                DuDy = -0.1f,
                DvDx = 0.2f,
                DvDy = -0.2f,
                Uv = new Point2F(0f, 0f)
            };
            var p = sphericalMapping2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(0.5f, 1e-4f);
            Check.That(p.Y).IsCloseTo(0.5f, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(-0.487918079f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(-0.25f, 1e-4f);
        }
          
        [Test]
        public void Map_4_Test()
        {
            Transform transform = Transform.Translate(-1, 0, 0);
            SphericalMapping2D sphericalMapping2D = new SphericalMapping2D(transform);
            SurfaceInteraction si = new SurfaceInteraction
            {
                P = new Point3F(0, 0, 0),
                DpDx = new Vector3F(5f, -5f, 0),
                DpDy = new Vector3F(-5f, 5f, 0),
                DuDx = 0.1f,
                DuDy = 0.1f,
                DvDx = 0.2f,
                DvDy = 0.2f,
                Uv = new Point2F(0f, 0f)
            };
            var p = sphericalMapping2D.Map(si, out var dstdx, out var dstdy);

            Check.That(p.X).IsCloseTo(0.5f, 1e-4f);
            Check.That(p.Y).IsCloseTo(0.5f, 1e-4f);
            Check.That(dstdx.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdx.Y).IsCloseTo(-0.25f, 1e-4f);
            Check.That(dstdy.X).IsCloseTo(0f, 1e-4f);
            Check.That(dstdy.Y).IsCloseTo(-0.487918079f, 1e-4f);
        }

        [Test]
        [TestCase(1, 0, 0, 0.5f, 0)]
        [TestCase(1, 1, 0, 0.5f, 0.07379181f)]
        public void SphereTest(float x, float y, float z, float expectedX, float expectedY)
        {
            Transform transform = Transform.Translate(1, 0, 0);
            SphericalMapping2D sphericalMapping2D = new SphericalMapping2D(transform);
            var v = sphericalMapping2D.Sphere(new Point3F(x, y, z));
            Check.That(v.X).IsCloseTo(expectedX, 1e-5);
            Check.That(v.Y).IsCloseTo(expectedY, 1e-5);
        }
    }
}