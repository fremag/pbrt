using System;
using System.Numerics;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
// ReSharper disable ExpressionIsAlwaysNull
// ReSharper disable SuspiciousTypeConversion.Global
// ReSharper disable EqualExpressionComparison
#pragma warning disable CS1718 

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class TransformTests
    {
        float[] row0 = { 1, 0, 0, 0 };
        float[] row1 = { 0, 1, 0, 0 };
        float[] row2 = { 0, 0, 1, 0 };
        float[] row3 = { 0, 0, 0, 1 };

        [SetUp]
        public void SetUp()
        {

        }

        [Test]
        public void ConstructorsTest()
        {
            Matrix4x4 m = new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            var t = new Transform(m);
            Check.That(t[0, 0]).IsEqualTo(1);
            Check.That(t[0, 1]).IsEqualTo(2);
            Check.That(t[0, 2]).IsEqualTo(3);
            Check.That(t[0, 3]).IsEqualTo(4);
            Check.That(t[1, 0]).IsEqualTo(5);
            Check.That(t[1, 1]).IsEqualTo(6);
            Check.That(t[1, 2]).IsEqualTo(7);
            Check.That(t[1, 3]).IsEqualTo(8);
            Check.That(t[2, 0]).IsEqualTo(9);
            Check.That(t[2, 1]).IsEqualTo(10);
            Check.That(t[2, 2]).IsEqualTo(11);
            Check.That(t[2, 3]).IsEqualTo(12);
            Check.That(t[3, 0]).IsEqualTo(13);
            Check.That(t[3, 1]).IsEqualTo(14);
            Check.That(t[3, 2]).IsEqualTo(15);
            Check.That(t[3, 3]).IsEqualTo(16);

            Check.ThatCode(() => t[-1, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[0, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[1, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[2, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[3, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[0, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[0, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[1, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[1, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[2, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[2, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[3, -1]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[3, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[4, 4]).Throws<IndexOutOfRangeException>();
            Check.ThatCode(() => t[4, 4]).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void TranslatePointTest()
        {
            Transform t = Transform.Translate(1, 1, 1);
            var p = Point3F.Zero;
            var translated = t.Apply(p);
            Check.That(translated).IsEqualTo(new Point3F(1, 1, 1));
            var invTranslated = t.Inverse().Apply(translated);
            Check.That(invTranslated).IsEqualTo(p);
        }

        [Test]
        public void TranslateVectorTest()
        {
            Transform t = Transform.Translate(new Vector3F(5, 7, 9));
            var v = new Vector3F(1,2,3);
            var translated = t.Apply(v);
            Check.That(translated).IsEqualTo(new Vector3F(1, 2, 3));
            var invTranslated = t.Inverse().Apply(translated);
            Check.That(invTranslated).IsEqualTo(v);
        }

        [Test]
        public void IsIdentityTest()
        {
            var mat = new[] { row0, row1, row2, row3 };
            var t = new Transform(mat);
            Check.That(t.IsIdentity).IsTrue();
            row0[0] = 2;
            t = new Transform(mat);
            Check.That(t.IsIdentity).IsFalse();
        }

        [Test]
        public void TransposeTest()
        {
            Matrix4x4 m = new Matrix4x4(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16);
            var t = new Transform(m).Transpose();
            Check.That(t[0, 0]).IsEqualTo(1);
            Check.That(t[1, 0]).IsEqualTo(2);
            Check.That(t[2, 0]).IsEqualTo(3);
            Check.That(t[3, 0]).IsEqualTo(4);
            Check.That(t[0, 1]).IsEqualTo(5);
            Check.That(t[1, 1]).IsEqualTo(6);
            Check.That(t[2, 1]).IsEqualTo(7);
            Check.That(t[3, 1]).IsEqualTo(8);
            Check.That(t[0, 2]).IsEqualTo(9);
            Check.That(t[1, 2]).IsEqualTo(10);
            Check.That(t[2, 2]).IsEqualTo(11);
            Check.That(t[3, 2]).IsEqualTo(12);
            Check.That(t[0, 3]).IsEqualTo(13);
            Check.That(t[1, 3]).IsEqualTo(14);
            Check.That(t[2, 3]).IsEqualTo(15);
            Check.That(t[3, 3]).IsEqualTo(16);
        }

        [Test]
        public void EqualityTest()
        {
            var t1 = Transform.Translate(1, 0, 0);
            var t2 = Transform.Translate(1, 0, 0);
            Check.That(t1 == t2).IsTrue();
            Check.That(t2 == t1).IsTrue();
            Check.That(t1 == null).IsFalse();
            Check.That(null == t1).IsFalse();

            Check.That(t1 != t2).IsFalse();
            Check.That(t2 != t1).IsFalse();
            Check.That(t1 != null).IsTrue();
            Check.That(null != t1).IsTrue();
            t1 = null;
            Check.That(t1 == t2).IsFalse();
            Check.That(t1 == null).IsTrue();

            Check.That(t1 != t2).IsTrue();
            Check.That(t1 != null).IsFalse();
        }

        [Test]
        public void EqualsTest()
        {
            var t1 = Transform.Translate(1, 0, 0);
            var t2 = Transform.Translate(1, 0, 0);
            Check.That(t1).IsEqualTo(t2);
            Check.That(t1).IsEqualTo(t1);
            Check.That(t1.Equals((object)null)).IsFalse();
            Check.That(t1.Equals("5")).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            var t1 = Transform.Translate(1, 0, 0);
            var t2 = Transform.Translate(1, 0, 0);
            var t3 = Transform.Translate(0, 1, 0);
            Check.That(t1.GetHashCode()).IsEqualTo(t2.GetHashCode());
            Check.That(t1.GetHashCode()).Not.IsEqualTo(t3.GetHashCode());
        }

        [Test]
        public void ComparatorTest()
        {
            var mat1 = new[] { row0, row1, row2, row3 };
            var mat2 = new[] { row0, row0, row0, row0 };
            var transform1 = new Transform(mat1);
            var transform2 = new Transform(mat2);

            Check.That(transform1 < transform2).IsTrue();
            Check.That(transform1 > transform2).IsFalse();
            Check.That(transform1 > transform1).IsFalse();
            Check.That(transform1 > transform1).IsFalse();
        }

        [Test]
        public void ScaleTest()
        {
            var t = Transform.Scale(2, 3, 4);
            var v = new Vector3F(3, 5, 7);
            var scaled = t.Apply(v);
            Check.That(scaled).IsEqualTo(new Vector3F(6,15,28));
        }
        
        [Test]
        [TestCase(90, 1, 0, 0, 1, 0, 0)]
        [TestCase(90, 0, 1, 0, 0, 0, 1)]
        [TestCase(90, 0, 0, 1, 0, -1, 0)]
        public void RotateXTest(float deg, float x, float y, float z, float expX, float expY, float expZ)
        {
            var t = Transform.RotateX(deg);
            var rotated = t.Apply(new Point3F(x, y, z));
            
            Check.That(rotated.X).IsCloseTo(expX, 1e-6);
            Check.That(rotated.Y).IsCloseTo(expY, 1e-6);
            Check.That(rotated.Z).IsCloseTo(expZ, 1e-6);
        }
        
        [Test]
        [TestCase(90, 1, 0, 0, 0, 0, -1)]
        [TestCase(90, 0, 1, 0, 0, 1, 0)]
        [TestCase(90, 0, 0, 1, 1, 0, 0)]
        public void RotateYTest(float deg, float x, float y, float z, float expX, float expY, float expZ)
        {
            var t = Transform.RotateY(deg);
            var rotated = t.Apply(new Point3F(x, y, z));
            
            Check.That(rotated.X).IsCloseTo(expX, 1e-6);
            Check.That(rotated.Y).IsCloseTo(expY, 1e-6);
            Check.That(rotated.Z).IsCloseTo(expZ, 1e-6);
        }
        
        [Test]
        [TestCase(90, 1, 0, 0, 0, 1, 0)]
        [TestCase(90, 0, 1, 0, -1, 0, 0)]
        [TestCase(90, 0, 0, 1, 0, 0, 1)]
        public void RotateZTest(float deg, float x, float y, float z, float expX, float expY, float expZ)
        {
            var t = Transform.RotateZ(deg);
            var rotated = t.Apply(new Point3F(x, y, z));
            
            Check.That(rotated.X).IsCloseTo(expX, 1e-6);
            Check.That(rotated.Y).IsCloseTo(expY, 1e-6);
            Check.That(rotated.Z).IsCloseTo(expZ, 1e-6);
        }

        [Test]
        public void LookAtTest()
        {
            var pos = new Point3F( 2.34f, 0, 0);
            var look = new Point3F(1.23f, 0, 0);
            var up = new Vector3F(0, 1, 0);
            
            var t = Transform.LookAt(pos, look, up);
            var p = t.Apply(new Point3F(1.23f, 0, 0));
            Check.That(p).IsEqualTo(new Point3F(0, 0, 2.34f-1.23f));
        }

        [Test]
        public void SwapsHandednessTest()
        {
            var t = Transform.Translate(1, 0, 0);
            Check.That(t.SwapsHandedness()).IsFalse();

            var m = new Matrix4x4(1, 0, 0, 0,
                0, -1, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
            var anotherTransform = new Transform(m);
            Check.That(anotherTransform.SwapsHandedness()).IsTrue();
        }

        [Test]
        public void ApplyPointTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var p = t.Apply(Point3F.Zero);
            Check.That(p).IsEqualTo(new Point3F(1, 0, 0));
        }
        
        [Test]
        public void Wp_ApplyPointTest()
        { // just for code coverage
            var m = new Matrix4x4();
            var t = new Transform(m);
            var p = t.Apply(Point3F.Zero);
            Check.That(p.X).IsNaN();
            Check.That(p.Y).IsNaN();
            Check.That(p.Z).IsNaN();
        }
        
        [Test]
        public void ApplyVectorTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var v = t.Apply(new Vector3F(1, 0, 0));
            Check.That(v).IsEqualTo(new Vector3F(1, 0, 0));
        }
        
        [Test]
        public void ApplyNormalTest()
        {
            // normal is not transformed by translation because the angle with tangent is not modified 
            var t = Transform.Translate(1, 0, 0);
            var n = t.Apply(new Normal3F(1, 0, 0));
            Check.That(n).IsEqualTo(new Normal3F(1, 0, 0));
            
            // normal is modified by rotation
            var r = Transform.RotateY(180);
            var n2 = r.Apply(new Normal3F(1, 0, 0));
            Check.That(n2.X).IsCloseTo(-1, 1e-5);
            Check.That(n2.Y).IsCloseTo(0, 1e-5);
            Check.That(n2.Z).IsCloseTo(0, 1e-5);
        }

        [Test]
        public void ApplyRayTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var origin = Point3F.Zero;
            var dir = new Vector3F(1, 0, 0);
            var ray = new Ray(origin, dir, 1000, 1, null);
            var newRay = t.Apply(ray);
            // Origin is translated but direction is not modified
            Check.That(newRay.O).IsEqualTo(new Point3F(1, 0, 0));
            Check.That(newRay.D).IsEqualTo(dir);
        }

        [Test]
        public void ApplyBoundsTest()
        {
            var p1 = new Point3F(-1, -1, -1);
            var p2 = new Point3F(1, 1, 1);
            var bounds = new Bounds3F(p1, p2);

            var translation = Transform.Translate(1, 1, 1);
            var translatedBounds = translation.Apply(bounds);
            Check.That(translatedBounds.PMin).IsEqualTo(Point3F.Zero);
            Check.That(translatedBounds.PMax).IsEqualTo(new Point3F(2,2,2));

            // bounds are invariant by rotation of pi/2
            var rotation = Transform.RotateX(90);
            var rotatedBounds = rotation.Apply(bounds);
            Check.That(rotatedBounds.PMin).IsEqualTo(bounds.PMin);
            Check.That(rotatedBounds.PMax).IsEqualTo(bounds.PMax);
        }
        
        [Test]
        public void MulPointTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var p = t * Point3F.Zero;
            Check.That(p).IsEqualTo(new Point3F(1, 0, 0));
        }
        
        [Test]
        public void MulVectorTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var v = t * new Vector3F(1, 0, 0);
            Check.That(v).IsEqualTo(new Vector3F(1, 0, 0));
        }
        
        [Test]
        public void MulNormalTest()
        {
            // normal is not transformed by translation because the angle with tangent is not modified 
            var t = Transform.Translate(1, 0, 0);
            var n = t * new Normal3F(1, 0, 0);
            Check.That(n).IsEqualTo(new Normal3F(1, 0, 0));
            
            // normal is modified by rotation
            var r = Transform.RotateY(180);
            var n2 = r * new Normal3F(1, 0, 0);
            Check.That(n2.X).IsCloseTo(-1, 1e-5);
            Check.That(n2.Y).IsCloseTo(0, 1e-5);
            Check.That(n2.Z).IsCloseTo(0, 1e-5);
        }

        [Test]
        public void MulRayTest()
        {
            var t = Transform.Translate(1, 0, 0);
            var origin = Point3F.Zero;
            var dir = new Vector3F(1, 0, 0);
            var ray = new Ray(origin, dir, 1000, 1, null);
            var newRay = t * ray;
            // Origin is translated but direction is not modified
            Check.That(newRay.O).IsEqualTo(new Point3F(1, 0, 0));
            Check.That(newRay.D).IsEqualTo(dir);
        }

        [Test]
        public void MulBoundsTest()
        {
            var p1 = new Point3F(-1, -1, -1);
            var p2 = new Point3F(1, 1, 1);
            var bounds = new Bounds3F(p1, p2);

            var translation = Transform.Translate(1, 1, 1);
            var translatedBounds = translation * bounds;
            Check.That(translatedBounds.PMin).IsEqualTo(Point3F.Zero);
            Check.That(translatedBounds.PMax).IsEqualTo(new Point3F(2,2,2));

            // bounds are invariant by rotation of pi/2
            var rotation = Transform.RotateX(90);
            var rotatedBounds = rotation * bounds;
            Check.That(rotatedBounds.PMin).IsEqualTo(bounds.PMin);
            Check.That(rotatedBounds.PMax).IsEqualTo(bounds.PMax);
        }

        [Test]
        public void MulTransformTest()
        {
            var translation = Transform.Translate(1, 1, 1);
            var rotation = Transform.RotateX(90);
          
            // Point is rotated then translated
            var p = Point3F.Zero;
            var compos = translation * rotation;
            var p0 = compos * p;
            var p1 = rotation * p;
            var p2 = translation * p1;
            
            Check.That(p0).IsEqualTo(new Point3F(1, 1, 1));
            Check.That(p2).IsEqualTo(new Point3F(1, 1, 1));

            var p3 = translation * (rotation * p);
            Check.That(p3).IsEqualTo(new Point3F(1, 1, 1));
            var p4 = (translation * rotation) * p;
            Check.That(p4).IsEqualTo(new Point3F(1, 1, 1));
            
            // Let's check it doesn't commute
            var p5 = (rotation * translation) * p;
            Check.That(p5).IsEqualTo(new Point3F(1, -1, 1));
            Check.That(p5).IsNotEqualTo(p0);
        }

        [Test]
        public void ApplyPointAbsErrorTest()
        {
            var translation = Transform.Translate(1, 1, 1);
            var p = Point3F.Zero;

            var p1 = translation.Apply(p, out var pError);
            Check.That(p1).IsEqualTo(new Point3F(1,1,1));
            Check.That(pError).IsCloseTo((3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon));
        }

        [Test]
        public void ApplyPointWithErrorTest()
        {
            var translation = Transform.Translate(1, 1, 1);
            var p = Point3F.Zero;

            Vector3F vError =2* new Vector3F(float.Epsilon,float.Epsilon,float.Epsilon);
            var p1 = translation.Apply(p, vError, out var pError);
            Check.That(p1).IsEqualTo(new Point3F(1,1,1));
            Check.That(pError).IsCloseTo((3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon));
        }

        [Test]
        public void Wp_ApplyPointWithErrorTest()
        {
            // Just for code coverage
            var translation = new Transform(new Matrix4x4());
            var p = Point3F.Zero;

            Vector3F vError =2* new Vector3F(float.Epsilon,float.Epsilon,float.Epsilon);
            var p1 = translation.Apply(p, vError, out var pError);
            Check.That(p1.X).IsNaN();
            Check.That(p1.Y).IsNaN();
            Check.That(p1.Z).IsNaN();
            Check.That(pError).IsEqualTo(Vector3F.Zero);
        }

        [Test]
        public void ApplyVectorAbsErrorTest()
        {
            var rotation = Transform.RotateX(180);
            var v = new Vector3F(1, 1, 1);

            var v1 = rotation.Apply(v, out var pError);
            Check.That(pError).IsCloseTo((3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon));
            Check.That(v1.X).IsCloseTo(1, 1e-6);
            Check.That(v1.Y).IsCloseTo( -1, 1e-6);
            Check.That(v1.Z).IsCloseTo( -1, 1e-6);
        }
        
        [Test]
        public void ApplyRayWithErrorTest()
        {
            var t = Transform.RotateX(180);
            var origin = new Point3F(1, 1, 1);
            var dir = new Vector3F(1, 0, 0);
            var ray = new Ray(origin, dir, 1000, 1, null);

            var newRay = t.Apply(ray, out var oError, out var vError);
            
            Check.That(newRay.O.X).IsCloseTo(1, 1e-6);
            Check.That(newRay.O.Y).IsCloseTo( -1, 1e-6);
            Check.That(newRay.O.Z).IsCloseTo( -1, 1e-6);
            Check.That(newRay.D).IsEqualTo(dir);
            Check.That(oError).IsCloseTo((3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon, 3*MathUtils.MachineEpsilon));
            Check.That(vError).IsCloseTo((3*MathUtils.MachineEpsilon,0, 0));
        }

        [Test]
        public void ApplySurfaceInteractionTest()
        {
            AbstractShape shape = new Sphere(Transform.Translate(0,0,0), Transform.Translate(0,0,0), false, 2, -1, 1, 360);
            Point3F p = new Point3F(0, 1, 0);
            Vector3F pError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon);
            Point2F uv = new Point2F(0, 0);
            Vector3F wo = new Vector3F(1, 1, 1);

            Vector3F dpdu = new Vector3F(0, 0, 1);
            Vector3F dpdv = new Vector3F(1, 0, 0);
            Normal3F dndu = new Normal3F(0, 1, 0.01f);
            Normal3F dndv = new Normal3F(0.01f, 1, 0);
            float time = 1;
            
            var si = new SurfaceInteraction(p, pError, uv, wo, dpdu, dpdv, dndu, dndv, time, shape);
            si.DpDx = new Vector3F(1, 1, 1);
            si.DpDy = new Vector3F(1, 1, 1);
            
            var translation = Transform.Translate(1, 0, 0);
            var newSurfInter = translation.Apply(si);

            Check.That(newSurfInter.P).IsEqualTo(translation * p);
            Check.That(newSurfInter.Time).IsEqualTo(time);
            Check.That(newSurfInter.Uv).IsEqualTo(uv);
            Check.That(newSurfInter.Shape).IsSameReferenceAs(shape);
            Check.That(newSurfInter.Shading.N).IsEqualTo(translation * si.N);
            Check.That(newSurfInter.Shading.DnDu).IsEqualTo(translation * dndu);
            Check.That(newSurfInter.Shading.DnDv).IsEqualTo(translation * dndv);
            Check.That(newSurfInter.Shading.DpDu).IsEqualTo(translation * dpdu);
            Check.That(newSurfInter.Shading.DpDv).IsEqualTo(translation * dpdv);
            Check.That(newSurfInter.DpDx).IsEqualTo(translation * si.DpDx);
            Check.That(newSurfInter.DpDy).IsEqualTo(translation * si.DpDy);
        }

        [Test]
        public void RotationVectorTest()
        {
            var v1 = new Vector3F(1, 0, 0);
            var v2 = new Vector3F(0, 1, 0);
            Transform mat = Transform.Rotation(v1, v2);
            Check.That(mat).IsEqualTo(new Transform(new Matrix4x4(
                0, -1, 0, 0 ,
                1, 0, 0, 0, 
                0, 0, 1, 0, 
                0, 0, 0, 1)));
        }
        
        [Test]
        public void RotationVector_SinPhi_Test()
        {
            var v1 = new Vector3F(1, 0, 0);
            var v2 = new Vector3F(2, 0, 0);
            Transform mat = Transform.Rotation(v1, v2);
            Check.That(mat).IsEqualTo(new Transform(new Matrix4x4(
                2, 0, 0, 0 ,
                0, 2, 0, 0, 
                0, 0, 2, 0, 
                0, 0, 0, 1)));
        }

        [Test]
        public void ToStringTest()
        {
            var mat = Transform.Translate(1, 2, 3);
            Check.That(mat.ToString()).IsEqualTo("{ {M11:1 M12:0 M13:0 M14:1} {M21:0 M22:1 M23:0 M24:2} {M31:0 M32:0 M33:1 M34:3} {M41:0 M42:0 M43:0 M44:1} }");
        }
    }
}