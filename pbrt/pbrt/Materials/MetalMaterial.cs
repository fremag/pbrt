using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using pbrt.Textures;

namespace pbrt.Materials;

public class MetalMaterial : Material
{
    public Texture<Spectrum> Eta { get; }
    public Texture<Spectrum> K { get; }
    public Texture<float> Roughness { get; }
    public Texture<float> URoughness { get; }
    public Texture<float> VRoughness { get; }
    public Texture<float> BumpMap { get; }
    public bool RemapRoughness { get; }

    public MetalMaterial(Texture<Spectrum> eta,
        Texture<Spectrum> k,
        Texture<float> roughness,
        Texture<float> uRoughness,
        Texture<float> vRoughness,
        Texture<float> bumpMap,
        bool remapRoughness)
    {
        Eta = eta;
        K = k;
        Roughness = roughness;
        URoughness = uRoughness;
        VRoughness = vRoughness;
        BumpMap = bumpMap;
        RemapRoughness = remapRoughness;
    }
    
    public override void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes)
    {
        // Perform bump mapping with bumpMap, if present
        if (BumpMap != null)
        {
            Bump(BumpMap, si);
        }

        si.Bsdf = new BSDF(si);

        float uRough = URoughness != null ? URoughness.Evaluate(si) : Roughness.Evaluate(si);
        float vRough = VRoughness != null ? VRoughness.Evaluate(si) : Roughness.Evaluate(si);
        if (RemapRoughness) {
            uRough = TrowbridgeReitzDistribution.RoughnessToAlpha(uRough);
            vRough = TrowbridgeReitzDistribution.RoughnessToAlpha(vRough);
        }
        
        Fresnel frMf = new FresnelConductor(new Spectrum(1f), Eta.Evaluate(si), K.Evaluate(si));
        MicrofacetDistribution distrib = new TrowbridgeReitzDistribution(uRough, vRough);
        si.Bsdf.Add(new MicrofacetReflection(new Spectrum(1f), distrib, frMf));
    }
}