using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using pbrt.Textures;

namespace pbrt.Materials
{
    public class GlassMaterial : Material
    {
        public Texture<Spectrum> Kr { get; }
        public Texture<Spectrum> Kt { get; }
        public Texture<float> URoughness { get; }
        public Texture<float> VRoughness { get; }
        public Texture<float> Index { get; }
        public Texture<float> BumpMap { get; }
        public bool RemapRoughness { get; }

        public GlassMaterial(Texture<Spectrum> kr, Texture<Spectrum> kt, 
            Texture<float> uRoughness, Texture<float> vRoughness, 
            Texture<float> index, Texture<float> bumpMap, bool remapRoughness)
        {
            Kr = kr;
            Kt = kt;
            URoughness = uRoughness;
            VRoughness = vRoughness;
            Index = index;
            BumpMap = bumpMap;
            RemapRoughness = remapRoughness;
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes)
        {
            // Perform bump mapping with _bumpMap_, if present
            if (BumpMap != null)
            {
                Bump(BumpMap, si);
            }

            float eta = Index.Evaluate(si);
            float urough = URoughness.Evaluate(si);
            float vrough = VRoughness.Evaluate(si);
            Spectrum R = Kr.Evaluate(si);
            R.Clamp();
            Spectrum T = Kt.Evaluate(si);
            T.Clamp();
            
            // Initialize _bsdf_ for smooth or rough dielectric
            si.Bsdf = new BSDF(si, eta);

            if (R.IsBlack() && T.IsBlack())
            {
                return;
            }

            bool isSpecular = urough == 0 && vrough == 0;
            if (isSpecular && allowMultipleLobes) 
            {
                si.Bsdf.Add(new FresnelSpecular(R, T, 1f, eta, mode));
            }
            else 
            {
                if (RemapRoughness)
                {
                    urough = TrowbridgeReitzDistribution.RoughnessToAlpha(urough);
                    vrough = TrowbridgeReitzDistribution.RoughnessToAlpha(vrough);
                }
                
                MicrofacetDistribution distrib = isSpecular ? null : new TrowbridgeReitzDistribution(urough, vrough);
                if (!R.IsBlack()) 
                {
                    Fresnel fresnel = new FresnelDielectric(1f, eta);
                    if (isSpecular)
                    {
                        si.Bsdf.Add(new SpecularReflection(R, fresnel));
                    }
                    else
                    {
                        si.Bsdf.Add(new MicrofacetReflection(R, distrib, fresnel));
                    }
                }
                if (!T.IsBlack()) 
                {
                    if (isSpecular)
                    {
                        si.Bsdf.Add(new SpecularTransmission(T, 1f, eta, mode));
                    }
                    else
                    {
                        si.Bsdf.Add(new MicrofacetTransmission(T, distrib, 1f, eta, mode));
                    }
                }
            }
        }
    }
}