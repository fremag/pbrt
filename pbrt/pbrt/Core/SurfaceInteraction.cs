using System;
using pbrt.Reflections;
using pbrt.Shapes;

namespace pbrt.Core
{
    public class Shading
    {
        public Normal3F N { get; set; }
        public Vector3F DpDu { get; set; }
        public Vector3F DpDv { get; set; }
        public Normal3F DnDu { get; set; }
        public Normal3F DnDv { get; set; }
    }

    public class SurfaceInteraction : Interaction
    {
        public Point2F Uv { get; set; }
        public Vector3F DpDu { get; set; }
        public Vector3F DpDv { get; set; }
        public Normal3F DnDu { get; set; }
        public Normal3F DnDv { get; set; }
        public AbstractShape Shape { get; set; }
        public Shading Shading { get; set; }

        public IPrimitive Primitive = null;
        public BSDF Bsdf = null;
        public BSSRDF Bssrdf = null;
        public Vector3F DpDx { get; set; }
        public Vector3F DpDy { get; set; }
        public float DuDx { get; set; }
        public float DvDx { get; set; }
        public float DuDy { get; set; }
        public float DvDy { get; set; }
        public int FaceIndex { get; set; } = 0;

        public SurfaceInteraction()
        {
        }

        public SurfaceInteraction(Point3F p, Vector3F pError, Point2F uv, Vector3F wo,
            Vector3F dpdu, Vector3F dpdv,
            Normal3F dndu, Normal3F dndv,
            float time, AbstractShape shape)
            : base(p, new Normal3F(dpdu.Cross(dpdv).Normalized()), pError, wo, time, null)
        {
            Uv = uv;
            DpDu = dpdu;
            DpDv = dpdv;
            DnDu = dndu;
            DnDv = dndv;
            Shape = shape;

            Shading = new Shading
            {
                N = N,
                DnDu = dndu,
                DnDv = dndv,
                DpDu = dpdu,
                DpDv = dpdv
            };
        }

        public void SetShadingGeometry(Vector3F dpdus, Vector3F dpdvs, Normal3F dndus, Normal3F dndvs, bool orientationIsAuthoritative)
        {
            Vector3F vector3F = dpdus.Cross(dpdvs).Normalized();
            Shading.N = new Normal3F(vector3F);
            if (Shape != null && (Shape.ReverseOrientation ^ Shape.TransformSwapsHandedness))
            {
                Shading.N = -Shading.N;
            }

            if (orientationIsAuthoritative)
            {
                N = N.FaceForward(Shading.N);
            }
            else
            {
                Shading.N = Shading.N.FaceForward(N);
            }

            Shading.DpDu = dpdus;
            Shading.DpDv = dpdvs;
            Shading.DnDu = dndus;
            Shading.DnDv = dndvs;
        }
        
        public void ComputeScatteringFunctions(RayDifferential ray, MemoryArena arena, bool allowMultipleLobes, TransportMode mode) 
        {
            ComputeDifferentials(ray);
            Primitive.ComputeScatteringFunctions(this, arena, mode, allowMultipleLobes);
        }

        public void ComputeDifferentials(RayDifferential ray)
        {
            if (!ray.HasDifferentials)
            {
                Reset();
                return;
            }

            // Estimate screen space change in p and (u, v)
            // Compute auxiliary intersection points with plane 
            float d = N.Dot(new Vector3F(P.X, P.Y, P.Z));

            float tx = (-N.Dot(new Vector3F(ray.RxOrigin)) - d) / N.Dot(ray.RxDirection);
            float ty = (-N.Dot(new Vector3F(ray.RyOrigin)) - d) / N.Dot(ray.RyDirection);
            
            if (float.IsInfinity(tx) || float.IsNaN(tx) || float.IsInfinity(ty) || float.IsNaN(ty))
            {
                Reset();
                return;
            }

            Point3F px = ray.RxOrigin + tx * ray.RxDirection;
            Point3F py = ray.RyOrigin + ty * ray.RyDirection;

            DpDx = px - P;
            DpDy = py - P;

            // Compute (u, v) offsets at auxiliary points
            // Choose two dimensions to use for ray offset computation 
            int[] dim = new int[2];
            if (MathF.Abs(N.X) > MathF.Abs(N.Y) && MathF.Abs(N.X) > MathF.Abs(N.Z))
            {
                dim[0] = 1;
                dim[1] = 2;
            }
            else if (MathF.Abs(N.Y) > MathF.Abs(N.Z))
            {
                dim[0] = 0;
                dim[1] = 2;
            }
            else
            {
                dim[0] = 0;
                dim[1] = 1;
            }

            // Initialize A, Bx, and By matrices for offset computation 
            float[][] A = new float[][] { new float[] { DpDu[dim[0]], DpDv[dim[0]] }, new float[] { DpDu[dim[1]], DpDv[dim[1]] } };

            float[] Bx = { px[dim[0]] - P[dim[0]], px[dim[1]] - P[dim[1]] };
            float[] By = { py[dim[0]] - P[dim[0]], py[dim[1]] - P[dim[1]] };

            MathUtils.SolveLinearSystem2X2(A, Bx, out var duDx, out var dvDx);
            MathUtils.SolveLinearSystem2X2(A, By, out var duDy, out var dvDy);
            DuDx = duDx;
            DvDx = dvDx;

            DuDy = duDy; 
            DvDy = dvDy;
        }

        private void Reset()
        {
            DuDx = 0;
            DvDx = 0;
            DuDy = 0;
            DvDy = 0;
            DpDx = new Vector3F(0, 0, 0);
            DpDy = new Vector3F(0, 0, 0);
        }
    }
}