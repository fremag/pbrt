using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class SpecularTransmission : BxDF
    {
        public Spectrum T { get; }
        public float EtaA { get; }
        public float EtaB { get; }
        public TransportMode Mode { get; }
        public Fresnel  Fresnel { get; }
        
        public SpecularTransmission(Spectrum t, float etaA, float etaB, TransportMode mode)
            : base(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)
        {
            T = t;
            EtaA = etaA;
            EtaB = etaB;
            Mode = mode;
            Fresnel = new FresnelDielectric(etaA, etaB);
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi) => new Spectrum(0f);
        
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
        {
            sampledType = BxdfType;
            pdf = 1;
            
            // Figure out which eta is incident and which is transmitted 
            bool entering = CosTheta(wo) > 0;
            float etaI = entering ? EtaA : EtaB;
            float etaT = entering ? EtaB : EtaA;
            
            // Compute ray direction for specular transmission 
            var faceForward = (new Normal3F(0, 0, 1).FaceForward(wo));
            if (BSDF.Refract(wo, faceForward, etaI / etaT, out wi))
            {
                Spectrum ft = T * (new Spectrum(1f) - Fresnel.Evaluate(CosTheta(wi)));
                // Account for non-symmetry with transmission to different medium 
                return ft / AbsCosTheta(wi);
            }

            pdf = 0;
            return new Spectrum(0f);
        }
        
        public override float Pdf(Vector3F wo, Vector3F wi) => 0;
    }
}