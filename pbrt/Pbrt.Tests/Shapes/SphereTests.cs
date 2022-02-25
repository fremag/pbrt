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
        public void BasicIntersectTest()
        {
            var o = new Point3F(-10, 1, 0);
            var dir = new Vector3F(1, 0, 0);
            float max = 1000;
            var time = 1;
            var ray = new Ray(o, dir, max, time, null);
            var hit = sphere.Intersect(ray, out var tHit, out var iSec);
            Check.That(hit).IsTrue();
            Check.That(tHit).IsEqualTo(8);
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
            Check.That(tHit).IsEqualTo(4);
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
        public void WithRefineSphereIntersectionPointTest()
        {
            Transform transZero = Transform.Translate(0, 0, 0);
            sphere = new Sphere(transZero, transZero, false, 0.1f, -2, 2, 360);
            var o = new Point3F(0, 0, -1e15f);
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
            sphere = new Sphere(transZero, transZero, false, 0.2e-16f, -float.Epsilon, float.Epsilon, 360);
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
    }
}