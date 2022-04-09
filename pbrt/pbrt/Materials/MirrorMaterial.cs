using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using pbrt.Textures;

namespace pbrt.Materials
{
    public class MirrorMaterial : Material
    {
        public Texture<float> BumpMap { get; }
        public Texture<Spectrum> Kr { get; }

        public MirrorMaterial(Texture<Spectrum> r, Texture<float> bumpMap) 
        {
            Kr = r;
            BumpMap = bumpMap;
        }

        public  override void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes) 
        {
            // Perform bump mapping with _bumpMap_, if present
            if (BumpMap != null)
            {
                Bump(BumpMap, si);
            }

            si.Bsdf = new BSDF(si);
            Spectrum R = Kr.Evaluate(si);
            R.Clamp();
            if (R.IsBlack())
            {
                return;
            }

            si.Bsdf.Add(new SpecularReflection(R, new FresnelNoOp()));
        }
    }
}