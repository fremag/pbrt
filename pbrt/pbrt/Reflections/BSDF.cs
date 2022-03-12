using System;
using pbrt.Core;

namespace pbrt.Reflections
{
    public class BSDF
    {
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