using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Vector3FTests
    {
        private readonly Vector3F v1 = new Vector3F(1, 2, 3);
        private readonly Vector3F v2 = new Vector3F(4, 5, 6);
        private readonly Vector3F v3 = new Vector3F(-1, 0, 1);

        [Test]
        public void ConstructorTests()
        {
            var v = new Vector3F(1, 2, 3);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
        }

        [Test]
        public void ConstructorVectorTests()
        {
            var v = new Vector3F(v1);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
        }

        [Test]
        public void ConstructorPointTests()
        {
            var p = new Point3F(1, 2, 3);
            var v = new Vector3F(p);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
        }

        [Test]
        public void ConstructorNormalTests()
        {
            var n = new Normal3F(1, 2, 3);
            var v4 = new Vector3F(n);
            Check.That(v4.X).IsEqualTo(1);
            Check.That(v4.Y).IsEqualTo(2);
            Check.That(v4.Z).IsEqualTo(3);
        }

        [Test]
        public void IndexTests()
        {
            Check.That(v1[0]).IsEqualTo(1);
            Check.That(v1[1]).IsEqualTo(2);
            Check.That(v1[2]).IsEqualTo(3);
            Check.ThatCode(() => v1[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => v1[3]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void OperatorAddTest()
        {
            var v = v1 + v2;
            Check.That(v.X).IsEqualTo(5);
            Check.That(v.Y).IsEqualTo(7);
            Check.That(v.Z).IsEqualTo(9);
        }

        [Test]
        public void OperatorSubTest()
        {
            var v = v1 - v2;
            Check.That(v.X).IsEqualTo(-3);
            Check.That(v.Y).IsEqualTo(-3);
            Check.That(v.Z).IsEqualTo(-3);
        }

        [Test]
        public void OperatorNegTest()
        {
            var v = -v1;
            Check.That(v.X).IsEqualTo(-1);
            Check.That(v.Y).IsEqualTo(-2);
            Check.That(v.Z).IsEqualTo(-3);
        }

        [Test]
        public void OperatorMulTest()
        {
            var v = 3 * v1;
            Check.That(v.X).IsEqualTo(3);
            Check.That(v.Y).IsEqualTo(6);
            Check.That(v.Z).IsEqualTo(9);

            v = v1 * 3;
            Check.That(v.X).IsEqualTo(3);
            Check.That(v.Y).IsEqualTo(6);
            Check.That(v.Z).IsEqualTo(9);
        }

        [Test]
        public void OperatorDivTest()
        {
            var v = v1 / 2;
            Check.That(v.X).IsEqualTo(0.5);
            Check.That(v.Y).IsEqualTo(1);
            Check.That(v.Z).IsEqualTo(1.5);
        }

        [Test]
        public void LengthTest()
        {
            Check.That(v1.Length).IsEqualTo(MathF.Sqrt(1 + 2*2 + 3*3));
            Check.That(v1.LengthSquared).IsEqualTo(1 + 4 + 9);
        }

        [Test]
        public void AbsTest()
        {
            var v = new Vector3F(-2, -3, -4);
            var absV = v.Abs();
            Check.That(absV.X).IsEqualTo(2);
            Check.That(absV.Y).IsEqualTo(3);
            Check.That(absV.Z).IsEqualTo(4);
        }

        [Test]
        public void MinMaxComponentTest()
        {
            Check.That(v1.MinComponent).IsEqualTo(1);
            Check.That(v1.MaxComponent).IsEqualTo(3);
        }

        [Test]
        public void MaxDimensionTest()
        {
            Check.That(new Vector3F(1, 2, 3).MaxDimension).IsEqualTo(2);
            Check.That(new Vector3F(3, 2, 1).MaxDimension).IsEqualTo(0);
            Check.That(new Vector3F(2, 3, 1).MaxDimension).IsEqualTo(1);
        }

        [Test]
        public void HasNaNsTest()
        {
            Check.That(v1.HasNaNs).IsFalse();
            Check.That(new Vector3F(float.NaN, 2, 3).HasNaNs).IsTrue();
            Check.That(new Vector3F(1, float.NaN, 3).HasNaNs).IsTrue();
            Check.That(new Vector3F(1, 2, float.NaN).HasNaNs).IsTrue();
        }

        [Test]
        public void DotVectorTest()
        {
            Check.That(v1.Dot(v2)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(v1.Dot(v3)).IsEqualTo(1 * -1 + 2 * 0 + 3 * 1);
        }

        [Test]
        public void DotNormalTest()
        {
            var n = new Normal3F(4, 5, 6);
            Check.That(v1.Dot(n)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(Vector3F.Dot(v1, n)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(Vector3F.Dot(n, v1)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
        }
        
        [Test]
        public void AbsDotVectorTest()
        {
            Check.That(v2.Dot(-v1)).IsEqualTo(-1 * 4  -2 * 5 - 3 * 6);
            Check.That(v2.AbsDot(-v1)).IsEqualTo(Math.Abs(v2.Dot(v1)));
        }

        [Test]
        public void AbsDotNormalTest()
        {
            var n = new Normal3F(4, 5, 6);
            Check.That(-v1.Dot(n)).IsEqualTo(-1 * 4 - 2 * 5 - 3 * 6);
            Check.That(v1.AbsDot(n)).IsEqualTo(Math.Abs(-v1.Dot(n)));
            Check.That(Vector3F.AbsDot(n, -v1)).IsEqualTo(Math.Abs(-v1.Dot(n)));
        }

        [Test]
        public void CrossVectorTest()
        {
            var v = v1.Cross(v2);
            Check.That(v.X).IsEqualTo(v1.Y*v2.Z - v1.Z * v2.Y);
            Check.That(v.Y).IsEqualTo(v1.Z*v2.X - v1.X * v2.Z);
            Check.That(v.Z).IsEqualTo(v1.X*v2.Y - v1.Y * v2.X);
        }

        [Test]
        public void NormalizeTest()
        {
            var norm = v1.Normalized();
            var length = MathF.Sqrt(1 + 2 * 2 + 3 * 3);
            Check.That(norm.X).IsEqualTo(1 / length);
            Check.That(norm.Y).IsEqualTo(2 / length);
            Check.That(norm.Z).IsEqualTo(3 / length);
        }

        [Test]
        public void MinTest()
        {
            var v = Vector3F.Min(v1, v2);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
            
            v = Vector3F.Min(v1, v3);
            Check.That(v.X).IsEqualTo(-1);
            Check.That(v.Y).IsEqualTo(0);
            Check.That(v.Z).IsEqualTo(1);
        }
        
        [Test]
        public void MaxTest()
        {
            var v = Vector3F.Max(v1, v2);
            Check.That(v.X).IsEqualTo(4);
            Check.That(v.Y).IsEqualTo(5);
            Check.That(v.Z).IsEqualTo(6);
            
            v = Vector3F.Max(v1, v3);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
        }
        
        [Test]
        public void PermuteTest()
        {
            var v = v2.Permute(2, 0, 1);
            Check.That(v.X).IsEqualTo(6);
            Check.That(v.Y).IsEqualTo(4);
            Check.That(v.Z).IsEqualTo(5);
        }
        
        [Test]
        public void EqualsTest()
        {
            Check.That(v1).IsEqualTo(v1);
            var v = new Vector3F(v1);
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
            var v = new Vector3F(v1);
            Check.That(v1.GetHashCode() == v.GetHashCode()).IsTrue();
        }

        [Test]
        public void FaceforwardTest()
        {
            var n = new Normal3F(1, 2, 3);
            var normal3F = Vector3F.Faceforward(n, v1);
            Check.That(normal3F).IsEqualTo(n);
            Check.That(Vector3F.Faceforward(n, -v1)).IsEqualTo(-n);
        }

        [Test]
        public void CoordinateSystemTest()
        {
            var x = new Vector3F(1, 0, 0);
            Vector3F.CoordinateSystem(x, out var x1, out var x2);
            Check.That(x1).IsEqualTo(new Vector3F(0, 0, 1));
            Check.That(x2).IsEqualTo(new Vector3F(0, -1, 0));

            var y = new Vector3F(0, 1, 0);
            Vector3F.CoordinateSystem(y, out var y1, out var y2);
            Check.That(y1).IsEqualTo(new Vector3F(0, 0, -1));
            Check.That(y2).IsEqualTo(new Vector3F(-1, 0, 0));
        }
    }
}