using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Materials;
using pbrt.Reflections;
using pbrt.Shapes;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Materials
{
    [TestFixture]
    public class MirrorMaterialTests
    {
        Texture<Spectrum> r = new ConstantTexture<Spectrum>(new Spectrum(1f));
        Texture<float> bumpMap = new ConstantTexture<float>(0);
        private MirrorMaterial mirror;
        AbstractShape shape = new Sphere(Transform.Translate(0,0,0), 1);

        [SetUp]
        public void SetUp()
        {
            mirror = new MirrorMaterial(r, bumpMap);
        }

        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            var si = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, Vector3F.Zero, Vector3F.Zero, Vector3F.Zero, new Normal3F(), new Normal3F(), 1, shape);
            mirror.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOfType(typeof(SpecularReflection));
        }
        
        [Test]
        public void ComputeScatteringFunctions_NoReflectionTest()
        {
            mirror = new MirrorMaterial(new ConstantTexture<Spectrum>(new Spectrum(0f)), bumpMap);
            var si = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, Vector3F.Zero, Vector3F.Zero, Vector3F.Zero, new Normal3F(), new Normal3F(), 1, shape);
            mirror.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(0);
        }
    }
}