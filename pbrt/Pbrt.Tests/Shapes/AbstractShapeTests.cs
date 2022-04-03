using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Media;
using pbrt.Shapes;

namespace Pbrt.Tests.Shapes
{
    public class AbstractShapeTests
    {
        [Test]
        public void SampleTest()
        {
            var transform = Transform.Translate(1, 0, 0);
            var shape = new DummyShape(transform, transform.Inverse(), false);
            var sample = shape.Sample(new Interaction(), Point2F.Zero);
            Check.That(sample.P).IsEqualTo(Point3F.Zero);
            Check.That(sample.Time).IsEqualTo(1.23f);
        }
        
        [Test]
        public void PdfTest()
        {
            var transform = Transform.Translate(1, 0, 0);
            var shape = new DummyShape(transform, transform.Inverse(), false);
            var pdf = shape.Pdf(new Interaction());
            Check.That(pdf).IsEqualTo(1/MathF.PI);
        }
        
        [Test]
        public void Pdf_NoIntersectTest()
        {
            Medium inside = HomogeneousMedium.Default();
            Medium outside = HomogeneousMedium.Default();
            MediumInterface mediumInterface = new MediumInterface(inside, outside);
            Point3F point = new Point3F(1, 1, 1);
            Normal3F normal = new Normal3F(0, 1, 0);
            Vector3F pError = new Vector3F(0.1f, 0.1f, 0.1f);
            Vector3F wo = new Vector3F(0.1f, 0.1f, 0.1f);
            float time = 1.23f;
            
            var interaction = new Interaction(point, normal, pError, wo, time, mediumInterface);
            Vector3F wi = new Vector3F(-10, 0, 1);
            var transform = Transform.Translate(1, 0, 0);
            var shape = new DummyShape(transform, transform.Inverse(), false);
            var pdf = shape.Pdf(interaction, wi);
            Check.That(pdf).IsEqualTo(0f);
        }

        private class DummyShape : AbstractShape
        {
            public DummyShape(Transform transform, Transform inverse, bool reverseOrientation) : base(transform, inverse, reverseOrientation)
            {
            }

            public override float Area => MathF.PI;

            public override bool Intersect(Ray ray, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true)
            {
                tHit = 0;
                isect = null;
                return false;
            }

            public override Interaction Sample(Point2F u)
            {
                return new Interaction(Point3F.Zero, 1.23f, new MediumInterface(null));
            }
        }
    }
}