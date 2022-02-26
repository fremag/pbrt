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
    public class Normal3FTests
    {
        private readonly Normal3F n1 = new Normal3F(1, 2, 3);
        private readonly Normal3F n2 = new Normal3F(4, 5, 6);
        private readonly Normal3F n3 = new Normal3F(-1, 0, 1);

        [Test]
        public void ConstructorTests()
        {
            var n = new Normal3F(1, 2, 3);
            Check.That(n.X).IsEqualTo(1);
            Check.That(n.Y).IsEqualTo(2);
            Check.That(n.Z).IsEqualTo(3);

            n = new Normal3F();
            Check.That(n.X).IsEqualTo(0);
            Check.That(n.Y).IsEqualTo(0);
            Check.That(n.Z).IsEqualTo(0);
        }

        [Test]
        public void ConstructorVectorTests()
        {
            var v = new Vector3F(1,2,3);
            var n = new Normal3F(v);
            Check.That(n.X).IsEqualTo(1);
            Check.That(n.Y).IsEqualTo(2);
            Check.That(n.Z).IsEqualTo(3);
        }

        [Test]
        public void ConstructorNormalTests()
        {
            var n = new Normal3F(1, 2, 3);
            var n4 = new Normal3F(n);
            Check.That(n4.X).IsEqualTo(1);
            Check.That(n4.Y).IsEqualTo(2);
            Check.That(n4.Z).IsEqualTo(3);
        }

        [Test]
        public void IndexTests()
        {
            Check.That(n1[0]).IsEqualTo(1);
            Check.That(n1[1]).IsEqualTo(2);
            Check.That(n1[2]).IsEqualTo(3);
            Check.ThatCode(() => n1[-1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => n1[3]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void OperatorAddTest()
        {
            var n = n1 + n2;
            Check.That(n.X).IsEqualTo(5);
            Check.That(n.Y).IsEqualTo(7);
            Check.That(n.Z).IsEqualTo(9);
        }

        [Test]
        public void OperatorSubTest()
        {
            var n = n1 - n2;
            Check.That(n.X).IsEqualTo(-3);
            Check.That(n.Y).IsEqualTo(-3);
            Check.That(n.Z).IsEqualTo(-3);
        }

        [Test]
        public void OperatorNegTest()
        {
            var n = -n1;
            Check.That(n.X).IsEqualTo(-1);
            Check.That(n.Y).IsEqualTo(-2);
            Check.That(n.Z).IsEqualTo(-3);
        }

        [Test]
        public void OperatorMulTest()
        {
            var n = 3 * n1;
            Check.That(n.X).IsEqualTo(3);
            Check.That(n.Y).IsEqualTo(6);
            Check.That(n.Z).IsEqualTo(9);

            n = n1 * 3;
            Check.That(n.X).IsEqualTo(3);
            Check.That(n.Y).IsEqualTo(6);
            Check.That(n.Z).IsEqualTo(9);
        }

        [Test]
        public void OperatorDivTest()
        {
            var n = n1 / 2;
            Check.That(n.X).IsEqualTo(0.5);
            Check.That(n.Y).IsEqualTo(1);
            Check.That(n.Z).IsEqualTo(1.5);
        }

        [Test]
        public void LengthTest()
        {
            Check.That(n1.Length).IsEqualTo(MathF.Sqrt(1 + 2*2 + 3*3));
            Check.That(n1.LengthSquared).IsEqualTo(1 + 4 + 9);
        }

        [Test]
        public void HasNaNsTest()
        {
            Check.That(n1.HasNaNs).IsFalse();
            Check.That(new Normal3F(float.NaN, 2, 3).HasNaNs).IsTrue();
            Check.That(new Normal3F(1, float.NaN, 3).HasNaNs).IsTrue();
            Check.That(new Normal3F(1, 2, float.NaN).HasNaNs).IsTrue();
        }

        [Test]
        public void DotVectorTest()
        {
            Check.That(n1.Dot(n2)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(n1.Dot(n3)).IsEqualTo(1 * -1 + 2 * 0 + 3 * 1);
        }

        [Test]
        public void DotNormalTest()
        {
            var n = new Normal3F(4, 5, 6);
            Check.That(n1.Dot(n)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
            Check.That(n.Dot(n1)).IsEqualTo(1 * 4 + 2 * 5 + 3 * 6);
        }
        
        [Test]
        public void AbsDotTest()
        {
            var v = new Vector3F(4, 5, 6);
            Check.That(-n1.AbsDot(v)).IsEqualTo(-1 * 4 - 2 * 5 - 3 * 6);
        }

        [Test]
        public void NormalizeTest()
        {
            var norm = n1.Normalize();
            var length = MathF.Sqrt(1 + 2 * 2 + 3 * 3);
            Check.That(norm.X).IsEqualTo(1 / length);
            Check.That(norm.Y).IsEqualTo(2 / length);
            Check.That(norm.Z).IsEqualTo(3 / length);
        }

        [Test]
        public void EqualsTest()
        {
            Check.That(n1).IsEqualTo(n1);
            var n = new Normal3F(n1);
            Check.That(n1 == n).IsTrue();
            Check.That(n1 == null).IsFalse();
            Check.That(null == n1).IsFalse();
            Check.That(n1 == n1).IsTrue();
            Check.That(n2 != n1).IsTrue();
            Check.That(n2 == n1).IsFalse();
            
            Check.That(n1.Equals(n)).IsTrue();
            Check.That(n1.Equals((string)null)).IsFalse();
            string s = null;
            Check.That(n1.Equals(s)).IsFalse();
            Check.That(n1.Equals(n1)).IsTrue();
            Check.That(n1.Equals(5)).IsFalse();
            Vector3F normNull = null;
            Check.That(n1.Equals(normNull)).IsFalse();
            Check.That(n1.Equals(n1)).IsTrue();
        }

        [Test]
        public void GetHashCodeTest()
        {
            Check.That(n1.GetHashCode() == n2.GetHashCode()).IsFalse();
            var n = new Normal3F(n1);
            Check.That(n1.GetHashCode() == n.GetHashCode()).IsTrue();
        }
        
        [Test]
        public void FaceForwardVectorTest()
        {
            var v = new Vector3F(1, 2, 3);
            var n = n1.FaceForward(v);
            Check.That(n).IsEqualTo(n1);
            
            n = n1.FaceForward(-v);
            Check.That(n).IsEqualTo(-n1);
        }

        [Test]
        public void FaceForwardNormalTest()
        {
            var nn = new Normal3F(1, 2, 3);
            var n = n1.FaceForward(nn);
            Check.That(n).IsEqualTo(n1);
            
            n = n1.FaceForward(-nn);
            Check.That(n).IsEqualTo(-n1);
        }
 
        [Test]
        public void  ToStringTest()
        {
            Check.That(new Normal3F(1.23f, 2.34f, 3.45f).ToString()).IsEqualTo("Nx[1.23] Ny[2.34] Nz[3.45]");
        }
        

        [Test]
        public void DeConstructorTest()
        {
            var n = (1f, 2f, 3f);
            var normal = (Normal3F)n;
            Check.That(normal.X).IsEqualTo(1);
            Check.That(normal.Y).IsEqualTo(2);
            Check.That(normal.Z).IsEqualTo(3);

            var (x, y, z) = normal;
            Check.That(x).IsEqualTo(1);
            Check.That(y).IsEqualTo(2);
            Check.That(z).IsEqualTo(3);
            
            Check.That(normal).IsEqualTo((1f, 2f, 3f));
            Check.That(normal).IsEqualTo((1, 2, 3));
        }
    }
}