using NFluent;
using NSubstitute;
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

        [Test]
        public void Orthogonal_ComputeDifferentialsTest()
        {
            Check.That(si.DpDx).IsNull();
            Check.That(si.DpDy).IsNull();

            Point3F o = new Point3F(-10, 0, 0);
            var dir = new Vector3F(1, 0, 0);
            var rayDiff = new RayDifferential(o, dir)
            {
                HasDifferentials = true,
                RxOrigin = new Point3F(-1, 0, 0),
                RyOrigin = new Point3F(-1, 0, 0),
                RxDirection = new Vector3F(1, 0, 0),
                RyDirection = new Vector3F(1, 0, 0)
            };

            si.ComputeDifferentials(rayDiff);

            Check.That(si.DvDx).IsEqualTo(0);
            Check.That(si.DvDy).IsEqualTo(0);
            Check.That(si.DuDx).IsEqualTo(0);
            Check.That(si.DuDy).IsEqualTo(0);
            Check.That(si.DpDx).IsEqualTo(Vector3F.Zero);
            Check.That(si.DpDy).IsEqualTo(Vector3F.Zero);
        }
        
        [Test]
        public void NoDifferentials_ComputeDifferentialsTest()
        {
            Check.That(si.DpDx).IsNull();
            Check.That(si.DpDy).IsNull();
            
            RayDifferential rayDiff = new RayDifferential(null, null)
            {
                HasDifferentials = false
            };

            si.ComputeDifferentials(rayDiff);

            Check.That(si.DvDx).IsEqualTo(0);
            Check.That(si.DvDy).IsEqualTo(0);
            Check.That(si.DuDx).IsEqualTo(0);
            Check.That(si.DuDy).IsEqualTo(0);
            Check.That(si.DpDx).IsEqualTo(Vector3F.Zero);
            Check.That(si.DpDy).IsEqualTo(Vector3F.Zero);
        }
        
        [Test]
        public void ComputeDifferentials_1_Test()
        {
            Check.That(si.DpDx).IsNull();
            Check.That(si.DpDy).IsNull();

            Point3F o = new Point3F(-10, 0, 0);
            var dir = new Vector3F(1, 0, 0);
            var rayDiff = new RayDifferential(o, dir)
            {
                HasDifferentials = true,
                RxOrigin = new Point3F(-1, 0, 0),
                RyOrigin = new Point3F(-1, 0, 0),
                RxDirection = new Vector3F(1, -1, 0),
                RyDirection = new Vector3F(-1, -1, 0)
            };

            si.ComputeDifferentials(rayDiff);

            Check.That(si.DvDx).IsEqualTo(0);
            Check.That(si.DvDy).IsEqualTo(-2);
            Check.That(si.DuDx).IsEqualTo(0);
            Check.That(si.DuDy).IsEqualTo(0);
            Check.That(si.DpDx).IsEqualTo(new Vector3F(0, -2f, 0));
            Check.That(si.DpDy).IsEqualTo(new Vector3F(-2f , -2f, 0));
        }
        
        [Test]
        public void ComputeDifferentials_2_Test()
        {
            Check.That(si.DpDx).IsNull();
            Check.That(si.DpDy).IsNull();

            Point3F o = new Point3F(-10, 0, 0);
            var dir = new Vector3F(1, 0, 0);
            var rayDiff = new RayDifferential(o, dir)
            {
                HasDifferentials = true,
                RxOrigin = new Point3F(-1, 0, 0),
                RyOrigin = new Point3F(-1, 0, 0),
                RxDirection = new Vector3F(-1, -1, 0),
                RyDirection = new Vector3F(1, -1, 0)
            };

            var newDpDu = new Vector3F(0, 1, 0);
            var newDpDv = new Vector3F(0, 0, 1);
            si = new SurfaceInteraction(p, pError, uv, wo, newDpDu, newDpDv, dndu, dndv, time, shape);
            si.ComputeDifferentials(rayDiff);

            Check.That(si.DvDx).IsEqualTo(0f);
            Check.That(si.DvDy).IsEqualTo(0);
            Check.That(si.DuDx).IsEqualTo(0);
            Check.That(si.DuDy).IsEqualTo(-2f);
            Check.That(si.DpDx).IsEqualTo(new Vector3F(0, 0, 0));
            Check.That(si.DpDy).IsEqualTo(new Vector3F(0 , -2f, 0));
        }
        
        [Test]
        public void ComputeDifferentials_3_Test()
        {
            Check.That(si.DpDx).IsNull();
            Check.That(si.DpDy).IsNull();

            Point3F o = new Point3F(-10, 0, 0);
            var dir = new Vector3F(1, 0, 0);
            var rayDiff = new RayDifferential(o, dir)
            {
                HasDifferentials = true,
                RxOrigin = new Point3F(0, 0, -1),
                RyOrigin = new Point3F(0, 0, 1),
                RxDirection = new Vector3F(0, 0, 1),
                RyDirection = new Vector3F(0, -1, 1)
            };

            var newDpDu = new Vector3F(0, 1, 0);
            var newDpDv = new Vector3F(1, 0, 0);
            si = new SurfaceInteraction(p, pError, uv, wo, newDpDu, newDpDv, dndu, dndv, time, shape);
            si.ComputeDifferentials(rayDiff);

            Check.That(si.DvDx).IsEqualTo(0f);
            Check.That(si.DvDy).IsEqualTo(0);
            Check.That(si.DuDx).IsEqualTo(-1f);
            Check.That(si.DuDy).IsEqualTo(0f);
            Check.That(si.DpDx).IsEqualTo(new Vector3F(0, -1f, 0));
            Check.That(si.DpDy).IsEqualTo(new Vector3F(0 , 0, 0));
        }
        
        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            RayDifferential ray = new RayDifferential(Point3F.Zero, Vector3F.Zero);
            MemoryArena arena = new MemoryArena();
            si.Primitive = Substitute.For<IPrimitive>();
            si.ComputeScatteringFunctions(ray, arena, true, TransportMode.Radiance);
            
            si.Primitive.Received(1).ComputeScatteringFunctions(si, arena, TransportMode.Radiance, true);
        }
        
        
    }
}