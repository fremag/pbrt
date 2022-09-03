using System;
using pbrt.Core;

namespace pbrt.Media
{
    public class HenyeyGreensteinPhaseFunction : PhaseFunction
    {
        public float G { get; set; }

        public HenyeyGreensteinPhaseFunction(float g)
        {
            G = g;
        }

        public float PhaseHG(float cosTheta, float g) 
        {
            float denom = 1 + g * g + 2 * g * cosTheta;
            return MathUtils.Inv4PI * (1 - g * g) / (denom * MathF.Sqrt(denom));
        }
        
        public override float P(Vector3F wo, Vector3F wi) => PhaseHG(wo.Dot(wi), G);

        public override float Sample_P(Vector3F wo, out Vector3F wi, Point2F u)
        {
            float cosTheta;
            if (MathF.Abs(G) < 1e-3)
            {
                cosTheta = 1 - 2 * u[0];
            }
            else {
                float sqrTerm = (1 - G * G) / (1 + G - 2 * G * u[0]);
                cosTheta = -(1 + G * G - sqrTerm * sqrTerm) / (2 * G);
            }

            // Compute direction wi for Henyey--Greenstein sample
            float sinTheta = MathF.Sqrt(MathF.Max(0f, 1 - cosTheta * cosTheta));
            float phi = 2 * MathF.PI * u[1];

            Vector3F.CoordinateSystem(wo, out var v1, out var v2);
            wi = MathUtils.SphericalDirection(sinTheta, cosTheta, phi, v1, v2, wo);
            return PhaseHG(cosTheta, G);   
        }
    }
}