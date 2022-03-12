using System;
using pbrt.Core;

namespace pbrt.Reflections
{
    public class BeckmannDistribution : MicrofacetDistribution
    {
        public float AlphaX { get; }
        public float AlphaY { get; }

        public BeckmannDistribution(float alphaX, float alphaY, bool sampleVis = true)
            : base(sampleVis)
        {
            AlphaX = alphaX;
            AlphaY = alphaY;
        }

        public override float D(Vector3F wh)
        {
            float tan2Theta = BxDF.Tan2Theta(wh);
            if (float.IsInfinity(tan2Theta))
            {
                return 0f;
            }

            float cos4Theta = BxDF.Cos2Theta(wh) * BxDF.Cos2Theta(wh);
            var f1 = BxDF.Cos2Phi(wh) / (AlphaX * AlphaX);
            var f2 = BxDF.Sin2Phi(wh) / (AlphaY * AlphaY);
            var f3 = MathF.PI * AlphaX * AlphaY * cos4Theta;
            var d = MathF.Exp(-tan2Theta * (f1 + f2)) / f3;

            return d;
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
            float a = 1 / (alpha * absTanTheta);
            if (a >= 1.6f)
            {
                return 0;
            }

            return (1 - 1.259f * a + 0.396f * a * a) / (3.535f * a + 2.181f * a * a);
        }
    }
}