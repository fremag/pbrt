using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace pbrt.Materials
{
    public class MatteMaterial : Material
    {
        public Texture<float> Sigma { get; }
        public Texture<float> BumpMap { get; }
        Texture<Spectrum> Kd;

        public MatteMaterial(Texture<Spectrum> kd, Texture<float> sigma, Texture<float> bumpMap)
        {
            Kd = kd;
            Sigma = sigma;
            BumpMap = bumpMap;
        }

        public override void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes)
        {
            // Perform bump mapping with bumpMap, if present 
            if (BumpMap != null)
            {
                Bump(BumpMap, si);
            }

            // Evaluate textures for MatteMaterial material and allocate BRDF>>
            si.Bsdf = new BSDF(si);
            Spectrum r = Kd.Evaluate(si);
            r.Clamp();
            float sig = Sigma.Evaluate(si).Clamp(0, 90);
            if (r.IsBlack())
            {
                return;
            }

            if (sig == 0)
            {
                si.Bsdf.Add(new LambertianReflection(r));
            }
            else
            {
                si.Bsdf.Add(new OrenNayar(r, sig));
            }
        }
   }
}