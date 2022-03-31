using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class BSDF
    {
        public static readonly int MaxBxDFs = 8;
        public int NbBxDFs { get; private set; }
        public readonly BxDF[] bxdfs;

        public float Eta { get; }
        public Normal3F Ns { get; }
        public Normal3F Ng { get; }
        public Vector3F Ss { get; }
        public Vector3F Ts { get; }
        
        public BSDF(SurfaceInteraction si, float eta = 1)
        {
            Eta = eta;
            Ns = si.Shading.N;
            Ng = si.N;
            Ss = si.Shading.DpDu.Normalized();
            Ts = new Vector3F(Ns).Cross(Ss);
            bxdfs = new BxDF[MaxBxDFs];
        }
        
        public void Add(BxDF b) 
        {
            if (NbBxDFs >= MaxBxDFs)
            {
                throw new InvalidOperationException();
            }
            bxdfs[NbBxDFs++] = b;
        }
        
        public Vector3F WorldToLocal(Vector3F v)
        {
            var vx = v.Dot(Ss);
            var vy = v.Dot(Ts);
            var vz = v.Dot(Ns);
            return new Vector3F(vx, vy, vz);
        }
        
        public Vector3F LocalToWorld(Vector3F v)
        {
            var (x, y, z) = v;
            var vx = Ss.X * x + Ts.X * y + Ns.X * z;
            var vy = Ss.Y * x + Ts.Y * y + Ns.Y * z;
            var vz = Ss.Z * x + Ts.Z * y + Ns.Z * z;
            return new Vector3F(vx, vy, vz);
        }        
        
        public Spectrum F(Vector3F woW, Vector3F wiW, BxDFType flags = BxDFType.BSDF_ALL) 
        {
            var wi = WorldToLocal(wiW);
            var wo = WorldToLocal(woW);
                
            bool reflect = wiW.Dot(Ng) * woW.Dot(Ng) > 0;
            var f= new Spectrum(0f);
            
            for (int i = 0; i < NbBxDFs; ++i)
            {
                var bxdf = bxdfs[i];
                var matchesFlags = bxdf.MatchesFlags(flags);
                var matchReflection = reflect && (bxdf.BxdfType & BxDFType.BSDF_REFLECTION)!=0;
                var matchTransmission = !reflect && (bxdf.BxdfType & BxDFType.BSDF_TRANSMISSION)!=0;
                
                if (matchesFlags && (matchReflection || matchTransmission))
                {
                    f += bxdf.F(wo, wi);
                }
            }

            return f;
        }        
            
        public static Vector3F Reflect(Vector3F wo, Vector3F n) => -wo + 2 * wo.Dot(n) * n;

        public static bool Refract(Vector3F wi, Normal3F n, float etaRatio, out Vector3F wt)
        {
            // Compute cos theta using Descartes’s law 
            float cosThetaI = n.Dot(wi);
            float sin2ThetaI = MathF.Max(0f, 1 - cosThetaI * cosThetaI);
            float sin2ThetaT = etaRatio * etaRatio * sin2ThetaI;
            
            // Handle total internal reflection for transmission
            if (sin2ThetaT >= 1)
            {
                wt = null;
                return false;
            }

            float cosThetaT = MathF.Sqrt(1 - sin2ThetaT);
            wt = etaRatio * -wi + (etaRatio * cosThetaI - cosThetaT) * new Vector3F(n);
            return true;
        }

        public Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F u, out float pdf, BxDFType type)
        {
            throw new NotImplementedException();
        }
    }
}