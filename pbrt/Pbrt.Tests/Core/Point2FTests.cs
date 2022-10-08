using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

// ReSharper disable EqualExpressionComparison
// ReSharper disable PossibleNullReferenceException
// ReSharper disable SuspiciousTypeConversion.Global
#pragma warning disable CS1718

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Point2FTests
    {
        private readonly Point2F p1 = new Point2F(1.1f, 2.2f);
        private readonly Point2F p2 = new Point2F(4.4f, 5.5f);

        [Test]
        public void EmptyConstructorTest()
        {
            var p = new Point2F();
            Check.That(p.X).IsEqualTo(0);
            Check.That(p.Y).IsEqualTo(0);
        }

        [Test]
        public void ConstructorTest()
        {
            var p = new Point2F(1, 2);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void PointConstructorTest()
        {
            var p0 = new Point2F(1, 2);
            var p = new Point2F(p0);
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void IndexTest()
        {
            var p = new Point2F(1, 2);
            Check.That(p[0]).IsEqualTo(1);
            Check.That(p[1]).IsEqualTo(2);
            Check.ThatCode(() => p[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => p[2]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void ExplicitConvertOperatorTest()
        {
            var p3I = (Point2I)p1;
            Check.That(p3I.X).IsEqualTo(1);
            Check.That(p3I.Y).IsEqualTo(2);
        }

        [Test]
        public void AddTest()
        {
            var p = p1 + p2;
            Check.That(p.X).IsEqualTo(5.5f);
            Check.That(p.Y).IsEqualTo(7.7f);
        }

        [Test]
        public void AddVectorTest()
        {
            var v2 = new Vector2F(p2);
            var p = p1 + v2;
            Check.That(p.X).IsEqualTo(5.5f);
            Check.That(p.Y).IsEqualTo(7.7f);
        }

        [Test]
        public void SubTest()
        {
            var p = p1 - p2;
            Check.That(p.X).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(-3.3f, 1e-5);
        }

        [Test]
        public void SubVectorTest()
        {
            var v2 = new Vector2F(p2);
            var p = p1 - v2;
            Check.That(p.X).IsCloseTo(-3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(-3.3f, 1e-5);
        }

        [Test]
        public void MulTest()
        {
            var p = p1 * 3;
            Check.That(p.X).IsCloseTo(3.3f, 1e-5);
            Check.That(p.Y).IsCloseTo(6.6f, 1e-5);

            p = 2 * p1;
            Check.That(p.X).IsCloseTo(2.2f, 1e-5);
            Check.That(p.Y).IsCloseTo(4.4f, 1e-5);
        }

        [Test]
        public void NegTest()
        {
            var p = -p1;
            Check.That(p.X).IsCloseTo(-1.1f, 1e-5);
            Check.That(p.Y).IsCloseTo(-2.2f, 1e-5);
        }

        [Test]
        public void EqualsTest()
        {
            Check.That(p1 == p2).IsFalse();
            Check.That(p1 == null).IsFalse();
            Check.That(null == p2).IsFalse();
            Check.That(p1 == p1).IsTrue();
            Check.That(p1 == new Point2F(p1)).IsTrue();

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
            Check.That(p1 != new Point2F(p1)).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Check.That(p1.GetHashCode()).IsEqualTo(new Point2F(p1).GetHashCode());
            Check.That(p1.GetHashCode()).Not.IsEqualTo(p2.GetHashCode());
        }

        [Test]
        public void DivTest()
        {
            var p = p1 / 2;
            Check.That(p.X).IsEqualTo(0.55f);
            Check.That(p.Y).IsEqualTo(1.10f);
        }

        [Test]
        public void DistanceTest()
        {
            Check.That(p1.Distance(p1)).IsZero();
            Check.That(p1.Distance(p1 + new Point2F(1, 1))).IsCloseTo(MathF.Sqrt(2), 1e-5);
        }

        [Test]
        public void DistanceSquaredTest()
        {
            Check.That(p1.DistanceSquared(p1)).IsZero();
            Check.That(p1.DistanceSquared(p1 + new Point2F(-1, -1))).IsCloseTo(2, 1e-5);
        }

        [Test]
        public void MinTest()
        {
            var p = Point2F.Min(new Point2F(1, 2), new Point2F(-1, 4));
            Check.That(p.X).IsEqualTo(-1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void MaxTest()
        {
            var p = Point2F.Max(new Point2F(1, 2), new Point2F(-1, 4));
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(4);
        }

        [Test]
        public void AbsTest()
        {
            var p = new Point2F(-1, 4).Abs();
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(4);
        }

        [Test]
        public void FloorTest()
        {
            var p = p1.Floor();
            Check.That(p.X).IsEqualTo(1);
            Check.That(p.Y).IsEqualTo(2);
        }

        [Test]
        public void CeilTest()
        {
            var p = p1.Ceil();
            Check.That(p.X).IsEqualTo(2);
            Check.That(p.Y).IsEqualTo(3);
        }

        [Test]
        public void LerpTest()
        {
            var p = Point2F.Lerp(0.5f, p1, p2);
            Check.That(p.X).IsEqualTo(2.75f);
            Check.That(p.Y).IsEqualTo(3.85f);

            p = Point2F.Lerp(0, p1, p2);
            Check.That(p).IsEqualTo(p1);
            p = Point2F.Lerp(1, p1, p2);
            Check.That(p).IsEqualTo(p2);
        }

        [Test]
        public void ToStringTest()
        {
            Check.That(new Point2F(1, 2).ToString()).IsEqualTo("px[1] py[2]");
        }
    }
}