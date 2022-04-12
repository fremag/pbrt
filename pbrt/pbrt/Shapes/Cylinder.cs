using System;
using pbrt.Core;

namespace pbrt.Shapes
{
    public class Cylinder : AbstractShape
    {
        public float Radius { get; }
        public float ZMin { get; }
        public float ZMax { get; }
        public float PhiMax { get; }

        public Cylinder(Transform objectToWorld, float radius = 0.5f, float zMin = 0, float zMax = 1, float phiMax = 360)
            : this(objectToWorld, objectToWorld.Inverse(), false, radius, zMin, zMax, phiMax)
        {
        }

        public Cylinder(Transform objectToWorld, Transform worldToObject, bool reverseOrientation, float radius = 0.5f, float zMin = 0, float zMax = 1, float phiMax = 360)
            : base(objectToWorld, worldToObject, reverseOrientation)
        {
            Radius = radius;
            ZMin = zMin;
            ZMax = zMax;
            PhiMax = phiMax.Clamp(0, 360).Radians();
            ObjectBound = new Bounds3F(new Point3F(-Radius, -Radius, ZMin), new Point3F(Radius, Radius, ZMax));
        }

        public override float Area => (ZMax - ZMin) * Radius * PhiMax;

        public override bool Intersect(Ray r, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture=true)
        {
            float phi;
            Point3F pHit;
            // Transform _Ray_ to object space
            Vector3F oErr, dErr;
            Ray ray = WorldToObject.Apply(r, out oErr, out dErr);

            // Compute quadratic cylinder coefficients

            // Initialize _EFloat_ ray coordinate values
            var ox = new EFloat(ray.O.X, oErr.X);
            var oy = new EFloat(ray.O.Y, oErr.Y);

            var dx = new EFloat(ray.D.X, dErr.X);
            var dy = new EFloat(ray.D.Y, dErr.Y);

            EFloat a = dx * dx + dy * dy;
            EFloat b = 2 * (dx * ox + dy * oy);
            EFloat c = ox * ox + oy * oy - new EFloat(Radius) * new EFloat(Radius);

            tHit = 0;
            isect = null;
            // Solve quadratic equation for _t_ values
            if (!MathUtils.Quadratic((float)a, (float)b, (float)c, out var ft0, out var ft1))
            {
                return false;
            }

            var t0 = new EFloat(ft0);
            var t1 = new EFloat(ft1);

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

            // Compute cylinder hit point and $\phi$
            pHit = ray.At((float)tShapeHit);

            // Refine cylinder intersection point
            float hitRad = MathF.Sqrt(pHit.X * pHit.X + pHit.Y * pHit.Y);
            pHit.X *= Radius / hitRad;
            pHit.Y *= Radius / hitRad;
            phi = MathF.Atan2(pHit.Y, pHit.X);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            // Test cylinder intersection against clipping parameters
            if (pHit.Z < ZMin || pHit.Z > ZMax || phi > PhiMax)
            {
                if (tShapeHit == t1)
                {
                    return false;
                }

                tShapeHit = t1;
                if (t1.UpperBound() > ray.TMax)
                {
                    return false;
                }

                // Compute cylinder hit point and $\phi$
                pHit = ray.At((float)tShapeHit);

                // Refine cylinder intersection point
                float hitRadRefined = MathF.Sqrt(pHit.X * pHit.X + pHit.Y * pHit.Y);
                pHit.X *= Radius / hitRadRefined;
                pHit.Y *= Radius / hitRadRefined;
                phi = MathF.Atan2(pHit.Y, pHit.X);
                if (phi < 0)
                {
                    phi += 2 * MathF.PI;
                }

                if (pHit.Z < ZMin || pHit.Z > ZMax || phi > PhiMax)
                {
                    return false;
                }
            }

            // Find parametric representation of cylinder hit
            float u = phi / PhiMax;
            float v = (pHit.Z - ZMin) / (ZMax - ZMin);

            // Compute cylinder $\dpdu$ and $\dpdv$
            Vector3F dpdu = new Vector3F(-PhiMax * pHit.Y, PhiMax * pHit.X, 0);
            Vector3F dpdv = new Vector3F(0, 0, ZMax - ZMin);

            // Compute cylinder $\dndu$ and $\dndv$
            var phiMax2 = (-PhiMax * PhiMax);
            var d2Pduu = new Vector3F(phiMax2 * pHit.X, phiMax2 * pHit.Y, 0);
            var d2Pduv = new Vector3F(0, 0, 0);
            var d2Pdvv = new Vector3F(0, 0, 0);

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
            Normal3F dndu = new Normal3F((f * F - e * G) * invEGF2 * dpdu +
                                         (e * F - f * E) * invEGF2 * dpdv);
            Normal3F dndv = new Normal3F((g * F - f * G) * invEGF2 * dpdu +
                                         (f * F - g * E) * invEGF2 * dpdv);

            // Compute error bounds for cylinder intersection
            Vector3F pError = MathUtils.Gamma(3) * new Vector3F(pHit.X, pHit.Y, 0).Abs();

            // Initialize _SurfaceInteraction_ from parametric information
            var surfaceInteraction = new SurfaceInteraction(pHit, pError, new Point2F(u, v),
                -ray.D, dpdu, dpdv, dndu, dndv,
                ray.Time, this);
            isect = ObjectToWorld.Apply(surfaceInteraction);

            // Update _tHit_ for quadric intersection
            tHit = (float)tShapeHit;
            return true;
        }

        public bool IntersectP(Ray r)
        {
            float phi;
            Point3F pHit;
            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r, out var oErr, out var dErr);

            // Compute quadratic cylinder coefficients

            // Initialize _EFloat_ ray coordinate values
            var ox = new EFloat(ray.O.X, oErr.X);
            var oy = new EFloat(ray.O.Y, oErr.Y);

            var dx = new EFloat(ray.D.X, dErr.X);
            var dy = new EFloat(ray.D.Y, dErr.Y);

            EFloat a = dx * dx + dy * dy;
            EFloat b = 2 * (dx * ox + dy * oy);
            EFloat c = ox * ox + oy * oy - new EFloat(Radius) * new EFloat(Radius);

            // Solve quadratic equation for _t_ values
            if (!MathUtils.Quadratic((float)a, (float)b, (float)c, out var ft0, out var ft1))
            {
                return false;
            }

            var t0 = (EFloat)ft0;
            var t1 = (EFloat)ft1;

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

            // Compute cylinder hit point and $\phi$
            pHit = ray.At((float)tShapeHit);

            // Refine cylinder intersection point
            float hitRad = MathF.Sqrt(pHit.X * pHit.X + pHit.Y * pHit.Y);
            pHit.X *= Radius / hitRad;
            pHit.Y *= Radius / hitRad;
            phi = MathF.Atan2(pHit.Y, pHit.X);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            // Test cylinder intersection against clipping parameters
            if (pHit.Z < ZMin || pHit.Z > ZMax || phi > PhiMax)
            {
                if (tShapeHit == t1) return false;
                tShapeHit = t1;
                if (t1.UpperBound() > ray.TMax) return false;
                // Compute cylinder hit point and $\phi$
                pHit = ray.At((float)tShapeHit);

                // Refine cylinder intersection point
                hitRad = MathF.Sqrt(pHit.X * pHit.X + pHit.Y * pHit.Y);
                pHit.X *= Radius / hitRad;
                pHit.Y *= Radius / hitRad;
                phi = MathF.Atan2(pHit.Y, pHit.X);
                if (phi < 0)
                {
                    phi += 2 * MathF.PI;
                }

                if (pHit.Z < ZMin || pHit.Z > ZMax || phi > PhiMax)
                {
                    return false;
                }
            }

            return true;
        }

        public override Interaction Sample(Point2F u)
        {
            float z = MathUtils.Lerp(u[0], ZMin, ZMax);
            float phi = u[1] * PhiMax;
            Point3F pObj = new Point3F(Radius * MathF.Cos(phi), Radius * MathF.Sin(phi), z);
            Interaction it = new Interaction();
            it.N = (ObjectToWorld.Apply(new Normal3F(pObj.X, pObj.Y, 0))).Normalize();
            if (ReverseOrientation)
            {
                it.N *= -1;
            }

            // Reproject _pObj_ to cylinder surface and compute _pObjError_
            float hitRad = MathF.Sqrt(pObj.X * pObj.X + pObj.Y * pObj.Y);
            pObj.X *= Radius / hitRad;
            pObj.Y *= Radius / hitRad;
            Vector3F pObjError = MathUtils.Gamma(3) * new Vector3F(pObj.X, pObj.Y, 0).Abs();
            it.P = ObjectToWorld.Apply(pObj, pObjError, out var pError);
            it.PError = pError;
            return it;
        }
    }
}