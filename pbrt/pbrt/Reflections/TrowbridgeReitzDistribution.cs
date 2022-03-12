using System;
using pbrt.Core;

namespace pbrt.Reflections
{
    public class TrowbridgeReitzDistribution : MicrofacetDistribution
    {
        public float AlphaX { get; }
        public float AlphaY { get; }

        public TrowbridgeReitzDistribution(float alphaX, float alphaY, bool samplevis = true)
            : base(samplevis)
        {
            AlphaX = alphaX;
            AlphaY = alphaY;
        }
        
        public static float RoughnessToAlpha(float roughness) {
            roughness = MathF.Max(roughness, 1e-3f);
            float x = MathF.Log(roughness);
            return 1.62142f + 0.819955f * x + 0.1734f * x * x + 0.0171201f * x * x * x + 0.000640711f * x * x * x * x;
        }

        public override float D(Vector3F wh) 
        {
            float tan2Theta = BxDF.Tan2Theta(wh);
            if (float.IsInfinity(tan2Theta))
            {
                return 0f;
            }

            float cos4Theta = BxDF.Cos2Theta(wh) * BxDF.Cos2Theta(wh);
            float e = (BxDF.Cos2Phi(wh) / (AlphaX * AlphaX) + BxDF.Sin2Phi(wh) / (AlphaY * AlphaY)) * tan2Theta;
            return 1 / (MathF.PI * AlphaX * AlphaY * cos4Theta * (1 + e) * (1 + e));
        }       
        
        public override float Lambda(Vector3F w) 
        {
            float absTanTheta = MathF.Abs(BxDF.TanTheta(w));
            if (float.IsInfinity(absTanTheta))
            {
                return 0f;
            }

            // Compute alpha for direction w 
            float alpha = MathF.Sqrt(BxDF.Cos2Phi(w) * AlphaX * AlphaX + BxDF.Sin2Phi(w) * AlphaY * AlphaY);
            
            float alpha2Tan2Theta = (alpha * absTanTheta) * (alpha * absTanTheta);
            return (-1 + MathF.Sqrt(1f + alpha2Tan2Theta)) / 2;
        }
    }
}