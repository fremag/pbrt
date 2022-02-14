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
        }
        
        [Test]
        public void NextFloatDownTest()
        {
            var f = 1.23f;
            var nextF = MathUtils.NextFloatDown(f);
            var delta = nextF-f;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyNegative();
        }
    }
}