using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class SpecularReflection : BxDF
    {
        public Spectrum R { get; }
        public Fresnel Fresnel { get; }

        public SpecularReflection(Spectrum r, Fresnel fresnel)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR)
        {
            R = r;
            Fresnel = fresnel;
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi) => new Spectrum(0f);
        
        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType) 
        {
            // Compute perfect specular reflection direction
            wi = new Vector3F(-wo.X, -wo.Y, wo.Z);
            pdf = 1;
            sampledType = BxdfType;
            return Fresnel.Evaluate(CosTheta(wi)) * R / AbsCosTheta(wi);
        }
        
        public override float Pdf(Vector3F wo, Vector3F wi) => 0;
    }
}