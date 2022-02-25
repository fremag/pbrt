using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable HeapView.BoxingAllocation
#pragma warning disable CS1718 

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class EFloatTests
    {
        [Test]
        public void ConstructorNoErrorTest()
        {
            var emptyEf = new EFloat();
            Check.That(emptyEf.V).IsEqualTo(0f);
            Check.That(emptyEf.Low).IsEqualTo(0f);
            Check.That(emptyEf.High).IsEqualTo(0f);

            var ef = new EFloat(1.23f);
            Check.That(ef.V).IsEqualTo(1.23f);
            Check.That(ef.Low).IsEqualTo(1.23f);
            Check.That(ef.High).IsEqualTo(1.23f);
        }

        [Test]
        public void ConstructorCopyTest()
        {
            EFloat ef0 = new EFloat(1.23f, 0.01f);
            EFloat ef = new EFloat(ef0);
            Check.That(ef.V).IsEqualTo(1.23f);
            Check.That(ef.Low).IsEqualTo(1.2199999f);
            Check.That(ef.High).IsEqualTo(1.2400001f);
        }

        [Test]
        public void ConstructorErrorTest()
        {
            EFloat ef = new EFloat(1.23f, 0.01f);
            Check.That(ef.V).IsEqualTo(1.23f);
            Check.That(ef.Low).IsEqualTo(1.2199999f);
            Check.That(ef.High).IsEqualTo(1.2400001f);
        }

        [Test]
        public void ConstructorHighLowTest()
        {
            EFloat ef = new EFloat(1.23f, 1.24f, 1.22f);
            Check.That(ef.V).IsEqualTo(1.23f);
            Check.That(ef.Low).IsEqualTo(1.22f);
            Check.That(ef.High).IsEqualTo(1.24f);
            Check.That(ef.LowerBound()).IsEqualTo(1.22f);
            Check.That(ef.UpperBound()).IsEqualTo(1.24f);
        }

        [Test]
        public void CheckTest()
        {
            var ef = new EFloat(1, 1.1f, float.PositiveInfinity);
            Check.ThatCode(() => ef.Check()).DoesNotThrow();

            ef = new EFloat(1, float.PositiveInfinity, 1.1f);
            Check.ThatCode(() => ef.Check()).DoesNotThrow();

            ef = new EFloat(1, 1.1f, float.NaN);
            Check.ThatCode(() => ef.Check()).DoesNotThrow();

            ef = new EFloat(1, float.NaN, 1.1f);
            Check.ThatCode(() => ef.Check()).DoesNotThrow();

            ef = new EFloat(1, -0.001f);
            Check.ThatCode(() => ef.Check()).Throws<Exception>();
        }

        [Test]
        public void EqualsTest()
        {
            var ef1 = new EFloat(1.23f);
            var ef2 = new EFloat(1.23f);
            var ef3 = new EFloat(2.34f);

            Check.That(ef1.Equals(ef2)).IsTrue();
            Check.That(ef1.Equals(ef1)).IsTrue();
            Check.That(ef1.Equals(ef3)).IsFalse();
            Check.That(ef1.Equals(null)).IsFalse();
            Check.That(ef1.Equals(5)).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            var ef1 = new EFloat(1.23f);
            var ef2 = new EFloat(1.23f);
            var ef3 = new EFloat(2.34f);
            Check.That(ef1.GetHashCode()).IsEqualTo(ef2.GetHashCode());
            Check.That(ef1.GetHashCode()).IsNotEqualTo(ef3.GetHashCode());
        }

        [Test]
        public void GetAbsoluteErrorTest()
        {
            var ef = new EFloat(1.23f, 0.01f);
            Check.That(ef.GetAbsoluteError()).IsGreaterOrEqualThan(0.01f);
            Check.That(ef.GetAbsoluteError()).IsLessOrEqualThan(0.010001f);
        }

        [Test]
        public void ToStringTest()
        {
            var ef = new EFloat(1.23f, 0.01f);
            Check.That(ef.ToString()).IsEqualTo("V[1.23] High[1.2400001] Low[1.2199999]");
        }

        [Test]
        public void CastOperatorTest()
        {
            var ef = new EFloat(1.23f);
            var f = (float)ef;
            Check.That(f).IsEqualTo(1.23f);

            var d = (double)ef;
            Check.That(d).IsCloseTo(1.23d, 1e-7);

            ef = 4.56f;
            Check.That(ef).IsEqualTo(new EFloat(4.56f));
            ef = 5.67d;
            Check.That(ef).IsEqualTo(new EFloat(5.67d));
        }

        [Test]
        public void AdditionTest()
        {
            var ef1 = new EFloat(1.23f, 0.01f);
            var ef2 = new EFloat(2.34f, 0.01f);

            var ef3 = ef1 + ef2;
            Check.That(ef3.V).IsEqualTo(3.57f);
            Check.That(ef3.High).IsCloseTo(3.59f, 1e-4);
            Check.That(ef3.Low).IsCloseTo(3.549f, 1e-3);

            var ef4 = ef1 + 2.34f;
            Check.That(ef4.V).IsEqualTo(3.57f);
            Check.That(ef4.High).IsCloseTo(3.58f, 1e-4);
            Check.That(ef4.Low).IsCloseTo(3.559f, 1e-3);

            var ef5 = 2.34f + ef1;
            Check.That(ef5).IsEqualTo(ef4);
        }

        [Test]
        public void SubtractionTest()
        {
            var ef1 = new EFloat(1.23f, 0.01f);
            var ef2 = new EFloat(1.23f, 0.01f);

            var ef3 = ef1 - ef2;
            Check.That(ef3.V).IsEqualTo(0f);
            Check.That(ef3.High).IsCloseTo(0.02f, 1e-4);
            Check.That(ef3.Low).IsCloseTo(-0.020f, 1e-4);

            var ef4 = ef1 - 1.23f;
            Check.That(ef4.V).IsEqualTo(0f);
            Check.That(ef4.High).IsCloseTo(0.01f, 1e-4);
            Check.That(ef4.Low).IsCloseTo(-0.0101f, 1e-4);

            var ef5 = 1.23f - ef1;
            Check.That(ef5).IsEqualTo(ef4);
        }

        [Test]
        public void MultiplicationTest()
        {
            var ef1 = new EFloat(2f, 0.01f);
            var ef2 = new EFloat(3f, 0.01f);

            var ef3 = ef1 * ef2;
            Check.That(ef3.V).IsEqualTo(6f);
            Check.That(ef3.High).IsCloseTo(6.05f, 1e-3);
            Check.That(ef3.Low).IsCloseTo(5.95f, 1e-4);

            var ef4 = ef1 * 3f;
            Check.That(ef4.V).IsEqualTo(6f);
            Check.That(ef4.High).IsCloseTo(6.03f, 1e-4);
            Check.That(ef4.Low).IsCloseTo(5.969f, 1e-3);

            var ef5 = 3f * ef1;
            Check.That(ef5).IsEqualTo(ef4);
        }

        [Test]
        public void DivisionTest()
        {
            var ef1 = new EFloat(3f, 0.01f);
            var ef2 = new EFloat(2f, 0.01f);

            var ef3 = ef1 / ef2;
            Check.That(ef3.V).IsEqualTo(1.5f);
            Check.That(ef3.High).IsCloseTo(1.512f, 1e-3);
            Check.That(ef3.Low).IsCloseTo(1.487f, 1e-3);

            var ef4 = ef1 / 2f;
            Check.That(ef4.V).IsEqualTo(1.5f);
            Check.That(ef4.High).IsCloseTo(1.505f, 1e-4);
            Check.That(ef4.Low).IsCloseTo(1.494f, 1e-3);

            var ef5 = 3f / ef2;
            Check.That(ef5.V).IsEqualTo(1.5f);
            Check.That(ef5.High).IsCloseTo(1.507f, 1e-3f);
            Check.That(ef5.Low).IsCloseTo(1.492f, 1e-3f);

            var ef6 = new EFloat(0f, 0.1f, -0.1f);
            var ef7 = 1 / ef6;
            Check.That(ef7).IsEqualTo(new EFloat(float.PositiveInfinity, float.PositiveInfinity, float.NegativeInfinity));
        }

        [Test]
        public void UnaryNegationTest()
        {
            var ef = new EFloat(1.23f, 0.01f);
            var neg = -ef;
            Check.That(neg).IsEqualTo(new EFloat(-1.23f, 0.01f));
        }

        [Test]
        public void EqualityOperatorTest()
        {
            var ef1 = (EFloat)1.23f;
            var ef2 = new EFloat(1.23f, 0.01f);
            var ef3 = new EFloat(1.23f, 0.001f);
            var ef4 = new EFloat(1.24f, 0.001f);

            Check.That(ef1 == ef2).IsTrue();
            Check.That(ef1 == ef1).IsTrue();
            Check.That(ef1 == ef3).IsTrue();
            Check.That(ef1 == ef4).IsFalse();
            Check.That(ef1 == null).IsFalse();
            Check.That(null == ef1).IsFalse();
            
            Check.That(ef1 != ef2).IsFalse();
            Check.That(ef1 != ef1).IsFalse();
            Check.That(ef1 != ef3).IsFalse();
            Check.That(ef1 != ef4).IsTrue();
            Check.That(ef1 != null).IsTrue();
            Check.That(null != ef1).IsTrue();
        }

        [Test]
        public void SqrtTest()
        {
            var ef = new EFloat(4, 0.01f);
            var sqrt = ef.Sqrt();
            Check.That(sqrt.V).IsEqualTo(2f);
            Check.That(sqrt.High).IsCloseTo(2.002f, 1e-3f);
            Check.That(sqrt.Low).IsCloseTo(1.997f, 1e-3f);
        }
        
        [Test]
        public void AbsTest()
        {
            var ef = new EFloat(4, 0.01f);
            var abs = ef.Abs();
            Check.That(abs).IsSameReferenceAs(ef);

            ef = new EFloat(-0.001f, 0.01f);
            abs = ef.Abs();
            
            Check.That(abs.V).IsEqualTo(0.001f);
            Check.That(abs.High).IsCloseTo(0.011f, 1e-3f);
            Check.That(abs.Low).IsCloseTo(0.00f, 1e-3f);

            ef = new EFloat(-1f, 0.01f);
            abs = ef.Abs();
            
            Check.That(abs.V).IsEqualTo(1f);
            Check.That(abs.High).IsCloseTo(1.01f, 1e-3f);
            Check.That(abs.Low).IsCloseTo(0.99f, 1e-3f);
        }
        
        [Test]
        public void Quadratic_DiscrimNegativeTest()
        {
            var a = new EFloat(1, 0.01f);
            var b = new EFloat(1, 0.01f);
            var c = new EFloat(1, 0.01f);
            var result = EFloat.Quadratic(a, b, c, out var e, out var f);
            Check.That(result).IsFalse();
            Check.That(e).IsNull();
            Check.That(f).IsNull();
        }

        [Test]
        public void QuadraticTest()
        {
            var a = new EFloat(1, 0.01f);
            var b = new EFloat(-4, 0.01f);
            var c = new EFloat(3, 0.01f);
            var result = EFloat.Quadratic(a, b, c, out var e, out var f);
            Check.That(result).IsTrue();
            
            Check.That(e.V).IsCloseTo(1, 1e-3);
            Check.That(e.High).IsCloseTo(1.005, 1e-3);
            Check.That(e.Low).IsCloseTo(0.995, 1e-3);
            Check.That(f.V).IsCloseTo(3, 1e-3);
            Check.That(f.High).IsCloseTo(3.035, 1e-3);
            Check.That(f.Low).IsCloseTo(2.965, 1e-3);
        }
        
        [Test]
        public void AnotherQuadraticTest()
        {
            var a = new EFloat(-1, 0.01f);
            var b = new EFloat(4, 0.01f);
            var c = new EFloat(-3, 0.01f);
            var result = EFloat.Quadratic(a, b, c, out var e, out var f);
            Check.That(result).IsTrue();
            
            Check.That(e.V).IsCloseTo(1, 1e-3);
            Check.That(e.High).IsCloseTo(1.005, 1e-3);
            Check.That(e.Low).IsCloseTo(0.995, 1e-3);
            Check.That(f.V).IsCloseTo(3, 1e-3);
            Check.That(f.High).IsCloseTo(3.035, 1e-3);
            Check.That(f.Low).IsCloseTo(2.965, 1e-3);
        }
    }
}