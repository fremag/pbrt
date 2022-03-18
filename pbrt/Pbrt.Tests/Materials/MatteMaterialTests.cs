using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Materials;
using pbrt.Reflections;
using pbrt.Spectrums;
using Pbrt.Tests.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Materials
{
    [TestFixture]
    public class MatteMaterialTests
    {
        private readonly Texture<Spectrum> kd = new DummyTexture<Spectrum>(new Spectrum(1f));
        private readonly Texture<Spectrum> kdBlack = new DummyTexture<Spectrum>(new Spectrum(0f));
        private readonly Texture<float> sigma  = new DummyTexture<float>(1.23f);
        private readonly Texture<float> bumpMap = new DummyTexture<float>(2.34f);
        private MatteMaterial matteMaterial;

        Normal3F dndu = new Normal3F(0.1f, 0f, 0);
        Normal3F dndv = new Normal3F(0f, 0.1f, 0);
        Vector3F dpdu = new Vector3F(0.01f, 0f, 0f);
        Vector3F dpdv = new Vector3F(0f, 0.01f, 0f);
        private SurfaceInteraction si;

        [SetUp]
        public void Setup()
        {
            si = new SurfaceInteraction
            {
                P = Point3F.Zero, 
                Uv = Point2F.Zero,
                DnDu = new Normal3F(-0.01f, 0, 1),
                DnDv = new Normal3F(0, 0.01f, 1),
                Shading = new Shading(){DpDu = dpdu, DpDv = dpdv, DnDu = dndu, DnDv = dndv, N = new Normal3F(0, 0, 1f)}
            };
        }

        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            matteMaterial = new MatteMaterial(kd, sigma, bumpMap);
            matteMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf).IsNotNull();
            Check.That(si.Bsdf.Eta).IsEqualTo(1);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOf<OrenNayar>();
            
            Check.That(si.Bsdf.Ng).Check((0, 0.009900505f, 0.99995095f)); // because of BumpMapping
            Check.That(si.Bsdf.Ns).Check((0, 0, 1));
            Check.That(si.Bsdf.Ss).Check((1, 0, 0));
            Check.That(si.Bsdf.Ts).Check((0, 1, 0));
        }

        [Test]
        public void Black_ComputeScatteringFunctionsTest()
        {
            matteMaterial = new MatteMaterial(kdBlack, sigma, bumpMap);
            matteMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf).IsNotNull();
            Check.That(si.Bsdf.Eta).IsEqualTo(1);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(0);
        }

        [Test]
        public void SigmaZero_ComputeScatteringFunctionsTest()
        {
            matteMaterial = new MatteMaterial(kd, new DummyTexture<float>(0f), bumpMap);
            matteMaterial.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf).IsNotNull();
            Check.That(si.Bsdf.Eta).IsEqualTo(1);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
            Check.That(si.Bsdf.bxdfs[0]).IsInstanceOf<LambertianReflection>();
        }
    }
}