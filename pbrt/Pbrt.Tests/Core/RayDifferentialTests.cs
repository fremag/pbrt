using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class RayDifferentialTests
    {
        private readonly Point3F origin = new Point3F(1, 0, 0);
        private readonly Vector3F direction = new Vector3F(0,1,0);
        private readonly float tMax = 1234;
        private readonly float time = 2.34f;
        private readonly Medium medium = new Medium();
        private RayDifferential rayDiff;
        
        [SetUp]
        public void SetUp()
        {
            rayDiff = new RayDifferential(origin, direction, tMax, time, medium);
        }

        [Test]
        public void BasicTest()
        {
            Check.That(rayDiff.O).IsEqualTo(origin);
            Check.That(rayDiff.D).IsEqualTo(direction);
            Check.That(rayDiff.TMax).IsEqualTo(tMax);
            Check.That(rayDiff.Time).IsEqualTo(time);
            Check.That(rayDiff.Medium).IsEqualTo(medium);
            Check.That(rayDiff.HasDifferentials).IsFalse();
        }

        [Test]
        public void ScaleDifferentialsTest()
        {
            Check.That(rayDiff.RxDirection).IsEqualTo(Vector3F.Zero);
            Check.That(rayDiff.RyDirection).IsEqualTo(Vector3F.Zero);
            Check.That(rayDiff.RxOrigin).IsEqualTo(Point3F.Zero);
            Check.That(rayDiff.RyOrigin).IsEqualTo(Point3F.Zero);
            
            rayDiff.ScaleDifferentials(10);
            Check.That(rayDiff.O).IsEqualTo(origin);
            Check.That(rayDiff.D).IsEqualTo(direction);
            Check.That(rayDiff.TMax).IsEqualTo(tMax);
            Check.That(rayDiff.Time).IsEqualTo(time);
            Check.That(rayDiff.Medium).IsEqualTo(medium);
            Check.That(rayDiff.HasDifferentials).IsFalse();

            Check.That(rayDiff.RxDirection).IsEqualTo(new Vector3F(0, -9,0));
            Check.That(rayDiff.RyDirection).IsEqualTo(new Vector3F(0, -9,0));
            Check.That(rayDiff.RxOrigin).IsEqualTo(new Point3F(-9,0,0));
            Check.That(rayDiff.RyOrigin).IsEqualTo(new Point3F(-9,0,0));
        }
    }
}