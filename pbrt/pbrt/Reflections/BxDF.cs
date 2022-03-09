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
        public abstract Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples);

        public static float FrDielectric(float cosThetaI, float etaI, float etaT)
        {
            cosThetaI = cosThetaI.Clamp(-1, 1);
            // Potentially swap indices of refraction
            bool entering = cosThetaI > 0f;
            if (!entering)
            {
                (etaT, etaI) = (etaI, etaT);
                cosThetaI = MathF.Abs(cosThetaI);
            }

            // Compute cosThetaT using Descartes’ law 
            float sinThetaI = MathF.Sqrt(MathF.Max(0f, 1 - cosThetaI * cosThetaI));
            float sinThetaT = etaI / etaT * sinThetaI;

            if (sinThetaT >= 1)
                return 1;

            float cosThetaT = MathF.Sqrt(MathF.Max(0f, 1 - sinThetaT * sinThetaT));

            float Rparl = ((etaT * cosThetaI) - (etaI * cosThetaT)) /
                          ((etaT * cosThetaI) + (etaI * cosThetaT));
            float Rperp = ((etaI * cosThetaI) - (etaT * cosThetaT)) /
                          ((etaI * cosThetaI) + (etaT * cosThetaT));
            return (Rparl * Rparl + Rperp * Rperp) / 2;
        }
        
        // k = absorption coefficient
        // https://www.pbr-book.org/3ed-2018/Reflection_Models/Specular_Reflection_and_Transmission#eq:frcond
        public static Spectrum FrConductor(float cosThetaI, Spectrum etaI, Spectrum etaT, Spectrum k) 
        {
            cosThetaI = cosThetaI.Clamp(-1, 1);
            Spectrum eta = etaT / etaI;
            Spectrum etaK = k / etaI;

            float cosThetaI2 = cosThetaI * cosThetaI;
            float sinThetaI2 = 1 - cosThetaI2;
            Spectrum eta2 = eta * eta;
            Spectrum etaK2 = etaK * etaK;

            var spectrum = (eta2 - etaK2);
            Spectrum t0 = spectrum - sinThetaI2;
            Spectrum a2PlusB2 = (t0 * t0 + 4 * eta2 * etaK2);
            a2PlusB2.Sqrt();
            
            Spectrum t1 = a2PlusB2 + cosThetaI2;
            Spectrum a = (0.5f * (a2PlusB2 + t0));
            a.Sqrt();
            Spectrum t2 = 2f * cosThetaI * a;
            Spectrum Rs = (t1 - t2) / (t1 + t2);

            Spectrum t3 = cosThetaI2 * a2PlusB2 + (sinThetaI2 * sinThetaI2);
            Spectrum t4 = t2 * sinThetaI2;
            Spectrum Rp = Rs * (t3 - t4) / (t3 + t4);

            return 0.5f * (Rp + Rs);
        }        
    }
}