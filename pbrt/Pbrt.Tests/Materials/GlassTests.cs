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
    public class GlassTests
    {
        Texture<Spectrum> kr = new ConstantTexture<Spectrum>(new Spectrum(0.6f));
        Texture<Spectrum> kt = new ConstantTexture<Spectrum>(new Spectrum(1f));
        Texture<float> uRoughness = new ConstantTexture<float>(0f);
        Texture<float> vRoughness = new ConstantTexture<float>(0f);
        Texture<float> index = new ConstantTexture<float>(3f);
        AbstractShape shape = new Sphere(Transform.Translate(0,0,0), 1);
        private SurfaceInteraction si;

        [SetUp]
        public void SetUp()
        {
            si = new SurfaceInteraction(Point3F.Zero, Vector3F.Zero, Point2F.Zero, Vector3F.Zero, Vector3F.Zero, Vector3F.Zero, new Normal3F(), new Normal3F(), 1, shape);
        }

        [Test]
        public void ComputeScatteringFunctions_SpecularTest()
        {
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, new ConstantTexture<float>(0), false);
            glassMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(2);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOfType(typeof(SpecularReflection));
            Check.That(si.Bsdf.bxdfs[1]).IsInstanceOfType(typeof(SpecularTransmission));
        }
        
        [Test]
        public void ComputeScatteringFunctions_MicroFacetTest()
        {
            IMaterial glassMaterial = new GlassMaterial(kr, kt, new ConstantTexture<float>(1f), vRoughness, index, new ConstantTexture<float>(0), false);
            glassMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(2);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOfType(typeof(MicrofacetReflection));
            Check.That(si.Bsdf.bxdfs[1]).IsInstanceOfType(typeof(MicrofacetTransmission));
        }
        
        [Test]
        public void ComputeScatteringFunctions_BlackTest()
        {
            IMaterial glassMaterial = new GlassMaterial(new ConstantTexture<Spectrum>(new Spectrum(0)), new ConstantTexture<Spectrum>(new Spectrum(0)), uRoughness, vRoughness, index, new ConstantTexture<float>(0), false);
            glassMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(0);
        }
        
        [Test]
        public void ComputeScatteringFunctions_FresnelSpecularTest()
        {
            IMaterial glassMaterial = new GlassMaterial(kr, kt, uRoughness, vRoughness, index, new ConstantTexture<float>(0), true);
            glassMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOfType(typeof(FresnelSpecular));
        }
        [Test]
        public void ComputeScatteringFunctions_SpecularRemap_NoTransmissionTest()
        {
            IMaterial glassMaterial = new GlassMaterial(kr, new ConstantTexture<Spectrum>(new Spectrum(0)), uRoughness, vRoughness, index, new ConstantTexture<float>(0), true);
            glassMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOfType(typeof(SpecularReflection));
        }
    }
}