using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class LambertianTransmission : BxDF
    {
        public Spectrum T { get; }

        public LambertianTransmission(Spectrum t) 
        : base(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_DIFFUSE)
        {
            T = t;
        }

        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            return T / MathF.PI;
        }

        public override Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples) => T;
    }
}