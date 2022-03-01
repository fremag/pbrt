using System;
using NFluent;
using NUnit.Framework;
using pbrt.Accelerators;
using pbrt.Core;

namespace Pbrt.Tests.Accelerators
{
    [TestFixture]
    public class AggregateTests
    {
        private class MyAggregate : Aggregate
        {
            public override Bounds3F WorldBound() => null;
            public override bool IntersectP(Ray r) => false;
            public override bool Intersect(Ray r, out SurfaceInteraction isect)
            {
                isect = null;
                return false;
            }
        }

        [Test]
        public void DummyCoverageOnlyTest()
        {
            MyAggregate agg = new MyAggregate();
            Check.That(agg.WorldBound()).IsNull();
            Check.That(agg.IntersectP(null)).IsFalse();
            Check.That(agg.Intersect(null, out _)).IsFalse();
        }
        
        [Test]
        public void GetAreaLightTest()
        {
            var dummy = new MyAggregate();
            Check.ThatCode(() => dummy.GetAreaLight()).Throws<NotImplementedException>();
        }

        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            var dummy = new MyAggregate();
            Check.ThatCode(() => dummy.ComputeScatteringFunctions(null, null, null, false)).Throws<NotImplementedException>();
        }

        [Test]
        public void GetMaterialTest()
        {
            var dummy = new MyAggregate();
            Check.ThatCode(() => dummy.GetMaterial()).Throws<NotImplementedException>();
        }
    }
}