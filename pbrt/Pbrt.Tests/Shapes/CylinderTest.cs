using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;

namespace Pbrt.Tests.Shapes
{
    [TestFixture]
    public class CylinderTest
    {
        Cylinder cyl = new Cylinder(Transform.Translate(0,0,0), 0.5f, 0, 0.5f, 180f);
        
        [Test]
        public void AreaTest()
        {
            var area = cyl.Area;
            Check.That(area).IsEqualTo(MathF.PI/4);
        }

        [Test]
        public void SampleTest()
        {
            var it = cyl.Sample(Point2F.Zero);
            Check.That(it.N).IsEqualTo(new Normal3F(1,0,0));
            Check.That(it.P).IsEqualTo(new Point3F(0.5f,0,0));
            
            cyl = new Cylinder(Transform.Translate(0,0,0), Transform.Translate(0,0,0), true, 0.5f, 0, 0.5f, 180f);
            it = cyl.Sample(Point2F.Zero);
            Check.That(it.N).IsEqualTo(new Normal3F(-1,0,0));
            Check.That(it.P).IsEqualTo(new Point3F(0.5f,0,0));
        }

        [Test]
        public void Basic_NegativePhi_IntersectTest()
        {
            var o = new Point3F(0, -5, 0);
            var dir = new Vector3F(0, 1, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
            Check.That(tHit).IsCloseTo(5.5f, 1e-5f);
        }

        [Test]
        public void NoIntersectTest()
        {
            // ray is "higher" than the sphere
            var o = new Point3F(-10, 10, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void WrongDirection_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(-1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void TooFarAway_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }


        [Test]
        public void ZMax_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMin_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMin - 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
        

        [Test]
        public void Inside_NoIntersectTest()
        {
            var o = new Point3F(-1, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 0.1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMax_Inside_NoIntersectTest()
        {
            var o = new Point3F(1, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMax_FarAway_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 10;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
 
        [Test]
        public void TMax_NoIntersectTest()
        {
            var o = new Point3F(-1000f, 0, 0);
            var dir = new Vector3F(0.1f, 0f, 0f);
            float max = 10;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
 
        [Test]
        public void Hit_NegativeShapeHit_TMax_NoIntersectTest()
        {
            var o = new Point3F(0.1f, 0, 0);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 0.1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
       
        [Test]
        public void Hit_NegativeShapeHit_Clipping_NoIntersectTest()
        {
            var o = new Point3F(0.1f, 0, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
       
        [Test]
        public void Hit_NegativeShapeHit_Clipping_2_NoIntersectTest()
        {
            var o = new Point3F(-1f, 0, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
       
        [Test]
        public void Hit_Clipping_Z_TMax_NoIntersectTest()
        {
            var o = new Point3F(-10f, 0f, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 10f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
       
        [Test]
        public void Hit_Clipping_Z_NoIntersectTest()
        {
            var o = new Point3F(-10f, -0.1f, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 100f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }
       
        [Test]
        public void Basic_NegativePhi_IntersectPTest()
        {
            var o = new Point3F(0, -5, 0);
            var dir = new Vector3F(0, 1, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsTrue();
        }

        [Test]
        public void NoIntersectPTest()
        {
            // ray is "higher" than the sphere
            var o = new Point3F(-10, 10, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void WrongDirection_NoIntersectPTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(-1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void TooFarAway_NoIntersectPTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void ZMax_NoIntersectPTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void ZMin_NoIntersectPTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMin - 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
        

        [Test]
        public void Inside_NoIntersectPTest()
        {
            var o = new Point3F(-1, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 0.1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void ZMax_Inside_NoIntersectPTest()
        {
            var o = new Point3F(1, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void ZMax_FarAway_NoIntersectPTest()
        {
            var o = new Point3F(-10, 1, cyl.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 10;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
 
        [Test]
        public void TMax_NoIntersectPTest()
        {
            var o = new Point3F(-1000f, 0, 0);
            var dir = new Vector3F(0.1f, 0f, 0f);
            float max = 10;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
 
        [Test]
        public void Hit_NegativeShapeHit_TMax_NoIntersectPTest()
        {
            var o = new Point3F(0.1f, 0, 0);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 0.1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
       
        [Test]
        public void Hit_NegativeShapeHit_Clipping_NoIntersectPTest()
        {
            var o = new Point3F(0.1f, 0, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
       
        [Test]
        public void Hit_NegativeShapeHit_Clipping_2_NoIntersectPTest()
        {
            var o = new Point3F(-1f, 0, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 1f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
       
        [Test]
        public void Hit_Clipping_Z_TMax_NoIntersectPTest()
        {
            var o = new Point3F(-10f, 0f, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 10f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
       
        [Test]
        public void Hit_Clipping_Z_NoIntersectPTest()
        {
            var o = new Point3F(-10f, -0.1f, 1000f);
            var dir = new Vector3F(1f, 0f, 0f);
            float max = 100f;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = cyl.IntersectP(ray);
            Check.That(hit).IsFalse();
        }
    }
}