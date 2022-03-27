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

        public static float RoughnessToAlpha(float roughness)
        {
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

        public static void TrowbridgeReitzSample11(float cosTheta, float U1, float U2, out float slope_x, out float slope_y)
        {
            // special case (normal incidence)
            if (cosTheta > .9999)
            {
                float r = MathF.Sqrt(U1 / (1 - U1));
                float phi = 6.28318530718f * U2;
                slope_x = r * MathF.Cos(phi);
                slope_y = r * MathF.Sin(phi);
                return;
            }

            float sinTheta = MathF.Sqrt(MathF.Max(0f, 1f - cosTheta * cosTheta));
            float tanTheta = sinTheta / cosTheta;
            float a = 1 / tanTheta;
            float G1 = 2 / (1 + MathF.Sqrt(1f + 1f / (a * a)));

            // sample slope_x
            float A = 2 * U1 / G1 - 1;
            float tmp = 1f / (A * A - 1f);
            if (tmp > 1e10)
            {
                tmp = 1e10f;
            }

            float B = tanTheta;
            float D = MathF.Sqrt(MathF.Max(B * B * tmp * tmp - (A * A - B * B) * tmp, 0f));
            float slope_x_1 = B * tmp - D;
            float slope_x_2 = B * tmp + D;
            slope_x = (A < 0 || slope_x_2 > 1f / tanTheta) ? slope_x_1 : slope_x_2;

            // sample slope_y
            float S;
            if (U2 > 0.5f)
            {
                S = 1f;
                U2 = 2f * (U2 - .5f);
            }
            else
            {
                S = -1f;
                U2 = 2f * (.5f - U2);
            }

            float z = (U2 * (U2 * (U2 * 0.27385f - 0.73369f) + 0.46341f)) / (U2 * (U2 * (U2 * 0.093073f + 0.309420f) - 1.000000f) + 0.597999f);
            slope_y = S * z * MathF.Sqrt(1f + slope_x * slope_x);
        }

        public static Vector3F TrowbridgeReitzSample(Vector3F wi, float alpha_x, float alpha_y, float U1, float U2)
        {
            // 1. stretch wi
            Vector3F wiStretched = (new Vector3F(alpha_x * wi.X, alpha_y * wi.Y, wi.Z)).Normalized();

            // 2. simulate P22_{wi}(x_slope, y_slope, 1, 1)
            float slope_x;
            float slope_y;
            TrowbridgeReitzSample11(BxDF.CosTheta(wiStretched), U1, U2, out slope_x, out slope_y);

            // 3. rotate
            float tmp = BxDF.CosPhi(wiStretched) * slope_x - BxDF.SinPhi(wiStretched) * slope_y;
            slope_y = BxDF.SinPhi(wiStretched) * slope_x + BxDF.CosPhi(wiStretched) * slope_y;
            slope_x = tmp;

            // 4. unstretch
            slope_x = alpha_x * slope_x;
            slope_y = alpha_y * slope_y;

            // 5. compute normal
            return (new Vector3F(-slope_x, -slope_y, 1f)).Normalized();
        }

        public override Vector3F Sample_wh(Vector3F wo, Point2F u)
        {
            Vector3F wh;
            if (!SampleVisibleArea)
            {
                float cosTheta = 0, phi = (2 * MathF.PI) * u[1];
                if (Math.Abs(AlphaX - AlphaY) < float.Epsilon)
                {
                    float tanTheta2 = AlphaX * AlphaX * u[0] / (1.0f - u[0]);
                    cosTheta = 1 / MathF.Sqrt(1 + tanTheta2);
                }
                else
                {
                    phi = MathF.Atan(AlphaY / AlphaX * MathF.Tan(2 * MathF.PI * u[1] + .5f * MathF.PI));
                    if (u[1] > .5f)
                    {
                        phi += MathF.PI;
                    }

                    float sinPhi = MathF.Sin(phi), cosPhi = MathF.Cos(phi);
                    float alphax2 = AlphaX * AlphaX;
                    float alphay2 = AlphaY * AlphaY;
                    float alpha2 = 1 / (cosPhi * cosPhi / alphax2 + sinPhi * sinPhi / alphay2);
                    float tanTheta2 = alpha2 * u[0] / (1 - u[0]);
                    cosTheta = 1 / MathF.Sqrt(1 + tanTheta2);
                }

                float sinTheta = MathF.Sqrt(MathF.Max(0f, 1f - cosTheta * cosTheta));
                wh = MathUtils.SphericalDirection(sinTheta, cosTheta, phi);
                if (!BxDF.SameHemisphere(wo, wh))
                {
                    wh = -wh;
                }
            }
            else
            {
                bool flip = wo.Z < 0;
                wh = TrowbridgeReitzSample(flip ? -wo : wo, AlphaX, AlphaY, u[0], u[1]);
                if (flip)
                {
                    wh = -wh;
                }
            }

            return wh;
        }
    }
}