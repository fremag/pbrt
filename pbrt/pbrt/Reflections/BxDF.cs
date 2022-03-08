using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    [Flags]
    public enum BxDFType
    {
        BSDF_REFLECTION = 1 << 0,
        BSDF_TRANSMISSION = 1 << 1,
        BSDF_DIFFUSE = 1 << 2,
        BSDF_GLOSSY = 1 << 3,
        BSDF_SPECULAR = 1 << 4,
        BSDF_ALL = BSDF_DIFFUSE | BSDF_GLOSSY | BSDF_SPECULAR | BSDF_REFLECTION | BSDF_TRANSMISSION
    };

    public abstract class BxDF
    {
        public BxDFType BxdfType { get; }

        protected BxDF(BxDFType bxdfType)
        {
            BxdfType = bxdfType;
        }
        
        public bool MatchesFlags(BxDFType bxDfType) => (BxdfType & bxDfType) == bxDfType;
        public abstract Spectrum F(Vector3F wo, Vector3F wi);
        public abstract Spectrum Sample_f(Vector3F wo, Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType);
        public abstract  Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples);
    }
}