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
                var bsdfReflection = bxdf.BxdfType & BxDFType.BSDF_REFLECTION;
                var bsdfTransmission = bxdf.BxdfType & BxDFType.BSDF_TRANSMISSION;
                var matchReflection = reflect && bsdfReflection != BxDFType.BSDF_NONE;
                var matchTransmission = !reflect && bsdfTransmission != BxDFType.BSDF_NONE;
                
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

        public int NumComponents(BxDFType flags = BxDFType.BSDF_ALL)
        {
            int num = 0;
            for (int i = 0; i < NbBxDFs; ++i)
            {
                if (bxdfs[i].MatchesFlags(flags))
                {
                    ++num;
                }
            }

            return num;
        }
        
        public Spectrum Sample_f(Vector3F woWorld, out Vector3F wiWorld, Point2F u, out float pdf, BxDFType type, ref BxDFType sampledType)
        {
            // Choose which BxDF to sample
            int matchingComps = NumComponents(type);
            if (matchingComps == 0) 
            {
                pdf = 0;
                wiWorld = new Vector3F(0,0,0);
                sampledType = BxDFType.BSDF_NONE;
                return new Spectrum(0f);
            }
            
            int comp = (int)MathF.Min(MathF.Floor(u[0] * matchingComps), matchingComps - 1);
            // Get BxDF pointer for chosen component
            BxDF bxdf = null;
            int count = comp;
            for (int i = 0; i < NbBxDFs; i++)
            {
                if (bxdfs[i].MatchesFlags(type) && count-- == 0) 
                {
                    bxdf = bxdfs[i];
                    break;
                }
            }

            // Remap BxDF sample u to (0, 1) x (0, 1) 
            Point2F uRemapped = new Point2F(u[0] * matchingComps - comp, u[1]);
            
            // Sample chosen BxDF 
            Vector3F wi;
            Vector3F wo = WorldToLocal(woWorld);
            pdf = 0;
            if (sampledType == BxDFType.BSDF_NONE)
            {
                sampledType = bxdf.BxdfType;
            }

            Spectrum f = bxdf.Sample_f(wo, out wi, uRemapped, out pdf, out sampledType);
            if (pdf == 0)
            {
                wiWorld = new Vector3F(0, 0, 0);
                sampledType = BxDFType.BSDF_NONE;
                return new Spectrum(0f);
            }

            wiWorld = LocalToWorld(wi);
            
            // Compute overall PDF with all matching BxDFs
            if ((bxdf.BxdfType & BxDFType.BSDF_SPECULAR)==0 && matchingComps > 1)
            {
                for (int i = 0; i < NbBxDFs; ++i)
                {
                    if (bxdfs[i] != bxdf && bxdfs[i].MatchesFlags(type))
                    {
                        pdf += bxdfs[i].Pdf(wo, wi);
                    }
                }
            }

            if (matchingComps > 1)
            {
                pdf /= matchingComps;
            }
            
            // Compute value of BSDF for sampled direction
            var checkSpecular = (bxdf.BxdfType & BxDFType.BSDF_SPECULAR) == 0;
            if (checkSpecular && matchingComps > 1) 
            {
                bool reflect = wiWorld.Dot(Ng) * woWorld.Dot(Ng) > 0;
                f = new Spectrum(0f);
                for (int i = 0; i < NbBxDFs; ++i)
                {
                    var matchesFlags = bxdfs[i].MatchesFlags(type);
                    var checkReflection = reflect && (bxdfs[i].BxdfType & BxDFType.BSDF_REFLECTION) !=0;
                    var checkTransmission = !reflect && (bxdfs[i].BxdfType & BxDFType.BSDF_TRANSMISSION) !=0;
                    if (matchesFlags && (checkReflection || checkTransmission))
                    {
                        f += bxdfs[i].F(wo, wi);
                    }
                }
            }
            return f;
        }
        
        public float Pdf(Vector3F woWorld, Vector3F wiWorld, BxDFType flags) 
        {
            if (NbBxDFs == 0)
            {
                return 0f;
            }

            Vector3F wo = WorldToLocal(woWorld);
            Vector3F wi = WorldToLocal(wiWorld);
            if (wo.Z == 0)
            {
                return 0f;
            }

            float pdf = 0f;
            int matchingComps = 0;
            for (int i = 0; i < NbBxDFs; ++i)
            {
                if (bxdfs[i].MatchesFlags(flags)) 
                {
                    ++matchingComps;
                    pdf += bxdfs[i].Pdf(wo, wi);
                }
            }

            float v = matchingComps > 0 ? pdf / matchingComps : 0f;
            return v;
        }        
    }
}