using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Vector2ITests
    {
        private readonly Vector2I v1 = new Vector2I(1, 2);
        private readonly Vector2I v2 = new Vector2I(4, 5);
        private readonly Vector2I v3 = new Vector2I(-1, 0);

        [Test]
        public void ConstructorTests()
        {
            var v = new Vector2I(1, 2);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
        }

        [Test]
        public void ConstructorVectorTests()
        {
            var v = new Vector2I(v1);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
        }

        [Test]
        public void ConstructorPointTests()
        {
            var p = new Point2I(1, 2);
            var v = new Vector2I(p);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
        }

        [Test]
        public void IndexTests()
        {
            Check.That(v1[0]).IsEqualTo(1);
            Check.That(v1[1]).IsEqualTo(2);
            Check.ThatCode(() => v1[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => v1[2]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void OperatorAddTest()
        {
            var v = v1 + v2;
            Check.That(v.X).IsEqualTo(5);
            Check.That(v.Y).IsEqualTo(7);
        }

        [Test]
        public void OperatorSubTest()
        {
            var v = v1 - v2;
            Check.That(v.X).IsEqualTo(-3);
            Check.That(v.Y).IsEqualTo(-3);
        }

        [Test]
        public void OperatorNegTest()
        {
            var v = -v1;
            Check.That(v.X).IsEqualTo(-1);
            Check.That(v.Y).IsEqualTo(-2);
        }

        [Test]
        public void OperatorMulTest()
        {
            var v = 3 * v1;
            Check.That(v.X).IsEqualTo(3);
            Check.That(v.Y).IsEqualTo(6);

            v = v1 * 3;
            Check.That(v.X).IsEqualTo(3);
            Check.That(v.Y).IsEqualTo(6);
        }

        [Test]
        public void OperatorDivTest()
        {
            var v = v1 / 2;
            Check.That(v.X).IsEqualTo(0);
            Check.That(v.Y).IsEqualTo(1);
        }

        [Test]
        public void LengthTest()
        {
            Check.That(v1.Length).IsEqualTo(MathF.Sqrt(1 + 2*2));
            Check.That(v1.LengthSquared).IsEqualTo(1 + 4 );
        }

        [Test]
        public void AbsTest()
        {
            var v = new Vector2I(-2, -3);
            var absV = v.Abs();
            Check.That(absV.X).IsEqualTo(2);
            Check.That(absV.Y).IsEqualTo(3);
        }

        [Test]
        public void DotVectorTest()
        {
            Check.That(v1.Dot(v2)).IsEqualTo(1 * 4 + 2 * 5);
            Check.That(v1.Dot(v3)).IsEqualTo(1 * -1 + 2 * 0);
        }

        [Test]
        public void AbsDotVectorTest()
        {
            Check.That(v2.Dot(-v1)).IsEqualTo(-1 * 4  -2 * 5);
            Check.That(v2.AbsDot(-v1)).IsEqualTo(Math.Abs(v2.Dot(v1)));
        }

        [Test]
        public void EqualsTest()
        {
            Check.That(v1).IsEqualTo(v1);
            var v = new Vector2I(v1);
            Check.That(v1 == v).IsTrue();
            Check.That(v1 == null).IsFalse();
            Check.That(null == v1).IsFalse();
            Check.That(v1 == v1).IsTrue();
            Check.That(v2 != v1).IsTrue();
            Check.That(v2 == v1).IsFalse();

            Check.That(v1.Equals(v)).IsTrue();
            Check.That(v1.Equals(null)).IsFalse();
            Check.That(v1.Equals(v1)).IsTrue();
            Check.That(v1.Equals(5)).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Check.That(v1.GetHashCode() == v2.GetHashCode()).IsFalse();
            var v = new Vector2I(v1);
            Check.That(v1.GetHashCode() == v.GetHashCode()).IsTrue();
        }
    }
}