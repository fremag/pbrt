using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class FresnelBlend : BxDF
    {
        public Spectrum Rd { get; }
        public Spectrum Rs { get; }
        public MicrofacetDistribution Distribution { get; }

        public FresnelBlend(Spectrum rd, Spectrum rs, MicrofacetDistribution distribution)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_GLOSSY)
        {
            Rd = rd;
            Rs = rs;
            Distribution = distribution;
        }
        
        public static float Pow5(float v)
        {
            var vv = (v * v);
            return vv * vv * v;
        }

        public Spectrum SchlickFresnel(float cosTheta) 
        {
            return Rs + Pow5(1 - cosTheta) * (new Spectrum(1f) - Rs);
        }

        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            var diffuse = (28f/(23f*MathF.PI)) * Rd * (new Spectrum(1f) - Rs) * (1 - Pow5(1 - .5f * AbsCosTheta(wi))) * (1 - Pow5(1 - .5f * AbsCosTheta(wo)));
            var wh = wi + wo;
            if (wh.X == 0 && wh.Y == 0 && wh.Z == 0)
            {
                return new Spectrum(0);
            }

            wh = wh.Normalized();
            var specular = Distribution.D(wh) / (4 * wi.AbsDot(wh) * MathF.Max(AbsCosTheta(wi), AbsCosTheta(wo))) * SchlickFresnel(wi.Dot(wh));
            return diffuse + specular;
        }
    }
}