using NFluent;
using NUnit.Framework;
using pbrt.Accelerators;
using pbrt.Core;

namespace Pbrt.Tests.Accelerators
{
    [TestFixture]
    public class BVHPrimitiveInfoTests
    {
        [Test]
        public void BasicTest()
        {
            Bounds3F bounds = new Bounds3F(Point3F.Zero, new Point3F(1, 2, 3));
            BVHPrimitiveInfo info = new BVHPrimitiveInfo(123, bounds);
            Check.That(info.PrimitiveNumber).IsEqualTo(123);
            Check.That(info.Bounds).IsSameReferenceAs(bounds);
            Check.That(info.Centroid).IsEqualTo(new Point3F(0.5f, 1f, 1.5f));
        }
    }
}