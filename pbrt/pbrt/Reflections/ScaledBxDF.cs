using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class ScaledBxDF : BxDF
    {
        public BxDF Bxdf { get; }
        public Spectrum Scale { get; }

        public ScaledBxDF(BxDF bxdf, Spectrum scale) : base(bxdf.BxdfType)
        {
            Bxdf = bxdf;
            Scale = scale;
        }
        
        public override Spectrum F(Vector3F wo, Vector3F wi) => Scale * Bxdf.F(wo, wi);
        public override Spectrum Sample_f(Vector3F wo, Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType) => Scale * Bxdf.Sample_f(wo, wi, sample, out pdf, out sampledType);
        public override Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples) => Scale * Bxdf.Rho(wo, nSamples, samples);
    }
}