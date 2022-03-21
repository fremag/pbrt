using NFluent;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Media;
using pbrt.Shapes;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class GeometricPrimitiveTests
    {
        private readonly IShape shape = Substitute.For<IShape>();
        private readonly IMaterial material = Substitute.For<IMaterial>();

        private readonly MediumInterface mediumInterface = new MediumInterface(null);
        private readonly AreaLight light = new AreaLight();
        private GeometricPrimitive geoPrim;

        [SetUp]
        public void SetUp()
        {
            geoPrim = new GeometricPrimitive
            {
                Shape = shape,
                Material = material,
                AreaLight = light,
                MediumInterface = mediumInterface
            };
        } 
        [Test]
        public void BasicTest()
        {
            Check.That(geoPrim.GetMaterial()).IsEqualTo(material);
            Check.That(geoPrim.GetAreaLight()).IsEqualTo(light);
        }

        [Test]
        public void WorldBoundTest()
        {
            var worldBounds = new Bounds3F();
            shape.WorldBound().Returns(worldBounds);
            Check.That(geoPrim.WorldBound()).IsEqualTo(worldBounds);
        }
 
        [Test]
        public void IntersectPTest()
        {
            var ray1 = new Ray();
            var ray2 = new Ray();
            shape.IntersectP(ray1).Returns(true);
            shape.IntersectP(ray2).Returns(false);
            Check.That(geoPrim.IntersectP(ray1)).IsTrue();
            Check.That(geoPrim.IntersectP(ray2)).IsFalse();
        }

        [Test]
        public void IntersectTest()
        {
            var medium = HomogeneousMedium.Default();
            var ray = new Ray(Point3F.Zero, new Vector3F(1,1,1), 1000f, 1f, medium);
            var tHit = 1.23f;
            var surfaceInteraction = new SurfaceInteraction();
            
            shape.Intersect(ray, out Arg.Any<float>(), out Arg.Any<SurfaceInteraction>()).Returns(false);
            var inter = geoPrim.Intersect(ray, out var surf);
            Check.That(inter).IsFalse();
            Check.That(surf).IsNull();
            
            shape.ClearSubstitute();
            shape.Intersect(ray, out Arg.Any<float>(), out Arg.Any<SurfaceInteraction>()).Returns( datas =>
            {
                datas[1] = tHit;
                datas[2] = surfaceInteraction;
                return true;
            });

            inter = geoPrim.Intersect(ray, out surf);
            Check.That(inter).IsTrue();
            Check.That(surf).IsSameReferenceAs(surfaceInteraction);
            Check.That(ray.TMax).IsEqualTo(tHit);
            Check.That(surf.MediumInterface.Inside).IsSameReferenceAs(medium);
            Check.That(surf.MediumInterface.Outside).IsSameReferenceAs(medium);
            
            mediumInterface.Inside = HomogeneousMedium.Default(); 
            mediumInterface.Outside = HomogeneousMedium.Default(); 

            inter = geoPrim.Intersect(ray, out surf);
            Check.That(inter).IsTrue();
            Check.That(surf).IsSameReferenceAs(surfaceInteraction);
            Check.That(ray.TMax).IsEqualTo(tHit);
            Check.That(surf.MediumInterface).IsSameReferenceAs(mediumInterface);
        }

        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            var surfaceInteraction = new SurfaceInteraction();
            MemoryArena memoryArena = new MemoryArena();
            var transportMode = new TransportMode();
            geoPrim.ComputeScatteringFunctions(surfaceInteraction, memoryArena, transportMode, true);
            
            material.Received(1).ComputeScatteringFunctions(surfaceInteraction, memoryArena, transportMode, true);
        }
    }
}