using System;
using pbrt.Core;
using pbrt.Textures;

namespace pbrt.Materials
{
    public abstract class Material : IMaterial
    {
        public abstract void ComputeScatteringFunctions(SurfaceInteraction si, MemoryArena arena, TransportMode mode, bool allowMultipleLobes);

        public static void Bump(Texture<float> d, SurfaceInteraction si)
        {
            // Compute offset positions and evaluate displacement texture 
            SurfaceInteraction siEval = si;
            // Shift siEval du in the u direction
            float du = .5f * (MathF.Abs(si.DuDx) + MathF.Abs(si.DuDy));
            if (du == 0)
            {
                du = .01f;
            }

            siEval.P = si.P + du * si.Shading.DpDu;
            siEval.Uv = si.Uv + new Vector2F(du, 0f);
            var dudvN = si.Shading.DpDu.Cross(si.Shading.DpDv);
            Normal3F siDnDu = du * si.DnDu;
            var n = new Normal3F(dudvN.X + siDnDu.X, dudvN.Y + siDnDu.Y, dudvN.Z + siDnDu.Z);
            siEval.N = n.Normalize();
            float uDisplace = d.Evaluate(siEval);
            
            // Shift siEval dv in the v direction 
            float dv = .5f * (MathF.Abs(si.DvDx) + MathF.Abs(si.DvDy));
            if (dv == 0)
            {
                dv = .01f;
            }

            siEval.P = si.P + dv * si.Shading.DpDv;
            siEval.Uv = si.Uv + new Vector2F(0f, dv);
            var siDnDv = dv * si.DnDv;
            n = new Normal3F(dudvN.X + siDnDv.X, dudvN.Y + siDnDv.Y, dudvN.Z + siDnDv.Z);
            siEval.N = n.Normalize();

            float vDisplace = d.Evaluate(siEval);
            float displace = d.Evaluate(si);            

            // Compute bump-mapped differential geometry 
            Vector3F dpdu = si.Shading.DpDu + (uDisplace - displace) / du * new Vector3F(si.Shading.N) + displace * new Vector3F(si.Shading.DnDu);
            Vector3F dpdv = si.Shading.DpDv + (vDisplace - displace) / dv * new Vector3F(si.Shading.N) + displace * new Vector3F(si.Shading.DnDv);
            si.SetShadingGeometry(dpdu, dpdv, si.Shading.DnDu, si.Shading.DnDv, false);            
        }
    }
}