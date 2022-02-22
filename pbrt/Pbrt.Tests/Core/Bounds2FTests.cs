using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Bounds2FTests
    {
        private Point2F p1 = new Point2F(-1, -1);
        private Point2F p2 = new Point2F(1, 1);
        
        private Bounds2F bounds;
        
        [SetUp]
        public void SetUp() 
        {
            bounds = new Bounds2F(p1, p2);
        }

        [Test]
        public void ConstructorTest()
        {
            var b = new Bounds2F();
            Check.That(b.PMin).IsEqualTo(new Point2F(float.MaxValue,float.MaxValue));
            Check.That(b.PMax).IsEqualTo(new Point2F(float.MinValue,float.MinValue));
        }
        
        [Test]
        public void ConstructorPointTest()
        {
            var b = new Bounds2F(Point2F.Zero);
            Check.That(b.PMin).IsEqualTo(Point2F.Zero);
            Check.That(b.PMax).IsEqualTo(Point2F.Zero);
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
        public void DiagonalTest()
        {
            var diag = bounds.Diagonal(); 
            Check.That(diag).IsEqualTo(new Vector2F(2,2));
        }
       
        [Test]
        [TestCase(0, -1, 1)]
        [TestCase(1,  1, 1)]
        [TestCase(2, -1,-1)]
        [TestCase(3,  1,-1)]
        public void CornerTest(int corner, float x, float y)
        {
            var c = bounds.Corner(corner); 
            Check.That(c).IsEqualTo(new Point2F(x,y));
        }
       
        [Test]
        public void UnionBoundsTest()
        {
            var ext = bounds.Union(new Bounds2F(new Point2F(0,0), new Point2F(3,0)));
            Check.That(ext.PMin).IsEqualTo(new Point2F(-1, -1));
            Check.That(ext.PMax).IsEqualTo(new Point2F( 3,  1));

            ext = bounds.Union(new Bounds2F(new Point2F(0,0), new Point2F(0,2)));
            Check.That(ext.PMin).IsEqualTo(new Point2F(-1, -1));
            Check.That(ext.PMax).IsEqualTo(new Point2F( 1,  2));

            ext = bounds.Union(new Bounds2F(new Point2F(-2,0), new Point2F(2,2)));
            Check.That(ext.PMin).IsEqualTo(new Point2F(-2, -1));
            Check.That(ext.PMax).IsEqualTo(new Point2F( 2,  2));
        }
        
        [Test]
        [TestCase(0, 0, true)]
        [TestCase(3, 0, false)]
        [TestCase(1, 1, true)]
        [TestCase(-1, -1, true)]
        [TestCase(-2, 0, false)]
        [TestCase(0, -2, false)]
        [TestCase(0, 2,  false)]
        [TestCase(2, 0, false)]
        [TestCase(-1, 0, true)]
        [TestCase(0, -1, true)]
        [TestCase(0, 0,  true)]
        [TestCase(0, 1,  true)]
        [TestCase(1, 0,  true)]
        public void InsidePointTest(float x, float y, bool isInside)
        {
            var inside = bounds.Inside(new Point2F(x, y));
            Check.That(inside).IsEqualTo(isInside);
        }
      
        [Test]
        [TestCase(0, 0, true)]
        [TestCase(-1, 0, true)]
        [TestCase(0, -1, true)]
        [TestCase(-1, 0, true)]
        [TestCase(0, -1, true)]
        [TestCase(0, 0, true)]
        [TestCase(3, 0, false)]
        [TestCase(1, 1, false)]
        [TestCase(-2, 0, false)]
        [TestCase(0, -2, false)]
        [TestCase(0, 2, false)]
        [TestCase(2, 0, false)]
        [TestCase(0, 1, false)]
        [TestCase(1, 0, false)]
        public void InsideExclusivePointTest(float x, float y, bool isInside)
        {
            var inside = bounds.InsideExclusive(new Point2F(x, y));
            Check.That(inside).IsEqualTo(isInside);
        }

        [Test]
        public void ExpandTest()
        {
            var exp = bounds.Expand(1);
            Check.That(exp.PMax).IsEqualTo(p2 + new Point2F(1f, 1f));
            Check.That(exp.PMin.X).IsEqualTo(-2);
            Check.That(exp.PMin.Y).IsEqualTo(-2);
        }

        [Test]
        public void IntersectTest()
        {
            var b1 = new Bounds2F(new Point2F(0, 0), new Point2F(2, 2));
            var inter1 = bounds.Intersect(b1);
            Check.That(inter1.PMin).IsEqualTo(new Point2F(0, 0));
            Check.That(inter1.PMax).IsEqualTo(new Point2F(1, 1));
        }
        
        [Test]
        public void OverlapsTest()
        {
            var b1 = new Bounds2F(new Point2F(0, 0), new Point2F(2, 2));
            Check.That(bounds.Overlaps(b1)).IsTrue();
        }

        [Test]
        public void LerpTest()
        {
            Check.That(bounds.Lerp(new Point2F(0,0))).IsEqualTo(bounds.PMin);
            Check.That(bounds.Lerp(new Point2F(1,1))).IsEqualTo(bounds.PMax);
            Check.That(bounds.Lerp(new Point2F(0.5f,0.5f))).IsEqualTo(Point2F.Zero);
        }

        [Test]
        [TestCase(-1, -1,  0, 0)]
        [TestCase(1, 1,  1, 1)]
        [TestCase(0, 0,  0.5f, 0.5f)]
        [TestCase(-1, 1,  0, 1)]
        public void OffsetTest(float x, float y, float expX, float expY)
        {
            var v = bounds.Offset(new Point2F(x, y));
            Check.That(v.X).IsEqualTo(expX);
            Check.That(v.Y).IsEqualTo(expY);
        }

        [Test]
        public void SurfaceAreaTest()
        {
            Check.That(bounds.SurfaceArea).IsEqualTo(2 * 2);
        }

        [Test]
        public void MaximumExtentTest()
        {
            var b1 = new Bounds2F(Point2F.Zero, new Point2F(2, 1));
            Check.That(b1.MaximumExtent).IsEqualTo(0);
            var b2 = new Bounds2F(Point2F.Zero, new Point2F(1, 2));
            Check.That(b2.MaximumExtent).IsEqualTo(1);
        }
    }
}