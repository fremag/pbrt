using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Materials;
using pbrt.Spectrums;
using Pbrt.Tests.Core;
using pbrt.Textures;

namespace Pbrt.Tests.Materials
{
    public class DummyTexture<T> : Texture<T>
    {
        public T  Value{ get; }

        public DummyTexture(T value)
        {
            Value = value;
        }

        public override T Evaluate(SurfaceInteraction si)
        {
            return Value;
        }
    }

    [TestFixture]
    public class PlasticMaterialTests
    {
        private readonly Texture<Spectrum> kd = new DummyTexture<Spectrum>(new Spectrum(1f));
        private readonly Texture<Spectrum> ks = new DummyTexture<Spectrum>(new Spectrum(2f));
        private readonly Texture<float> roughness = new DummyTexture<float>(1.23f);
        private readonly Texture<float> bumpMap = new DummyTexture<float>(2.34f);
        private PlasticMaterial plastic;

        [SetUp]
        public void Setup()
        {
            plastic = new PlasticMaterial(kd, ks, roughness, bumpMap, true);
        }

        [Test]
        public void ComputeScatteringFunctionsTest()
        {
            Normal3F dndu = new Normal3F(0.1f, 0f, 0);
            Normal3F dndv = new Normal3F(0f, 0.1f, 0);
            Vector3F dpdu = new Vector3F(0.01f, 0f, 0f);
            Vector3F dpdv = new Vector3F(0f, 0.01f, 0f);

            SurfaceInteraction si = new SurfaceInteraction
            {
                P = Point3F.Zero,
                Uv = Point2F.Zero,
                DnDu = new Normal3F(-0.01f, 0, 1),
                DnDv = new Normal3F(0, 0.01f, 1),
                Shading = new Shading() { DpDu = dpdu, DpDv = dpdv, DnDu = dndu, DnDv = dndv, N = new Normal3F(0, 0, 1f) }
            };
            plastic.ComputeScatteringFunctions(si, null, TransportMode.Radiance, true);
            Check.That(si.Bsdf).IsNotNull();
            Check.That(si.Bsdf.Eta).IsEqualTo(1);
            Check.That(si.Bsdf.NbBxDFs).IsEqualTo(2);
            Check.That(si.Bsdf.Ng).Check((0, 0.009900505f, 0.99995095f)); // because of BumpMapping
            Check.That(si.Bsdf.Ns).Check((0, 0, 1));
            Check.That(si.Bsdf.Ss).Check((1, 0, 0));
            Check.That(si.Bsdf.Ts).Check((0, 1, 0));
        }
    }
}