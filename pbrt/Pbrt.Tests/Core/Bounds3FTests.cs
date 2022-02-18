using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class Bounds3FTests
    {
        [Test]
        public void ConstructorTest()
        {
            var b = new Bounds3F();
            Check.That(b.PMin).IsEqualTo(new Point3F(float.MaxValue,float.MaxValue,float.MaxValue));
            Check.That(b.PMax).IsEqualTo(new Point3F(float.MinValue,float.MinValue,float.MinValue));
        }
        
        [Test]
        public void ConstructorPointTest()
        {
            var b = new Bounds3F(Point3F.Zero);
            Check.That(b.PMin).IsEqualTo(Point3F.Zero);
            Check.That(b.PMax).IsEqualTo(Point3F.Zero);
        }
        
        [Test]
        public void ConstructorPMinMaxTest()
        {
            var p1 = new Point3F(-1, -1, -1);
            var p2 = new Point3F(1, 1, 1);
            var b = new Bounds3F(p1, p2);
            Check.That(b.PMin).IsEqualTo(p1);
            Check.That(b.PMax).IsEqualTo(p2);
        }
    }
}