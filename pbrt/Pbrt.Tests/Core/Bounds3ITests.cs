using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Bounds3ITests
    {
        private Point3I p1 = new Point3I(-1, -1, -1);
        private Point3I p2 = new Point3I(1, 1, 1);

        private Bounds3I bounds;

        [SetUp]
        public void SetUp()
        {
            bounds = new Bounds3I(p1, p2);
        }

        [Test]
        public void ConstructorTest()
        {
            var b = new Bounds3I();
            Check.That(b.PMin).IsEqualTo(new Point3I(int.MaxValue, int.MaxValue, int.MaxValue));
            Check.That(b.PMax).IsEqualTo(new Point3I(int.MinValue, int.MinValue, int.MinValue));
        }

        [Test]
        public void ConstructorPointTest()
        {
            var b = new Bounds3I(Point3I.Zero);
            Check.That(b.PMin).IsEqualTo(Point3I.Zero);
            Check.That(b.PMax).IsEqualTo(Point3I.Zero);
        }

        [Test]
        public void ConstructorPMinMaxTest()
        {
            Check.That(bounds.PMin).IsEqualTo(p1);
            Check.That(bounds.PMax).IsEqualTo(p2);
        }

        [Test]
        public void IndexTest()
        {
            Check.That(bounds[0]).IsEqualTo(p1);
            Check.That(bounds[1]).IsEqualTo(p2);
            Check.ThatCode(() => bounds[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => bounds[4]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void SurfaceAreaTest()
        {
            Check.That(bounds.SurfaceArea).IsEqualTo(6 * 4);
        }

        [Test]
        public void VolumeTest()
        {
            Check.That(bounds.Volume).IsEqualTo(8);
        }

        [Test]
        public void MaximumExtentTest()
        {
            var b1 = new Bounds3I(Point3I.Zero, new Point3I(2, 1, 1));
            Check.That(b1.MaximumExtent).IsEqualTo(0);
            var b2 = new Bounds3I(Point3I.Zero, new Point3I(1, 2, 1));
            Check.That(b2.MaximumExtent).IsEqualTo(1);
            var b3 = new Bounds3I(Point3I.Zero, new Point3I(1, 1, 2));
            Check.That(b3.MaximumExtent).IsEqualTo(2);
        }

        [Test]
        public void DiagonalTest()
        {
            var diag = bounds.Diagonal();
            Check.That(diag).IsEqualTo(new Vector3I(2, 2, 2));
        }

        [Test]
        [TestCase(0, -1, 1, 1)]
        [TestCase(1, 1, 1, 1)]
        [TestCase(2, -1, -1, 1)]
        [TestCase(3, 1, -1, 1)]
        [TestCase(4, -1, 1, -1)]
        [TestCase(5, 1, 1, -1)]
        [TestCase(6, -1, -1, -1)]
        [TestCase(7, 1, -1, -1)]
        public void CornerTest(int corner, int x, int y, int z)
        {
            var c = bounds.Corner(corner);
            Check.That(c).IsEqualTo(new Point3I(x, y, z));
        }

        [Test]
        public void UnionPointTest()
        {
            var ext = bounds.Union(Point3I.Zero);
            Check.That(ext.PMin).IsEqualTo(p1);
            Check.That(ext.PMax).IsEqualTo(p2);

            var p3 = new Point3I(2, 1, 1);
            ext = bounds.Union(p3);
            Check.That(ext.PMin).IsEqualTo(p1);
            Check.That(ext.PMax).IsEqualTo(p3);

            var p4 = new Point3I(-2, -2, -2);
            ext = bounds.Union(p4);
            Check.That(ext.PMin).IsEqualTo(p4);
            Check.That(ext.PMax).IsEqualTo(p2);

            var p5 = new Point3I(-2, 0, 0);
            ext = bounds.Union(p5);
            Check.That(ext.PMin).IsEqualTo(new Point3I(-2, -1, -1));
            Check.That(ext.PMax).IsEqualTo(p2);
        }

        [Test]
        public void UnionBoundsTest()
        {
            var ext = bounds.Union(new Bounds3I(new Point3I(0, 0, 0), new Point3I(3, 0, 0)));
            Check.That(ext.PMin).IsEqualTo(new Point3I(-1, -1, -1));
            Check.That(ext.PMax).IsEqualTo(new Point3I(3, 1, 1));

            ext = bounds.Union(new Bounds3I(new Point3I(0, 0, 0), new Point3I(0, 2, 2)));
            Check.That(ext.PMin).IsEqualTo(new Point3I(-1, -1, -1));
            Check.That(ext.PMax).IsEqualTo(new Point3I(1, 2, 2));

            ext = bounds.Union(new Bounds3I(new Point3I(-2, 0, 0), new Point3I(2, 2, 2)));
            Check.That(ext.PMin).IsEqualTo(new Point3I(-2, -1, -1));
            Check.That(ext.PMax).IsEqualTo(new Point3I(2, 2, 2));
        }

        [Test]
        [TestCase(0, 0, 0, true)]
        [TestCase(3, 0, 0, false)]
        [TestCase(1, 1, 1, true)]
        [TestCase(-1, -1, -2, false)]
        [TestCase(-2, 0, 0, false)]
        [TestCase(0, -2, 0, false)]
        [TestCase(0, 0, -2, false)]
        [TestCase(0, 0, 2, false)]
        [TestCase(0, 2, 0, false)]
        [TestCase(2, 0, 0, false)]
        [TestCase(-1, 0, 0, true)]
        [TestCase(0, -1, 0, true)]
        [TestCase(0, 0, -1, true)]
        [TestCase(0, 0, 1, true)]
        [TestCase(0, 1, 0, true)]
        [TestCase(1, 0, 0, true)]
        public void InsidePointTest(int x, int y, int z, bool isInside)
        {
            var inside = bounds.Inside(new Point3I(x, y, z));
            Check.That(inside).IsEqualTo(isInside);
        }

        [Test]
        [TestCase(0, 0, 0, true)]
        [TestCase(3, 0, 0, false)]
        [TestCase(1, 1, 1, false)]
        [TestCase(-1, -1, -2, false)]
        [TestCase(-2, 0, 0, false)]
        [TestCase(0, -2, 0, false)]
        [TestCase(0, 0, -2, false)]
        [TestCase(0, 0, 2, false)]
        [TestCase(0, 2, 0, false)]
        [TestCase(2, 0, 0, false)]
        [TestCase(-1, 0, 0, true)]
        [TestCase(0, -1, 0, true)]
        [TestCase(0, 0, -1, true)]
        [TestCase(0, 0, 1, false)]
        [TestCase(0, 1, 0, false)]
        [TestCase(1, 0, 0, false)]
        public void InsideExclusivePointTest(int x, int y, int z, bool isInside)
        {
            var inside = bounds.InsideExclusive(new Point3I(x, y, z));
            Check.That(inside).IsEqualTo(isInside);
        }

        [Test]
        public void ExpandTest()
        {
            var exp = bounds.Expand(1);
            Check.That(exp.PMax).IsEqualTo(p2 + new Point3I(1, 1, 1));
            Check.That(exp.PMin.X).IsEqualTo(-2);
            Check.That(exp.PMin.Y).IsEqualTo(-2);
            Check.That(exp.PMin.Z).IsEqualTo(-2);
        }

        [Test]
        public void IntersectTest()
        {
            var b1 = new Bounds3I(new Point3I(0, 0, 0), new Point3I(2, 2, 2));
            var inter1 = bounds.Intersect(b1);
            Check.That(inter1.PMin).IsEqualTo(new Point3I(0, 0, 0));
            Check.That(inter1.PMax).IsEqualTo(new Point3I(1, 1, 1));
        }

        [Test]
        public void OverlapsTest()
        {
            var b1 = new Bounds3I(new Point3I(0, 0, 0), new Point3I(2, 2, 2));
            Check.That(bounds.Overlaps(b1)).IsTrue();
        }

        [Test]
        public void LerpTest()
        {
            Check.That(bounds.Lerp(new Point3F(0, 0, 0))).IsEqualTo(bounds.PMin);
            Check.That(bounds.Lerp(new Point3F(1, 1, 1))).IsEqualTo(bounds.PMax);
            Check.That(bounds.Lerp(new Point3F(0.5f, 0.5f, 0.5f))).IsEqualTo(Point3I.Zero);
        }

        [Test]
        [TestCase(-1, -1, -1, 0, 0, 0)]
        [TestCase(1, 1, 1, 1, 1, 1)]
        [TestCase(0, 0, 0, 0, 0, 0)]
        [TestCase(-1, 1, 0, 0, 1, 0)]
        public void OffsetTest(int x, int y, int z, int expX, int expY, int expZ)
        {
            var v = bounds.Offset(new Point3I(x, y, z));
            Check.That(v.X).IsEqualTo(expX);
            Check.That(v.Y).IsEqualTo(expY);
            Check.That(v.Z).IsEqualTo(expZ);
        }

        [Test]
        public void BoundingSphereTest()
        {
            bounds.BoundingSphere(out Point3I center, out float radius);
            Check.That(center).IsEqualTo(Point3I.Zero);
            Check.That(radius).IsEqualTo(MathF.Sqrt(3));

            var b = new Bounds3I(Point3I.Zero, new Point3I(3, 2, 1));
            b.BoundingSphere(out center, out radius);
            Check.That(center).IsEqualTo(new Point3I(1, 1, 0));
            Check.That(radius).IsCloseTo(Math.Sqrt(6), 1e-6);
        }
    }
}