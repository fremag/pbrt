using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Shapes
{
    [TestFixture]
    public class DiskTests
    {
        [Test]
        public void ConstructorTest()
        {
            var disk = new Disk(Transform.Translate(1, 0, 0));
            var worldBound = disk.WorldBound();
            Check.That(worldBound.PMin).Check((0, -1, 0));
            Check.That(worldBound.PMax).Check((2, 1, 0));

            Check.That(disk.WorldToObject).IsEqualTo(Transform.Translate(-1, 0, 0));
            Check.That(disk.Radius).IsEqualTo(1);
            Check.That(disk.InnerRadius).IsEqualTo(0);
            Check.That(disk.PhiMax).IsEqualTo(MathF.PI*2);
        }
        
        [Test]
        public void SampleTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 1.23f, 3.45f, 2.34f, 270);
            var sample = disk.Sample(Point2F.Zero);
            Check.That(sample.N).IsEqualTo(new Normal3F(0,0,-1));
            Check.That(sample.P).IsEqualTo(new Point3F(-2.4395185f, -1.4395185f, 1.23f));
        }

        [Test]
        public void AreaTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 1.23f, 3.45f, 2.34f, 270);
            var area = disk.Area;
            Check.That(area).IsEqualTo((3.45f*3.45f - 2.34f*2.34f)* 270f.Radians() * 0.5);
        }

        [Test]
        public void Parallel_No_IntersectTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 1, 0)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsEqualTo(0);
            Check.That(isect).IsNull();
        }

        [Test]
        public void OriginAboveDisk_No_IntersectTest()
        {
            Transform transform = Transform.Translate(0, 0, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 5),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsEqualTo(0);
            Check.That(isect).IsNull();
        }

        [Test]
        public void InnerRadius_No_IntersectTest()
        {
            Transform transform = Transform.Translate(0, 0, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsEqualTo(0);
            Check.That(isect).IsNull();
        }

        [Test]
        public void OuterRadius_No_IntersectTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(-10, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsEqualTo(0);
            Check.That(isect).IsNull();
        }

        [Test]
        public void PhiMax_No_IntersectTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 0f, 90);
            var ray = new Ray
            {
                O = new Point3F(-1, 1, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsFalse();
            Check.That(tHit).IsEqualTo(0);
            Check.That(isect).IsNull();
        }

        [Test]
        public void IntersectTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 0f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.Intersect(ray, out var tHit, out var isect);
            Check.That(result).IsTrue();
            Check.That(tHit).IsEqualTo(3);
            Check.That(isect.P).Check((0, 0, 3));
            Check.That(isect.N).Check((0, 0, 1));
        }

        [Test]
        public void Parallel_No_IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 1, 0)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void OriginAboveDisk_No_IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 0, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 5),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void InnerRadius_No_IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 0, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void OuterRadius_No_IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 1f, 270);
            var ray = new Ray
            {
                O = new Point3F(-10, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void PhiMax_No_IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 0f, 90);
            var ray = new Ray
            {
                O = new Point3F(-1, 1, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsFalse();
        }

        [Test]
        public void IntersectPTest()
        {
            Transform transform = Transform.Translate(0, 1, 0);
            Disk disk = new Disk(transform, transform.Inverse(), true, 3f, 2f, 0f, 270);
            var ray = new Ray
            {
                O = new Point3F(0, 0, 0),
                D = new Vector3F(0, 0, 1)
            };
            
            var result = disk.IntersectP(ray);
            Check.That(result).IsTrue();
        }
    }
}