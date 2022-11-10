using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Materials;
using pbrt.Reflections;
using pbrt.Spectrums;
using pbrt.Textures;

namespace Pbrt.Tests.Materials;

[TestFixture]
public class MetalMaterialTests
{
    Texture<Spectrum> eta = new ConstantTexture<Spectrum>(new Spectrum(1.23f));
    Texture<Spectrum> k = new ConstantTexture<Spectrum>(new Spectrum(2.34f));
    Texture<float> roughness = new ConstantTexture<float>(0.1f);

    Texture<float> uRoughness = new ConstantTexture<float>(0.2f);
    Texture<float> vRoughness = new  ConstantTexture<float>(0.3f);

    Texture<float> bumpMap = new ConstantTexture<float>(0.4f);

    [Test]
    public void BasicTest()
    {
        var metal = new MetalMaterial(eta, k, roughness, uRoughness, vRoughness, bumpMap, false);
        SurfaceInteraction si = new SurfaceInteraction
        {
            P = Point3F.Zero,
            Uv = Point2F.Zero,
            DnDu = new Normal3F(-0.01f, 0, 1),
            DnDv = new Normal3F(0, 0.01f, 1),
            Shading = new Shading { DpDu = Vector3F.Zero, DpDv = Vector3F.Zero, DnDu = new Normal3F(), DnDv = new Normal3F(), N = new Normal3F(0, 0, 1f) }
        };
        metal.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
        Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
        Check.That(si.Bsdf.bxdfs[0]).IsInstanceOf<MicrofacetReflection>();
        var microFacet = (MicrofacetReflection)si.Bsdf.bxdfs[0];
        var distribution = (TrowbridgeReitzDistribution)microFacet.Distribution;
        Check.That(distribution.AlphaX).IsCloseTo(0.2f, 0.001f);
        Check.That(distribution.AlphaY).IsCloseTo(0.3f, 0.001f);
    }

    [Test]
    public void NoUVRoughness_BasicTest()
    {
        var metal = new MetalMaterial(eta, k, roughness, null, null, bumpMap, true);
        SurfaceInteraction si = new SurfaceInteraction
        {
            P = Point3F.Zero,
            Uv = Point2F.Zero,
            DnDu = new Normal3F(-0.01f, 0, 1),
            DnDv = new Normal3F(0, 0.01f, 1),
            Shading = new Shading { DpDu = Vector3F.Zero, DpDv = Vector3F.Zero, DnDu = new Normal3F(), DnDv = new Normal3F(), N = new Normal3F(0, 0, 1f) }
        };
        metal.ComputeScatteringFunctions(si, null, TransportMode.Radiance, false);
        Check.That(si.Bsdf.NbBxDFs).IsEqualTo(1);
        Check.That(si.Bsdf.bxdfs[0]).IsInstanceOf<MicrofacetReflection>();
        var microFacet = (MicrofacetReflection)si.Bsdf.bxdfs[0];
        var distribution = (TrowbridgeReitzDistribution)microFacet.Distribution;
        Check.That(distribution.AlphaX).IsCloseTo(0.461f, 0.001f);
        Check.That(distribution.AlphaY).IsCloseTo(0.461f, 0.001f);
    }
}