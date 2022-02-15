using System;
using pbrt.Core;

namespace pbrt.Shapes
{
    public class Sphere : AbstractShape
    {
        public float Radius { get; set; }
        public float ZMin { get; set; }
        public float ZMax { get; set; }
        public float ThetaMin { get; set; }
        public float ThetaMax { get; set; }
        public float PhiMax { get; set; }

        public Sphere(Transform objectToWorld, Transform worldToObject, bool reverseOrientation, float radius, float zMin, float zMax, float phiMax)
            : base(objectToWorld, worldToObject, reverseOrientation)
        {
            Radius = radius;
            ZMin = zMin.Clamp(-radius, radius);
            ZMax = zMax.Clamp(-radius, radius);
            PhiMax = phiMax.Radians().Clamp(0, 360);
            ThetaMin = MathF.Acos(zMin / radius).Clamp(-1, 1);
            ThetaMax = MathF.Acos(zMax / radius).Clamp(-1, 1);
            var pMin = new Point3F(-radius, -radius, zMin);
            var pMax = new Point3F(radius, radius, zMax);

            ObjectBound = new Bounds3F(pMin, pMax);
        }

        public override float Area => PhiMax * Radius * (ZMax - ZMin);

        public override bool Intersect(Ray r, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true)
        {
            tHit = 0;
            isect = null;
            float phi;
            Point3F pHit;
            // Transform _Ray_ to object space
            Vector3F oErr, dErr;
            Ray ray = WorldToObject.Apply(r, out oErr, out dErr);

            // Compute quadratic sphere coefficients

            // Initialize _EFloat_ ray coordinate values
            var ox = new EFloat(ray.O.X, oErr.X);
            var oy = new EFloat(ray.O.Y, oErr.Y);
            var oz = new EFloat(ray.O.Z, oErr.Z);

            EFloat dx = new EFloat(ray.D.X, dErr.X);
            var dy = new EFloat(ray.D.Y, dErr.Y);
            var dz = new EFloat(ray.D.Z, dErr.Z);

            EFloat a = dx * dx + dy * dy + dz * dz;
            EFloat b = 2 * (dx * ox + dy * oy + dz * oz);
            EFloat c = ox * ox + oy * oy + oz * oz - new EFloat(Radius) * new EFloat(Radius);

            // Solve quadratic equation for _t_ values
            EFloat t0, t1;
            if (!EFloat.Quadratic(a, b, c, out t0, out t1))
            {
                return false;
            }

            // Check quadric shape _t0_ and _t1_ for nearest intersection
            if (t0.UpperBound() > ray.TMax || t1.LowerBound() <= 0)
            {
                return false;
            }

            EFloat tShapeHit = t0;
            if (tShapeHit.LowerBound() <= 0)
            {
                tShapeHit = t1;
                if (tShapeHit.UpperBound() > ray.TMax)
                {
                    return false;
                }
            }

            // Compute sphere hit position and $\phi$
            pHit = ray.At((float)tShapeHit);

            // Refine sphere intersection point
            pHit *= Radius / Point3F.Distance(pHit, Point3F.Zero);
            if (pHit.X == 0 && pHit.Y == 0)
            {
                pHit.X = 1e-5f * Radius;
            }

            phi = MathF.Atan2(pHit.Y, pHit.X);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            // Test sphere intersection against clipping parameters
            if ((ZMin > -Radius && pHit.Z < ZMin) || (ZMax < Radius && pHit.Z > ZMax) || phi > PhiMax)
            {
                if (tShapeHit == t1)
                {
                    return false;
                }

                if (t1.UpperBound() > ray.TMax)
                {
                    return false;
                }

                tShapeHit = t1;
                // Compute sphere hit position and $\phi$
                pHit = ray.At((float)tShapeHit);

                // Refine sphere intersection point
                pHit *= Radius / Point3F.Distance(pHit, Point3F.Zero);
                if (pHit.X == 0 && pHit.Y == 0)
                {
                    pHit.X = 1e-5f * Radius;
                }

                phi = MathF.Atan2(pHit.Y, pHit.X);
                if (phi < 0)
                {
                    phi += 2 * MathF.PI;
                }

                if ((ZMin > -Radius && pHit.Z < ZMin) || (ZMax < Radius && pHit.Z > ZMax) || phi > PhiMax)
                {
                    return false;
                }
            }

            // Find parametric representation of sphere hit
            float u = phi / PhiMax;
            float theta = MathF.Acos((pHit.Z / Radius).Clamp(-1, 1));
            float v = (theta - ThetaMin) / (ThetaMax - ThetaMin);

            // Compute sphere $\dpdu$ and $\dpdv$
            float zRadius = MathF.Sqrt(pHit.X * pHit.X + pHit.Y * pHit.Y);
            float invZRadius = 1 / zRadius;
            float cosPhi = pHit.X * invZRadius;
            float sinPhi = pHit.Y * invZRadius;
            Vector3F dpdu = new Vector3F(-PhiMax * pHit.Y, PhiMax * pHit.X, 0);
            var vector3F = new Vector3F(pHit.Z * cosPhi, pHit.Z * sinPhi, -Radius * MathF.Sin(theta));
            Vector3F dpdv = (ThetaMax - ThetaMin) * vector3F;

            // Compute sphere $\dndu$ and $\dndv$
            Vector3F d2Pduu = (-PhiMax * PhiMax) * new Vector3F(pHit.X, pHit.Y, 0);
            Vector3F d2Pduv = (ThetaMax - ThetaMin) * pHit.Z * PhiMax * new Vector3F(-sinPhi, cosPhi, 0);
            Vector3F d2Pdvv = -(ThetaMax - ThetaMin) * (ThetaMax - ThetaMin) * new Vector3F(pHit.X, pHit.Y, pHit.Z);

            // Compute coefficients for fundamental forms
            float E = dpdu.Dot(dpdu);
            float F = dpdu.Dot(dpdv);
            float G = dpdv.Dot(dpdv);
            Vector3F N = dpdu.Cross(dpdv).Normalized();
            float e = N.Dot(d2Pduu);
            float f = N.Dot(d2Pduv);
            float g = N.Dot(d2Pdvv);

            // Compute $\dndu$ and $\dndv$ from fundamental form coefficients
            float invEGF2 = 1 / (E * G - F * F);
            Normal3F dndu = new Normal3F((f * F - e * G) * invEGF2 * dpdu + (e * F - f * E) * invEGF2 * dpdv);
            Normal3F dndv = new Normal3F((g * F - f * G) * invEGF2 * dpdu + (f * F - g * E) * invEGF2 * dpdv);

            // Compute error bounds for sphere intersection
            Vector3F pError = MathUtils.Gamma(5) * (new Vector3F(pHit)).Abs();

            // Initialize _SurfaceInteraction_ from parametric information
            var surfaceInteraction = new SurfaceInteraction(pHit, pError, new Point2F(u, v),
                -ray.D, dpdu, dpdv, dndu, dndv,
                ray.Time, this);

            isect = ObjectToWorld.Apply(surfaceInteraction);

            // Update _tHit_ for quadric intersection
            tHit = (float)tShapeHit;
            return true;
        }
    }
}