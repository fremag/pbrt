using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public class OrenNayar : BxDF
    {
        public Spectrum R { get; }
        public float A { get; }
        public float B { get; }

        public OrenNayar(Spectrum r, float sigma)
            : base(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE)
        {
            R = r;
            sigma = sigma.Radians();
            var sigma2 = sigma * sigma;
            A = 1f - (sigma2 / (2f * (sigma2 + 0.33f)));
            B = 0.45f * sigma2 / (sigma2 + 0.09f);
        }

        public override Spectrum F(Vector3F wo, Vector3F wi)
        {
            float sinThetaI = SinTheta(wi);
            float sinThetaO = SinTheta(wo);
            // Compute cosine term of Oren–Nayar model
            float maxCos = 0;
            if (sinThetaI > 1e-4 && sinThetaO > 1e-4)
            {
                float sinPhiI = SinPhi(wi);
                float cosPhiI = CosPhi(wi);
                float sinPhiO = SinPhi(wo);
                float cosPhiO = CosPhi(wo);
                float dCos = cosPhiI * cosPhiO + sinPhiI * sinPhiO;
                maxCos = MathF.Max(0f, dCos);
            }

            // Compute sine and tangent terms of Oren–Nayar model
            float sinAlpha, tanBeta;
            if (AbsCosTheta(wi) > AbsCosTheta(wo))
            {
                sinAlpha = sinThetaO;
                tanBeta = sinThetaI / AbsCosTheta(wi);
            }
            else
            {
                sinAlpha = sinThetaI;
                tanBeta = sinThetaO / AbsCosTheta(wo);
            }

            var coeff = (A + B * maxCos * sinAlpha * tanBeta) / MathF.PI;
            return R * coeff;
        }
    }
}