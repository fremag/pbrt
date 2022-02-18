using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests
{
    public class MathUtilsTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void FloatToBitsToFloatTest()
        {
            float f = 1.23f;
            var ui = MathUtils.FloatToBits(f);
            var ff = MathUtils.BitsToFloat(ui);
            Check.That(ff).IsEqualTo(f);
        }
        
        [Test]
        public void DoubleToBitsToDoubleTest()
        {
            double d = 1.23;
            ulong ul = MathUtils.DoubleToBits(d);
            double dd = MathUtils.BitsToDouble(ul);
            Check.That(dd).IsEqualTo(d);
        }

        [Test]
        public void NextFloatUpTest()
        {
            var f = 1.23f;
            var nextF = MathUtils.NextFloatUp(f);
            var delta = nextF-f;
            Check.That(delta).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyPositive();

            var nextFloatDown = MathUtils.NextFloatUp(float.PositiveInfinity);
            Check.That(float.IsPositiveInfinity(nextFloatDown)).IsTrue();
            Check.That(MathUtils.NextFloatUp(0f)).IsEqualTo(float.Epsilon);

            Check.That(MathUtils.NextFloatUp(-float.Epsilon)).IsZero();
        }
        
        [Test]
        public void NextFloatDownTest()
        {
            var f = 1.23f;
            var nextF = MathUtils.NextFloatDown(f);
            var delta = nextF-f;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyNegative();

            var nextFloatDown = MathUtils.NextFloatDown(float.NegativeInfinity);
            Check.That(float.IsNegativeInfinity(nextFloatDown)).IsTrue();
            Check.That(MathUtils.NextFloatDown(0f)).IsEqualTo(-float.Epsilon);
        }
        [Test]
        public void NextDoubletUpTest()
        {
            var d = 1.23d;
            var nextF = MathUtils.NextDoubleUp(d);
            var delta = nextF-d;
            Check.That(delta).IsLessOrEqualThan(1e-6d);
            Check.That(delta).IsStrictlyPositive();

            var nextDoubleDown = MathUtils.NextDoubleUp(double.PositiveInfinity);
            Check.That(double.IsPositiveInfinity(nextDoubleDown)).IsTrue();
            Check.That(MathUtils.NextDoubleUp(0f)).IsEqualTo(double.Epsilon);

            Check.That(MathUtils.NextDoubleUp(-double.Epsilon)).IsZero();
        }
        
        [Test]
        public void NextDoubleDownTest()
        {
            var d = 1.23f;
            var nextF = MathUtils.NextDoubleDown(d);
            var delta = nextF-d;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6d);
            Check.That(delta).IsStrictlyNegative();

            var nextDoubleDown = MathUtils.NextDoubleDown(double.NegativeInfinity);
            Check.That(Double.IsNegativeInfinity(nextDoubleDown)).IsTrue();
            Check.That(MathUtils.NextDoubleDown(0f)).IsEqualTo(-double.Epsilon);
        }

        [Test]
        public void GammaTest()
        {
            Check.That(MathUtils.Gamma(0)).IsEqualTo(0);
            Check.That(MathUtils.Gamma(1)).IsEqualTo(float.Epsilon);
            Check.That(MathUtils.Gamma(3)).IsEqualTo(4e-45f);
        }

        [Test]
        public void ClampTest()
        {
            Check.That(0f.Clamp( -1, 1)).IsEqualTo(0);
            Check.That((-5f).Clamp( -1, 1)).IsEqualTo(-1);
            Check.That(5f.Clamp(-1, 1)).IsEqualTo(1);
        }

        [Test]
        public void DegTest()
        {
            Check.That(MathF.PI.Degrees()).IsEqualTo(180);
            Check.That((2*MathF.PI).Degrees()).IsEqualTo(360);
            Check.That(0f.Degrees()).IsEqualTo(0);
        }

        [Test]
        public void RadTest()
        {
            Check.That(180f.Radians()).IsEqualTo(MathF.PI);
            Check.That((360f).Radians()).IsEqualTo(MathF.PI*2);
            Check.That(0f.Radians()).IsEqualTo(0);
        }

        [Test]
        [TestCase(1, 1, 1, 0, 0, false)]
        [TestCase(1, 0, 1, 0, 0, false)]
        [TestCase(1, -4, 3, 1, 3, true)]
        [TestCase(-1, 4, -3, 1, 3, true)]
        public void QuadraticTest(float a, float b, float c, float expectedT0, float expectedT1, bool expectedResult)
        {
            var result = MathUtils.Quadratic(a, b, c, out var t0, out var t1);
            Check.That(result).IsEqualTo(expectedResult);
            Check.That(t0).IsEqualTo(expectedT0);
            Check.That(t1).IsEqualTo(expectedT1);
        }

    }
}