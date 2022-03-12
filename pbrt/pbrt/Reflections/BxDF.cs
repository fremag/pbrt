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

        public abstract Spectrum F(Vector3F wo, Vector3F wi);

        public virtual Spectrum Sample_f(Vector3F wo, out Vector3F wi, Point2F sample, out float pdf, out BxDFType sampledType)
        {
            // The sample and pdf parameters aren’t needed for delta distribution BxDFs, so they will be explained later,
            // in Section 14.1, when we provide implementations of this method for nonspecular reflection functions.             
            throw new NotImplementedException();
        }
        
        // The BxDF::rho() method computes the reflectance function . Some BxDFs can compute this value
        // in closed form, although most use Monte Carlo integration to compute an approximation to it.
        // For those BxDFs, the nSamples and samples parameters are used by the implementation of the Monte Carlo algorithm;
        // they are explained in Section 14.1.5. 
        public virtual Spectrum Rho(Vector3F wo, int nSamples, Point2F[] samples) => throw new NotImplementedException();
        public virtual Spectrum Rho(int nSamples, out Point2F samples1, out Point2F samples2)  => throw new NotImplementedException();
        
        public bool MatchesFlags(BxDFType bxDfType) => (BxdfType & bxDfType) == bxDfType;

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

        public static float CosTheta(Vector3F w) => w.Z;
        public static float Cos2Theta(Vector3F w) => w.Z * w.Z;
        public static float AbsCosTheta(Vector3F w) => MathF.Abs(w.Z);
        public static float Sin2Theta(Vector3F w) => MathF.Max(0f, 1f - Cos2Theta(w));
        public static float SinTheta(Vector3F w) => MathF.Sqrt(Sin2Theta(w));
        public static float TanTheta(Vector3F w) => SinTheta(w) / CosTheta(w);
        public static float Tan2Theta(Vector3F w) => Sin2Theta(w) / Cos2Theta(w);

        public static float CosPhi(Vector3F w)
        {
            float sinTheta = SinTheta(w);
            return (sinTheta == 0) ? 1 : (w.X / sinTheta).Clamp(-1, 1);
        }

        public static float SinPhi(Vector3F w)
        {
            float sinTheta = SinTheta(w);
            return (sinTheta == 0) ? 0 : (w.Y / sinTheta).Clamp(-1, 1);
        }

        public static float Cos2Phi(Vector3F w) => CosPhi(w) * CosPhi(w);
        public static float Sin2Phi(Vector3F w) => SinPhi(w) * SinPhi(w);

        public static float CosDPhi(Vector3F wa, Vector3F wb)
        {
            var waXy = (wa.X * wa.X + wa.Y * wa.Y);
            var wbXy = (wb.X * wb.X + wb.Y * wb.Y);
            return (waXy / MathF.Sqrt(waXy * wbXy)).Clamp(-1, 1);
        }
        
        public static bool SameHemisphere(Vector3F w, Vector3F wp) => w.Z * wp.Z > 0;
        public static bool SameHemisphere(Vector3F w, Normal3F wp) => w.Z * wp.Z > 0;
    }
}