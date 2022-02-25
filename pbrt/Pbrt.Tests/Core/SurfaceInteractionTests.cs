using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Shapes;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class SurfaceInteractionTests
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
        float time = 1.23f;
        private SurfaceInteraction si;

        [SetUp]
        public void SetUp()
        {
            si = new SurfaceInteraction(p, pError, uv, wo, dpdu, dpdv, dndu, dndv, time, shape);
        }
        
        [Test]
        public void BasicTest()
        {
            Check.That(si.Bsdf).IsNull();
            Check.That(si.Bssrdf).IsNull();
            Check.That(si.Primitive).IsNull();
            Check.That(si.Shading.N).IsEqualTo(new Normal3F(0,1,0));
            Check.That(si.Shading.DnDu).IsEqualTo(dndu);
            Check.That(si.Shading.DnDv).IsEqualTo(dndv);
            Check.That(si.Shading.DpDu).IsEqualTo(dpdu);
            Check.That(si.Shading.DpDv).IsEqualTo(dpdv);
            Check.That(si.Shape).IsSameReferenceAs(shape);
            Check.That(si.P).IsEqualTo(p);
            Check.That(si.PError).IsEqualTo(pError);
            Check.That(si.Uv).IsEqualTo(uv);
            Check.That(si.Wo).IsEqualTo(wo);
            Check.That(si.Time).IsEqualTo(time);
        }

        [Test]
        public void SetShadingGeometryTest()
        {
            Vector3F dpdus = new Vector3F(1,0,0);
            Vector3F dpdvs = new Vector3F(0,0,1);
            Normal3F dndus = new Normal3F(0, 0.9f, 0);
            Normal3F dndvs = new Normal3F(0, 1.1f, 0);
            
            si.SetShadingGeometry(dpdus, dpdvs, dndus, dndvs, true);

            Check.That(si.Shading.N).IsEqualTo(new Normal3F(0,-1,0));
            Check.That(si.Shading.DpDu).IsEqualTo(dpdus);
            Check.That(si.Shading.DpDv).IsEqualTo(dpdvs);
            Check.That(si.Shading.DnDu).IsEqualTo(dndus);
            Check.That(si.Shading.DnDv).IsEqualTo(dndvs);

            shape.ReverseOrientation = true;
            si.SetShadingGeometry(dpdus, dpdvs, dndus, dndvs, true);
            Check.That(si.Shading.N).IsEqualTo(new Normal3F(0, 1,0));

            shape.ReverseOrientation = true;
            si.SetShadingGeometry(dpdus, dpdvs, dndus, dndvs, false);
            Check.That(si.Shading.N).IsEqualTo(new Normal3F(0, 1,0));
        }
    }
}