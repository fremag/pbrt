using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class RayTests
    {
        [Test]
        public void BasicTest()
        {
            var o = new Point3F(1.23f, 2.34f, 3.45f);
            var d = new Vector3F(1, 0, -1);
            var med = new Medium();
            var ray = new Ray(o, d, 1000, 0, med);
            Check.That(ray.O).IsEqualTo(o);
            Check.That(ray.D).IsEqualTo(d);
            Check.That(ray.TMax).IsEqualTo(1000);
            Check.That(ray.Time).IsEqualTo(0);
            Check.That(ray.Medium).IsSameReferenceAs(med);
            
            Check.That(ray.HasNaNs).IsFalse();
            Check.That(ray.At(1)).IsEqualTo(new Point3F(2.23f, 2.34f, 2.45f));
        }

        [Test]
        [TestCase(0,0,0,0,0,0, false)]
        [TestCase(0,0,0,0,0,float.NaN, true)]
        [TestCase(0,0,0,0,float.NaN,0, true)]
        [TestCase(0,0,0,float.NaN,0,0, true)]
        [TestCase(0,0,float.NaN,0,0,0, true)]
        [TestCase(0,float.NaN, 0,0,0,0,true)]
        [TestCase(float.NaN, 0,0,0,0,0,true)]
        public void HasNaNsTest(float ox, float oy, float oz, float dx, float dy, float dz, bool hasNaNs)
        {
            var o = new Point3F(ox, oy, oz);
            var d = new Vector3F(dx, dy, dz);
            var med = new Medium();
            var ray = new Ray(o, d, 1000, 0, med);
            Check.That(ray.HasNaNs).IsEqualTo(hasNaNs);
        }

        [Test]
        public void ConstructorTest()
        {
            var ray = new Ray();
            Check.That(ray.O).IsNull();
            Check.That(ray.D).IsNull();
            Check.That(ray.TMax).Not.IsFinite();
            Check.That(ray.Time).IsEqualTo(0);
            Check.That(ray.Medium).IsNull();
        }
    }
}