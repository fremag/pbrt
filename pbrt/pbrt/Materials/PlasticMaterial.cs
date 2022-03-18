using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using pbrt.Textures;

namespace pbrt.Materials
{
    public class PlasticMaterial : Material
    {
        public Texture<Spectrum> Kd { get; }
        public Texture<Spectrum> Ks { get; }
        public Texture<float> Roughness { get; }
        public Texture<float> BumpMap { get; }
        public bool RemapRoughness { get; }

        public PlasticMaterial(Texture<Spectrum> kd,
            Texture<Spectrum> ks,
            Texture<float> roughness,
            Texture<float> bumpMap,
            bool remapRoughness)
        {
            Kd = kd;
            Ks = ks;
            Roughness = roughness;
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

            // Initialize diffuse component of plastic material
            Spectrum kd = Kd.Evaluate(si);
            kd.Clamp();
            if (!kd.IsBlack())
            {
                si.Bsdf.Add(new LambertianReflection(kd));
            }

            // Initialize specular component of plastic material
            Spectrum ks = Ks.Evaluate(si);
            ks.Clamp();
            if (!ks.IsBlack()) 
            {
                Fresnel fresnel = new FresnelDielectric(1f, 1.5f);
                // Create microfacet distribution distrib for plastic material 
                float rough = Roughness.Evaluate(si);
                if (RemapRoughness)
                {
                    rough = TrowbridgeReitzDistribution.RoughnessToAlpha(rough);
                }

                MicrofacetDistribution distrib = new TrowbridgeReitzDistribution(rough, rough);                
                BxDF spec = new MicrofacetReflection(ks, distrib, fresnel);
                si.Bsdf.Add(spec);
            }            
        }
    }
}