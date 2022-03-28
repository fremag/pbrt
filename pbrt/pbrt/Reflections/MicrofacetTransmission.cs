using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class MicrofacetTransmission : BxDF
    {
        public Spectrum T { get; }
        public MicrofacetDistribution Distribution { get; }
        public float EtaA { get; }
        public float EtaB { get; }
        public TransportMode Mode { get; }
        public Fresnel Fresnel { get; set; }

        public MicrofacetTransmission(Spectrum t, MicrofacetDistribution distribution, float etaA, float etaB, TransportMode mode)
            : base(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_GLOSSY)
        {
            T = t;
            Distribution = distribution;
            EtaA = etaA;
            EtaB = etaB;
            Mode = mode;
            Fresnel = new FresnelDielectric(etaA, etaB);
        }

        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            if (SameHemisphere(wo, wi))
            {
                return new Spectrum(0f);  // transmission only
            }

            float cosThetaO = CosTheta(wo);
            float cosThetaI = CosTheta(wi);
            if (cosThetaI == 0 || cosThetaO == 0)
            {
                return new Spectrum(0f);
            }

            // Compute $\wh$ from $\wo$ and $\wi$ for microfacet transmission
            float eta = CosTheta(wo) > 0 ? (EtaB / EtaA) : (EtaA / EtaB);
            Vector3F wh = (wo + wi * eta).Normalized();
            if (wh.Z < 0)
            {
                wh = -wh;
            }

            // Same side?
            if (wo.Dot(wh) * wi.Dot(wh) > 0)
            {
                return new Spectrum(0);
            }

            Spectrum F = Fresnel.Evaluate(wo.Dot(wh));

            float sqrtDenom = wo.Dot(wh) + eta * wi.Dot(wh);
            float factor = (Mode == TransportMode.Radiance) ? (1 / eta) : 1;

            var f1 = (cosThetaI * cosThetaO * sqrtDenom * sqrtDenom);
            var f3 = Distribution.D(wh) * Distribution.G(wo, wi) * eta * eta;
            var f2 = f3 * wi.AbsDot(wh) * wo.AbsDot(wh) * factor * factor / f1;
            return (new Spectrum(1f) - F) * T * MathF.Abs(f2);            
        } 
        
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F u, out float pdf, out BxDFType sampledType) 
        {
            sampledType = BxdfType;
            Vector3F wh = Distribution.Sample_wh(wo, u);
            float eta = CosTheta(wo) > 0 ? (EtaA / EtaB) : (EtaB / EtaA);
            
            if (!BSDF.Refract(wo, new Normal3F(wh), eta, out wi))
            {
                pdf = 0;
                return new Spectrum(0f);
            }

            pdf = Pdf(wo, wi);
            return F(wo, wi);
        }
        
        public float Pdf(Vector3F wo, Vector3F wi) 
        {
            if (SameHemisphere(wo, wi))
            {
                return 0;
            }

            // Compute wh from wo and wi for microfacet transmission 
            float eta = CosTheta(wo) > 0 ? (EtaB / EtaA) : (EtaA / EtaB);
            Vector3F wh = (wo + wi * eta).Normalized();            
            // Compute change of variables dwh_dwi for microfacet transmission 
            float sqrtDenom = wo.Dot(wh) + eta * wi.Dot(wh);
            float dwh_dwi = MathF.Abs((eta * eta * wi.Dot(wh)) / (sqrtDenom * sqrtDenom));            
            return Distribution.Pdf(wo, wh) * dwh_dwi;
        }        
    }
}