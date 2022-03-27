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

        public override Vector3F Sample_wh(Vector3F wo, Point2F u)
        {
            if (!SampleVisibleArea)
            {
                // Sample full distribution of normals for Beckmann distribution 
                // Compute tan² theta and phi for Beckmann distribution sample 
                float tan2Theta;
                float phi;
                if (Math.Abs(AlphaX - AlphaY) < float.Epsilon)
                {
                    float logSample = MathF.Log(1 - u[0]);
                    if (float.IsInfinity(logSample))
                    {
                        logSample = 0;
                    }

                    tan2Theta = -AlphaX * AlphaX * logSample;
                    phi = u[1] * 2 * MathF.PI;
                }
                else
                {
                    // Compute tan2Theta and phi for anisotropic Beckmann distribution 
                    float logSample = MathF.Log(u[0]);
                    phi = MathF.Atan(AlphaY / AlphaX * MathF.Tan(2 * MathF.PI * u[1] + 0.5f * MathF.PI));
                    if (u[1] > 0.5f)
                    {
                        phi += MathF.PI;
                    }

                    float sinPhi = MathF.Sin(phi);
                    float cosPhi = MathF.Cos(phi);
                    float alphax2 = AlphaX * AlphaX;
                    float alphay2 = AlphaY * AlphaY;
                    tan2Theta = -logSample / (cosPhi * cosPhi / alphax2 + sinPhi * sinPhi / alphay2);
                }

                // Map sampled Beckmann angles to normal direction wh
                float cosTheta = 1 / MathF.Sqrt(1 + tan2Theta);
                float sinTheta = MathF.Sqrt(MathF.Max((float)0, 1 - cosTheta * cosTheta));
                Vector3F wh = MathUtils.SphericalDirection(sinTheta, cosTheta, phi);
                if (!BxDF.SameHemisphere(wo, wh))
                {
                    wh = -wh;
                }

                return wh;
            }
            else
            {
                // Sample visible area of normals for Beckmann distribution
                Vector3F wh;
                bool flip = wo.Z < 0;
                wh = BeckmannSample(flip ? -wo : wo, AlphaX, AlphaY, u[0], u[1]);
                if (flip) wh = -wh;
                return wh;
            }
        }

        // Microfacet Utility Functions
        private static readonly float SQRT_PI_INV = 1f / MathF.Sqrt(MathF.PI);

        public static void BeckmannSample11(float cosThetaI, float u1, float u2, out float slopeX, out float slopeY)
        {
            /* Special case (normal incidence) */
            if (cosThetaI > .9999f)
            {
                float r = MathF.Sqrt(-MathF.Log(1.0f - u1));
                float sinPhi = MathF.Sin(2 * MathF.PI * u2);
                float cosPhi = MathF.Cos(2 * MathF.PI * u2);
                slopeX = r * cosPhi;
                slopeY = r * sinPhi;
                return;
            }

            /* The original inversion routine from the paper contained
               discontinuities, which causes issues for QMC integration
               and techniques like Kelemen-style MLT. The following code
               performs a numerical inversion with better behavior */
            float sinThetaI = MathF.Sqrt(MathF.Max(0f, 1f - cosThetaI * cosThetaI));
            float tanThetaI = sinThetaI / cosThetaI;
            float cotThetaI = 1 / tanThetaI;

            /* Search interval -- everything is parameterized in the Erf() domain */
            float a = -1, c = MathUtils.Erf(cotThetaI);
            float sampleX = MathF.Max(u1, 1e-6f);

            /* Start with a good initial guess */
            // Float b = (1-sample_x) * a + sample_x * c;

            /* We can do better (inverse of an approximation computed in Mathematica) */
            float thetaI = MathF.Acos(cosThetaI);
            float fit = 1 + thetaI * (-0.876f + thetaI * (0.4265f - 0.0594f * thetaI));
            float b = c - (1 + c) * MathF.Pow(1 - sampleX, fit);

            /* Normalization factor for the CDF */
            float normalization = 1 / (1 + c + SQRT_PI_INV * tanThetaI * MathF.Exp(-cotThetaI * cotThetaI));

            int it = 0;
            while (++it < 10)
            {
                /* Bisection criterion -- the oddly-looking
                   Boolean expression are intentional to check
                   for NaNs at little additional cost */
                if (!(b >= a && b <= c))
                {
                    b = 0.5f * (a + c);
                }

                /* Evaluate the CDF and its derivative
                   (i.e. the density function) */
                float invErf = MathUtils.ErfInv(b);
                float value = normalization * (1 + b + SQRT_PI_INV * tanThetaI * MathF.Exp(-invErf * invErf)) - sampleX;
                float derivative = normalization * (1 - invErf * tanThetaI);

                if (MathF.Abs(value) < 1e-5f)
                {
                    break;
                }

                /* Update bisection intervals */
                if (value > 0)
                {
                    c = b;
                }
                else
                {
                    a = b;
                }

                b -= value / derivative;
            }

            /* Now convert back into a slope value */
            slopeX = MathUtils.ErfInv(b);

            /* Simulate Y component */
            slopeY = MathUtils.ErfInv(2.0f * MathF.Max(u2, 1e-6f) - 1.0f);
        }

        public static Vector3F BeckmannSample(Vector3F wi, float alphaX, float alphaY, float u1, float u2)
        {
            // 1. stretch wi
            Vector3F wiStretched = (new Vector3F(alphaX * wi.X, alphaY * wi.Y, wi.Z)).Normalized();

            // 2. simulate P22_{wi}(x_slope, y_slope, 1, 1)
            BeckmannSample11(BxDF.CosTheta(wiStretched), u1, u2, out var slopeX, out var slopeY);

            // 3. rotate
            float tmp = BxDF.CosPhi(wiStretched) * slopeX - BxDF.SinPhi(wiStretched) * slopeY;
            slopeY = BxDF.SinPhi(wiStretched) * slopeX + BxDF.CosPhi(wiStretched) * slopeY;
            slopeX = tmp;

            // 4. unstretch
            slopeX = alphaX * slopeX;
            slopeY = alphaY * slopeY;

            // 5. compute normal
            return (new Vector3F(-slopeX, -slopeY, 1f)).Normalized();
        }
    }
}