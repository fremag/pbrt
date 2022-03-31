using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelSpecular : BxDF 
    {
        public Spectrum R { get; }
        public Spectrum T { get; }
        public float EtaA { get; }
        public float EtaB { get; }
        public TransportMode Mode { get; }
        public Fresnel Fresnel { get; }
        
        public FresnelSpecular(Spectrum r, Spectrum t, float etaA, float etaB, TransportMode mode)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)
        {
            R = r;
            T = t;
            EtaA = etaA;
            EtaB = etaB;
            Mode = mode;
            Fresnel = new FresnelDielectric(etaA, etaB);
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi) => new Spectrum(0f);
        
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F u, out float pdf, out BxDFType sampledType)
        {
            float f = FrDielectric(CosTheta(wo), EtaA, EtaB);
            if (u[0] < f) 
            {
                // Compute specular reflection for FresnelSpecular 
                // Compute perfect specular reflection direction>> 
                wi = new Vector3F(-wo.X, -wo.Y, wo.Z);

                sampledType = BxDFType.BSDF_SPECULAR | BxDFType.BSDF_REFLECTION;
                pdf = f;
                return f * R / AbsCosTheta(wi);                
            }

            // Compute specular transmission for FresnelSpecular 
            // Figure out which n is incident and which is transmitted 
            bool entering = CosTheta(wo) > 0;
            float etaI = entering ? EtaA : EtaB;
            float etaT = entering ? EtaB : EtaA;

            // Compute ray direction for specular transmission 
            var normal3F = new Normal3F(0, 0, 1).FaceForward(wo);
            var etaRatio = etaI / etaT;
            if (!BSDF.Refract(wo, normal3F, etaRatio, out wi))
            {
                pdf = 0;
                sampledType = BxDFType.BSDF_NONE;
                return new Spectrum(0f);
            }

            Spectrum ft = T * (1 - f);
            // Account for non-symmetry with transmission to different medium 
            if (Mode == TransportMode.Radiance)
            {
                ft *= (etaI * etaI) / (etaT * etaT);
            }

            sampledType = BxDFType.BSDF_SPECULAR | BxDFType.BSDF_TRANSMISSION;
            pdf = 1 - f;
            return ft / AbsCosTheta(wi);
        }        
    }
}