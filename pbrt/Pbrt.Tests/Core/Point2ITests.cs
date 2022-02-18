using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Point2ITests
    {
        Point2I p1 = new Point2I(1, 2);
        Point2I p2 = new Point2I(4, 5);
        
        [Test]
        public void EmptyConstructorTest()
        {
            var p = new Point2I();
            Check.That(p.X).IsEqualTo(0);
            Check.That(p.Y).IsEqualTo(0);
        }
        
        [Test]
        public void ConstructorTest()
        {
            var p = new Point2I(1, 2);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void PointConstructorTest()
        {
            var p0 = new Point2I(1, 2);
            var p = new Point2I(p0);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void IndexTest()
        {
            var p = new Point2I(1, 2);
            Check.That(p[0]).IsEqualTo(1);
            Check.That(p[1]).IsEqualTo(2);
            Check.ThatCode(() => p[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => p[2]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void ExplicitConvertOperatorTest()
        {
            var p2f = (Point2F)p1;
            Check.That(p2f.X).IsEqualTo(1f);
            Check.That(p2f.Y).IsEqualTo(2f);

            var p3i = new Point3I(1, 2, 3);
            var p2i = (Point2I)p3i;
            Check.That(p2i.X).IsEqualTo(1);
            Check.That(p2i.Y).IsEqualTo(2);
        }

        [Test]
        public void AddTest()
        {
            var p = p1 + p2;
            Check.That(p.X).IsEqualTo(5);
            Check.That(p.Y).IsEqualTo(7);
        }
        
        [Test]
        public void AddVectorTest()
        {
            var v = new Vector2I(3, 4);
            var p = p1 + v;
            Check.That(p.X).IsEqualTo(4);
            Check.That(p.Y).IsEqualTo(6);
        }

        [Test]
        public void SubTest()
        {
            var p = p1 - p2;
            Check.That(p.X).IsEqualTo(-3);
            Check.That(p.Y).IsEqualTo(-3);
        }
        
        [Test]
        public void SubVectorTest()
        {
            var v = new Vector2I(-5, 4);
            var p = p1 - v;
            Check.That(p.X).IsEqualTo(6);
            Check.That(p.Y).IsEqualTo(-2);
        }
        
        [Test]
        public void MulTest()
        {
            var p = p1*3;
            Check.That(p.X).IsEqualTo(3);
            Check.That(p.Y).IsEqualTo(6);

            p = 2*p1;
            Check.That(p.X).IsEqualTo(2);
            Check.That(p.Y).IsEqualTo(4);
        }

        [Test]
        public void MulFloatTest()
        {
            var p = p1*3.5f;
            Check.That(p.X).IsEqualTo(3.5f);
            Check.That(p.Y).IsEqualTo(7.0f);

            p = 2.5f*p1;
            Check.That(p.X).IsEqualTo(2.5f);
            Check.That(p.Y).IsEqualTo(5.0f);
        }

        [Test]
        public void NegTest()
        {
            var p = -p1;
            Check.That(p.X).IsEqualTo(-1);
            Check.That(p.Y).IsEqualTo(-2);
        }

        [Test]
        public void EqualsTest()
        {
            Check.That(p1 == p2).IsFalse();
            Check.That(p1 == null).IsFalse();
            Check.That(null == p2).IsFalse();
            Check.That(p1 == p1).IsTrue();
            Check.That(p1 == new Point2I(p1)).IsTrue();

            Check.That(p1.Equals(p2)).IsFalse();
            Check.That(p1.Equals(p1)).IsTrue();
            Check.That(p1.Equals(5)).IsFalse();
            Check.That(p1.Equals(null)).IsFalse();
        }
        
        [Test]
        public void NotEqualsTest()
        {
            Check.That(p1 != p2).IsTrue();
            Check.That(p1 != null).IsTrue();
            Check.That(null != p2).IsTrue();
            Check.That(p1 != p1).IsFalse();
            Check.That(p1 != new Point2I(p1)).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Check.That(p1.GetHashCode()).IsEqualTo(new Point2I(p1).GetHashCode());
            Check.That(p1.GetHashCode()).Not.IsEqualTo(p2.GetHashCode());
        }

        [Test]
        public void DivTest()
        {
            var p = p1 / 2;
            Check.That(p.X).IsEqualTo(0);
            Check.That(p.Y).IsEqualTo(1);
        }

        [Test]
        public void DistanceTest()
        {
            Check.That(p1.Distance(p1)).IsZero();
            Check.That(p1.Distance(p1+new Point2I(1,1))).IsCloseTo(MathF.Sqrt(2), 1e-5);
        }

        [Test]
        public void DistanceSquaredTest()
        {
            Check.That(p1.DistanceSquared(p1)).IsZero();
            Check.That(p1.DistanceSquared(p1+new Point2I(-1,-1))).IsCloseTo(2, 1e-5);
        }

        [Test]
        public void MinTest()
        {
            var p = Point2I.Min(new Point2I(1,2),new Point2I(-1, 4));
            Check.That(p.X).IsEqualTo(-1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void MaxTest()
        {
            var p = Point2I.Max(new Point2I(1,2),new Point2I(-1, 4));
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(4);
        }
        
        [Test]
        public void AbsTest()
        {
            var p = new Point2I(-1, 4).Abs();
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(4);
        }

        [Test]
        public void LerpTest()
        {
            var p = Point2I.Lerp(0.5f, p1, p2);
            Check.That(p.X).IsEqualTo(2.5f);
            Check.That(p.Y).IsEqualTo(3.5f);

            p = Point2I.Lerp(0, p1, p2);
            Check.That(p).IsEqualTo((Point2F)p1);
            p = Point2I.Lerp(1, p1, p2);
            Check.That(p).IsEqualTo((Point2F)p2);
        }
    }
}