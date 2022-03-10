using System;
using pbrt.Core;

namespace pbrt.Reflections
{
    public class BSDF
    {
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

        public static Vector3F Reflect(Vector3F wo, Vector3F n) => -wo + 2 * wo.Dot(n) * n;

        public static bool Refract(Vector3F wi, Normal3F n, float etaRatio, out Vector3F wt)
        {
            // Compute cos theta using Descartes’s law 
            float cosThetaI = n.Dot(wi);
            float sin2ThetaI = MathF.Max(0f, 1 - cosThetaI * cosThetaI);
            float sin2ThetaT = etaRatio * etaRatio * sin2ThetaI;
            
            // Handle total internal reflection for transmission
            if (sin2ThetaT >= 1)
            {
                wt = null;
                return false;
            }

            float cosThetaT = MathF.Sqrt(1 - sin2ThetaT);

            wt = etaRatio * -wi + (etaRatio * cosThetaI - cosThetaT) * (new Vector3F(n));
            return true;
        }
    }
}