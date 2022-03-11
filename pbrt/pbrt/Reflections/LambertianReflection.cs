using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class LambertianReflection : BxDF
    {
        public Spectrum R { get; }

        public LambertianReflection(Spectrum r) 
        : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE)
        {
            R = r;
        }

        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            return R / MathF.PI;
        }

        public override Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
        {
            throw new System.NotImplementedException();
        }

        public override Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples) => R;
    }
}