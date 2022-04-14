using System;
using pbrt.Core;

namespace pbrt.Shapes
{
    public class Disk : AbstractShape
    {
        public float Height { get; }
        public float Radius { get; }
        public float InnerRadius { get; }
        public float PhiMax { get; }
        public override float Area { get; } 

        public Disk(Transform objectToWorld, Transform worldToObject, bool reverseOrientation, 
            float height, float radius, float innerRadius, float phiMax)
            : base(objectToWorld, worldToObject, reverseOrientation)
        {
            Height = height;
            Radius = radius;
            InnerRadius = innerRadius;
            PhiMax = phiMax.Clamp(0, 360).Radians();
            ObjectBound =  new  Bounds3F(new Point3F(-radius, -radius, height), new Point3F(radius, radius, height));
            Area = PhiMax * 0.5f * (Radius * Radius - InnerRadius * InnerRadius);
        }

        public override bool Intersect(Ray r, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true)
        {
            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r, out _, out _);

            // Compute plane intersection for disk
            isect = null;
            tHit = 0;
            
            // Reject disk intersections for rays parallel to the disk's plane
            if (ray.D.Z == 0)
            {
                return false;
            }

            float tShapeHit = (Height - ray.O.Z) / ray.D.Z;
            if (tShapeHit <= 0 || tShapeHit >= ray.TMax)
            {
                return false;
            }

            // See if hit point is inside disk radii and PhiMax
            Point3F pHit = ray.At(tShapeHit);
            float dist2 = pHit.X * pHit.X + pHit.Y * pHit.Y;
            if (dist2 > Radius * Radius || dist2 < InnerRadius * InnerRadius)
            {
                return false;
            }

            // Test disk phi value against PhiMax
            float phi = MathF.Atan2(pHit.Y, pHit.X);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            if (phi > PhiMax)
            {
                return false;
            }

            // Find parametric representation of disk hit
            float u = phi / PhiMax;
            float rHit = MathF.Sqrt(dist2);
            float v = (Radius - rHit) / (Radius - InnerRadius);
            Vector3F dpdu = new Vector3F(-PhiMax * pHit.Y, PhiMax * pHit.X, 0);
            Vector3F dpdv = new Vector3F(pHit.X, pHit.Y, 0f) * (InnerRadius - Radius) / rHit;
            Normal3F dndu = new Normal3F(0, 0, 0);
            Normal3F dndv = new Normal3F(0, 0, 0);

            // Refine disk intersection point
            pHit.Z = Height;

            // Compute error bounds for disk intersection
            Vector3F pError = new Vector3F(0, 0, 0);

            // Initialize _SurfaceInteraction_ from parametric information
            isect = ObjectToWorld.Apply(new SurfaceInteraction(pHit, pError, new Point2F(u, v), -ray.D, dpdu, dpdv, dndu, dndv, ray.Time, this));

            // Update tHit for quadric intersection
            tHit = tShapeHit;
            
            return true;
        }

        public override Interaction Sample(Point2F u)
        {
            Point2F pd = MathUtils.ConcentricSampleDisk(u);
            Point3F pObj = new Point3F(pd.X * Radius, pd.Y * Radius, Height);
            var normal = ObjectToWorld.Apply(new Normal3F(0, 0, 1)).Normalize();
            Interaction it = new Interaction
            {
                N = normal
            };
            
            if (ReverseOrientation)
            {
                it.N *= -1;
            }

            it.P = ObjectToWorld.Apply(pObj, new Vector3F(0, 0, 0), out var pError);
            it.PError = pError;
            return it;
        }
        

        public override bool IntersectP(Ray r, bool testAlphaTexture = true)
        {
            // Transform _Ray_ to object space
            Ray ray = WorldToObject.Apply(r, out _, out _);

            // Compute plane intersection for disk

            // Reject disk intersections for rays parallel to the disk's plane
            if (ray.D.Z == 0)
            {
                return false;
            }

            float tShapeHit = (Height - ray.O.Z) / ray.D.Z;
            if (tShapeHit <= 0 || tShapeHit >= ray.TMax)
            {
                return false;
            }

            // See if hit point is inside disk radii and $\phimax$
            Point3F pHit = ray.At(tShapeHit);
            float dist2 = pHit.X * pHit.X + pHit.Y * pHit.Y;
            if (dist2 > Radius * Radius || dist2 < InnerRadius * InnerRadius)
            {
                return false;
            }

            // Test disk $\phi$ value against $\phimax$
            float phi = MathF.Atan2(pHit.Y, pHit.X);
            if (phi < 0)
            {
                phi += 2 * MathF.PI;
            }

            if (phi > PhiMax)
            {
                return false;
            }

            return true;
        }        
    }
}