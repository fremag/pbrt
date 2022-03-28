using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class MicrofacetReflection : BxDF
    {
        public Spectrum R { get; }
        public MicrofacetDistribution Distribution { get; }
        public Fresnel Fresnel { get; }

        public MicrofacetReflection(Spectrum r, MicrofacetDistribution distribution, Fresnel fresnel)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY)
        {
            R = r;
            Distribution = distribution;
            Fresnel = fresnel;
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            float cosThetaO = AbsCosTheta(wo);
            float cosThetaI = AbsCosTheta(wi);
            Vector3F wh = wi + wo;
            //Handle degenerate cases for microfacet reflection
            if (cosThetaI == 0 || cosThetaO == 0)
            {
                return new Spectrum(0f);
            }

            if (wh.X == 0 && wh.Y == 0 && wh.Z == 0)
            {
                return new Spectrum(0f);
            }

            wh = wh.Normalized();
            var spectrum = Fresnel.Evaluate(wi.Dot(wh));
            return R * Distribution.D(wh) * Distribution.G(wo, wi) * spectrum / (4 * cosThetaI * cosThetaO);
        }
     
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F u, out float pdf,  out BxDFType sampledType) 
        {
            sampledType = BxdfType;
            // Sample microfacet orientation wh and reflected direction wi 
            Vector3F wh = Distribution.Sample_wh(wo, u);
            wi = BSDF.Reflect(wo, wh);
            if (!SameHemisphere(wo, wi))
            {
                pdf = 0;
                return new Spectrum(0f);
            }

            // Compute PDF of wi for microfacet reflection 
            pdf = Distribution.Pdf(wo, wh) / (4 * wo.Dot(wh));
            return F(wo, wi);
        }

        public override float Pdf(Vector3F wo, Vector3F wi)
        {
            if (!SameHemisphere(wo, wi))
            {
                return 0;
            }

            Vector3F wh = (wo + wi).Normalized();
            return Distribution.Pdf(wo, wh) / (4 * wo.Dot(wh));
        }        
    }
}