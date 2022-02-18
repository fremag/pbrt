using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Point3FTests
    {
        Point3F p1 = new Point3F(1.1f, 2.2f, 3.3f);
        Point3F p2 = new Point3F(4.4f, 5.5f, 6.6f);
        
        [Test]
        public void EmptyConstructorTest()
        {
            var p = new Point3F();
            Check.That(p.X).IsEqualTo(0);
            Check.That(p.Y).IsEqualTo(0);
            Check.That(p.Z).IsEqualTo(0);
        }
        
        [Test]
        public void ConstructorTest()
        {
            var p = new Point3F(1, 2, 3);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
            Check.That(p.Z).IsEqualTo(3);
        }

        [Test]
        public void PointConstructorTest()
        {
            var p0 = new Point3F(1, 2, 3);
            var p = new Point3F(p0);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
            Check.That(p.Z).IsEqualTo(3);
        }

        [Test]
        public void ZeroTest()
        {
            Check.That(Point3F.Zero.X).IsEqualTo(0);
            Check.That(Point3F.Zero.Y).IsEqualTo(0);
            Check.That(Point3F.Zero.Z).IsEqualTo(0);
        }

        [Test]
        public void HasNaNsTest()
        {
            var p = new Point3F(1, 2, 3);
            Check.That(p.HasNaNs).IsFalse();
             p = new Point3F(float.NaN, 2, 3);
            Check.That(p.HasNaNs).IsTrue();
            p = new Point3F(1, float.NaN, 3);
            Check.That(p.HasNaNs).IsTrue();
            p = new Point3F(1, 2, float.NaN);
            Check.That(p.HasNaNs).IsTrue();
        }

        [Test]
        public void IndexTest()
        {
            var p = new Point3F(1, 2, 3);
            Check.That(p[0]).IsEqualTo(1);
            Check.That(p[1]).IsEqualTo(2);
            Check.That(p[2]).IsEqualTo(3);
            Check.ThatCode(() => p[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => p[3]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void ExplicitConvertOperatorTest()
        {
            var p2f = (Point2F)p1;
            Check.That(p2f.X).IsEqualTo(1.1f);
            Check.That(p2f.Y).IsEqualTo(2.2f);
            var p3i = (Point3I)p1;
            Check.That(p3i.X).IsEqualTo(1);
            Check.That(p3i.Y).IsEqualTo(2);
            Check.That(p3i.Z).IsEqualTo(3);
        }

        [Test]
        public void AddTest()
        {
            var p = p1 + p2;
            Check.That(p.X).IsEqualTo(5.5f);
            Check.That(p.Y).IsEqualTo(7.7f);
            Check.That(p.Z).IsEqualTo(9.9f);
        }
        [Test]
        public void AddVectorTest()
        {
            var v2 = new Vector3F(p2);
            var p = p1 + v2;
            Check.That(p.X).IsEqualTo(5.5f);
            Check.That(p.Y).IsEqualTo(7.7f);
            Check.That(p.Z).IsEqualTo(9.9f);
        }

        [Test]
        public void SubTest()
        {
            var p = p1 - p2;
            Check.That(p.X).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Z).IsCloseTo(-3.3f, 1e-5);
        }
        [Test]
        public void SubVectorTest()
        {
            var v2 = new Vector3F(p2);
            var p = p1 - v2;
            Check.That(p.X).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Z).IsCloseTo(-3.3f, 1e-5);
        }
        
        [Test]
        public void MulTest()
        {
            var p = p1*3;
            Check.That(p.X).IsCloseTo(3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(6.6f, 1e-5);
            Check.That(p.Z).IsCloseTo(9.9f, 1e-5);

            p = 2*p1;
            Check.That(p.X).IsCloseTo(2.2f, 1e-5);
            Check.That(p.Y).IsCloseTo(4.4f, 1e-5);
            Check.That(p.Z).IsCloseTo(6.6f, 1e-5);
        }

        [Test]
        public void NegTest()
        {
            var p = -p1;
            Check.That(p.X).IsCloseTo(-1.1f, 1e-5);
            Check.That(p.Y).IsCloseTo(-2.2f, 1e-5);
            Check.That(p.Z).IsCloseTo(-3.3f, 1e-5);
        }

        [Test]
        public void EqualsTest()
        {
            Check.That(p1 == p2).IsFalse();
            Check.That(p1 == null).IsFalse();
            Check.That(null == p2).IsFalse();
            Check.That(p1 == p1).IsTrue();
            Check.That(p1 == new Point3F(p1)).IsTrue();

            Check.That(p1.Equals(p2)).IsFalse();
            Check.That(p1.Equals(p1)).IsTrue();
            Check.That(p1.Equals(5)).IsFalse();
            Check.That(p1.Equals(null)).IsFalse();
        }
        
        [Test]
        public void InEqualsTest()
        {
            Check.That(p1 != p2).IsTrue();
            Check.That(p1 != null).IsTrue();
            Check.That(null != p2).IsTrue();
            Check.That(p1 != p1).IsFalse();
            Check.That(p1 != new Point3F(p1)).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Check.That(p1.GetHashCode()).IsEqualTo(new Point3F(p1).GetHashCode());
            Check.That(p1.GetHashCode()).Not.IsEqualTo(p2.GetHashCode());
        }

        [Test]
        public void DivTest()
        {
            var p = p1 / 2;
            Check.That(p.X).IsEqualTo(0.55f);
            Check.That(p.Y).IsEqualTo(1.10f);
            Check.That(p.Z).IsEqualTo(1.65f);
        }

        [Test]
        public void DistanceTest()
        {
            Check.That(p1.Distance(p1)).IsZero();
            Check.That(p1.Distance(p1+new Point3F(1,1,-1))).IsCloseTo(MathF.Sqrt(3), 1e-5);
        }

        [Test]
        public void DistanceSquaredTest()
        {
            Check.That(p1.DistanceSquared(p1)).IsZero();
            Check.That(p1.DistanceSquared(p1+new Point3F(-1,-1,1))).IsCloseTo(3, 1e-5);
        }
    }
}