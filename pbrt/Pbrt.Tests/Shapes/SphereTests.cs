using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;

namespace Pbrt.Tests.Shapes
{
    [TestFixture]
    public class SphereTests
    {
        /*
         * Sphere coordinates
         *        ^  z
         *        |
         *        |
         *        / -----> x 
         *      / 
         *    y
         * 
         *  x = r sin theta Cos phi
         *  y = r sin theta sin phi
         *  z = r cos theta
         */
        private Sphere sphere;
        private Transform translation;
        private Transform invTranslation;

        [SetUp]
        public void SetUp()
        {
            translation = Transform.Translate(0, 1, 0);
            invTranslation = translation.Inverse();
            sphere = new Sphere(translation, invTranslation, false, 2, -1, 1, 360);
        }

        [Test]
        public void BasicTest()
        {
            Check.That(sphere.Radius).IsEqualTo(2);
            Check.That(sphere.ZMax).IsEqualTo(1);
            Check.That(sphere.ZMin).IsEqualTo(-1);
            Check.That(sphere.ThetaMax).IsEqualTo(MathF.PI / 3);
            Check.That(sphere.ThetaMin).IsEqualTo(2 * MathF.PI / 3);
            Check.That(sphere.PhiMax).IsCloseTo(6.28f, 1e-2);
            Check.That(sphere.Area).IsCloseTo(2 * 3.14f * sphere.Radius * sphere.Radius, 1e-1);
            Check.That(sphere.ObjectBound.PMin).IsEqualTo(new Point3F(-2, -2, -1));
            Check.That(sphere.ObjectBound.PMax).IsEqualTo(new Point3F(2, 2, 1));
            Check.That(sphere.ReverseOrientation).IsFalse();
            Check.That(sphere.ObjectToWorld).IsEqualTo(translation);
            Check.That(sphere.WorldToObject).IsEqualTo(invTranslation);
            Check.That(sphere.TransformSwapsHandedness).IsFalse();
        }

        [Test]
        public void BasicConstructorTest()
        {
            var myTranslation = Transform.Translate(1, 2, 3);
            var mySphere = new Sphere(myTranslation, 2);
            Check.That(mySphere.Radius).IsEqualTo(2);
            Check.That(mySphere.ZMax).IsEqualTo(2);
            Check.That(mySphere.ZMin).IsEqualTo(-2);
            Check.That(mySphere.ThetaMax).IsEqualTo(0);
            Check.That(mySphere.ThetaMin).IsEqualTo(MathF.PI);
            Check.That(mySphere.PhiMax).IsCloseTo(6.28f, 1e-2);
            Check.That(mySphere.Area).IsCloseTo(4 * 3.14f * sphere.Radius * sphere.Radius, 1e-1);
            Check.That(mySphere.ObjectBound.PMin).IsEqualTo(new Point3F(-2, -2, -2));
            Check.That(mySphere.ObjectBound.PMax).IsEqualTo(new Point3F(2, 2, 2));
            Check.That(mySphere.ReverseOrientation).IsFalse();
            Check.That(mySphere.ObjectToWorld).IsEqualTo(myTranslation);
            Check.That(mySphere.WorldToObject).IsEqualTo(myTranslation.Inverse());
            Check.That(mySphere.TransformSwapsHandedness).IsFalse();
        }

        [Test]
        public void BasicIntersectTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
            Check.That(tHit).IsCloseTo(8, 1e-5f);
            Check.That(iSec.Uv.X).IsEqualTo(0.5);
            Check.That(iSec.Uv.Y).IsEqualTo(0.5);
            Check.That(iSec.Shading.N).IsEqualTo(new Normal3F(-1, 0, 0));
            Check.That(iSec.Shading.DpDu).IsEqualTo(new Vector3F(0, -4 * MathF.PI, 0));
            Check.That(iSec.Shading.DpDv).IsEqualTo(new Vector3F(0, 0, MathF.PI * 2 / 3));
            Check.That(iSec.Shading.DnDu).IsEqualTo(new Normal3F(0, -2 * MathF.PI, 0));
            Check.That(iSec.Shading.DnDv).IsEqualTo(new Normal3F(0, 0, MathF.PI / 3));
            Check.That(iSec.DuDx).IsEqualTo(0);
            Check.That(iSec.DuDy).IsEqualTo(0);
            Check.That(iSec.DvDx).IsEqualTo(0);
            Check.That(iSec.DvDy).IsEqualTo(0);
            Check.That(iSec.FaceIndex).IsEqualTo(0);
            Check.That(iSec.Shape).IsSameReferenceAs(sphere);
        }

        [Test]
        public void Basic_NegativePhi_IntersectTest()
        {
            var o = new Point3F(0, -5, 0);
            var dir = new Vector3F(0, 1, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
            Check.That(tHit).IsCloseTo(4, 1e-5f);
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
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
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
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
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
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMax_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, sphere.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMin_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, sphere.ZMin - 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
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
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMax_Inside_NoIntersectTest()
        {
            var o = new Point3F(1, 1, sphere.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 1;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void ZMax_FarAway_NoIntersectTest()
        {
            var o = new Point3F(-10, 1, sphere.ZMax + 0.1f);
            var dir = new Vector3F(1, 0, 0);
            float max = 10;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void PhiMax_NegativePhi_NoIntersectTest()
        {
            Transform transZero = Transform.Translate(0, 0, 0);
            sphere = new Sphere(transZero, transZero, false, 2, -2, 2, 90);

            var o = new Point3F(-1, -1, -10);
            var dir = new Vector3F(0, 0, 1);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsFalse();
            Check.That(tHit).IsZero();
            Check.That(iSec).IsNull();
        }

        [Test]
        public void Inside_IntersectTest()
        {
            Transform transZero = Transform.Translate(0, 0, 0);
            sphere = new Sphere(transZero, transZero, false, 2, 0, 2, 360);

            var o = new Point3F(-1, -1, -10);
            var dir = new Vector3F(0, 0, 1);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
            Check.That(tHit).IsCloseTo(10+MathF.Sqrt(2), 1e-5f);
            Check.That(iSec.P.X).IsCloseTo(-1, 1e-4);
            Check.That(iSec.P.Y).IsCloseTo(-1, 1e-4);
            Check.That(iSec.P.Z).IsCloseTo(MathF.Sqrt(2), 1e-4);
            Check.That(iSec.N.X).IsCloseTo(-0.5, 1e-4);
            Check.That(iSec.N.Y).IsCloseTo(-0.5, 1e-4);
            Check.That(iSec.N.Z).IsCloseTo(MathF.Sqrt(2)/2, 1e-4);
        }

        [Test]
        public void WithRefineSphereIntersectionPointTest()
        {
            Transform transZero = Transform.Translate(0, 0, 0);
            sphere = new Sphere(transZero, transZero, false, 0.1f, -2, 2, 360);
            var o = new Point3F(0, 0, -1e7f);
            var dir = new Vector3F(0, 0, 1);
            float max = float.PositiveInfinity;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
        }

        [Test]
        public void ZMax_WithRefineSphereIntersectionPointTest()
        {
            Transform transZero = Transform.Translate(0, 0, 0);
            sphere = new Sphere(transZero, transZero, false, 0.2e14f, -MathUtils.MachineEpsilon, MathUtils.MachineEpsilon, 360);
            var o = new Point3F(0, 0, -1e16f);
            var dir = new Vector3F(0, 0, 1);
            float max = float.PositiveInfinity;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out _, out _);
            Check.That(hit).IsFalse();
        }

        [Test]
        public void WorldBoundTest()
        {
            var objBounds = sphere.ObjectBound;
            Check.That(objBounds.PMin).IsEqualTo(new Point3F(-2, -2, -1)); // -1 only  because of ZMin 
            Check.That(objBounds.PMax).IsEqualTo(new Point3F( 2,  2, 1));// 1 only  because of ZMax
            var worldBound = sphere.WorldBound();
            Check.That(worldBound.PMin).IsEqualTo(new Point3F(-2, -1, -1));// -1 only  because of ZMin
            Check.That(worldBound.PMax).IsEqualTo(new Point3F( 2,  3, 1)); // 1 only  because of ZMax
        }

        [Test]
        public void IntersectPTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.IntersectP(ray);
            Check.That(hit).IsTrue();
        }

        [Test]
        public void SamplePointTest()
        {
            var interaction =sphere.Sample(Point2F.Zero);
            Check.That(interaction.N).IsEqualTo(new Normal3F(0, 0, 1));
            Check.That(interaction.P).IsEqualTo(new Point3F(0, 1, 2));
        }
        
        [Test]
        public void SamplePoint_ReverseOrientation_Test()
        {
            sphere.ReverseOrientation = true;
            var interaction =sphere.Sample(Point2F.Zero);
            Check.That(interaction.N).IsEqualTo(new Normal3F(0, 0, -1));
            Check.That(interaction.P).IsEqualTo(new Point3F(0, 1, 2));
        }

        [Test]
        public void Pdf_OutsideSphere_Test()
        {
            var interaction = new Interaction
            {
                P = new Point3F(-2, 0, 0),
                N = new Normal3F(-1, 0, 0),
                PError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon)
            };
            
            var wi = new Vector3F(1, 0, 0);
            var pdf = sphere.Pdf(interaction, wi);
            Check.That(pdf).IsEqualTo(0.287914008f);
        }
        
        [Test]
        public void Pdf_InsideSphere_Test()
        {
            var interaction = new Interaction
            {
                P = new Point3F(0, 0, 0),
                N = new Normal3F(-1, 0, 0),
                PError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon),
                MediumInterface = new MediumInterface(null)
            };
            
            var wi = new Vector3F(1, 0, 0);
            var pdf = sphere.Pdf(interaction, wi);
            Check.That(pdf).IsEqualTo(0.137832224f);
        }

        [Test]
        public void SampleTest()
        {
            var interaction = new Interaction
            {
                P = new Point3F(-2, 0, 0),
                N = new Normal3F(-1, 0, 0),
                PError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon)
            };

            var inter = sphere.Sample(interaction, Point2F.Zero);
            Check.That(inter.N).IsEqualTo(new Normal3F(-0.89442718f, -0.44721359f, 0));
            Check.That(inter.P).IsEqualTo(new Point3F(-1.7888546f, 0.1055727f, 0));
        }

        [Test]
        public void Sample_ReverseOrientationTest()
        {
            var interaction = new Interaction
            {
                P = new Point3F(-2, 0, 0),
                N = new Normal3F(-1, 0, 0),
                PError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon)
            };
            sphere.ReverseOrientation = true;
            var inter = sphere.Sample(interaction, Point2F.Zero);
            Check.That(inter.N).IsEqualTo(new Normal3F(0.89442718f, 0.44721359f, 0));
            Check.That(inter.P).IsEqualTo(new Point3F(-1.7888546f, 0.1055727f, 0));
        }
        
        [Test]
        public void Sample_Inside_Test()
        {
            var interaction = new Interaction
            {
                P = new Point3F(0, 0, 0),
                N = new Normal3F(-1, 0, 0),
                PError = new Vector3F(float.Epsilon, float.Epsilon, float.Epsilon)
            };

            var inter = sphere.Sample(interaction, Point2F.Zero);
            Check.That(inter.N).IsEqualTo(new Normal3F(0, 0, 1));
            Check.That(inter.P).IsEqualTo(new Point3F(0, 1, 2));
        }

        [Test]
        public void PdfTest()
        {
            var pdf = sphere.Pdf(null);
            Check.That(pdf).IsEqualTo(1f / 8 /  MathF.PI);
        }
    }
}