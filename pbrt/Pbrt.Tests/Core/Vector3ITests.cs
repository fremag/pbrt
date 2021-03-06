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
    public class Vector3ITests
    {
        private readonly Vector3I v1 = new Vector3I(1, 2, 3);
        private readonly Vector3I v2 = new Vector3I(4, 5, 6);
        private readonly Vector3I v3 = new Vector3I(-1, 0, 1);

        [Test]
        public void ConstructorTests()
        {
            var v = new Vector3I(1, 2, 3);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
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
            var v = new Vector3I(-2, -3, -4);
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
            Check.That(new Vector3I(1, 2, 3).MaxDimension).IsEqualTo(2);
            Check.That(new Vector3I(3, 2, 1).MaxDimension).IsEqualTo(0);
            Check.That(new Vector3I(2, 3, 1).MaxDimension).IsEqualTo(1);
        }

        [Test]
        public void DotVectorTest()
        {
            Check.That(v1.Dot(v2)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(v1.Dot(v3)).IsEqualTo(1 * -1 + 2 * 0 + 3 * 1);
        }

        [Test]
        public void AbsDotVectorTest()
        {
            Check.That(v2.Dot(-v1)).IsEqualTo(-1 * 4  -2 * 5 - 3 * 6);
            Check.That(v2.AbsDot(-v1)).IsEqualTo(Math.Abs(v2.Dot(v1)));
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
            var v = Vector3I.Min(v1, v2);
            Check.That(v.X).IsEqualTo(1);
            Check.That(v.Y).IsEqualTo(2);
            Check.That(v.Z).IsEqualTo(3);
            
            v = Vector3I.Min(v1, v3);
            Check.That(v.X).IsEqualTo(-1);
            Check.That(v.Y).IsEqualTo(0);
            Check.That(v.Z).IsEqualTo(1);
        }
        
        [Test]
        public void MaxTest()
        {
            var v = Vector3I.Max(v1, v2);
            Check.That(v.X).IsEqualTo(4);
            Check.That(v.Y).IsEqualTo(5);
            Check.That(v.Z).IsEqualTo(6);
            
            v = Vector3I.Max(v1, v3);
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
            var v = new Vector3I(1, 2, 3);
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
            var v = new Vector3I(v1.X, v1.Y, v1.Z);
            Check.That(v1.GetHashCode() == v.GetHashCode()).IsTrue();
        }
        
        [Test]
        public void CrossVectorTest()
        {
            var v = v1.Cross(v2);
            Check.That(v.X).IsEqualTo(v1.Y*v2.Z - v1.Z * v2.Y);
            Check.That(v.Y).IsEqualTo(v1.Z*v2.X - v1.X * v2.Z);
            Check.That(v.Z).IsEqualTo(v1.X*v2.Y - v1.Y * v2.X);
        }
    }
}