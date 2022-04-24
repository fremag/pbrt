using System;
using System.Linq;
using pbrt.Core;

namespace pbrt.Shapes
{
    public class Triangle : AbstractShape
    {
        readonly TriangleMesh mesh;
        readonly int faceIndex;
        private readonly int triIndex;

        public Point3F P0
        {
            get
            {
                var meshVertexIndex = mesh.VertexIndices[triIndex];
                return mesh.P[meshVertexIndex];
            }
        }

        public Point3F P1
        {
            get
            {
                var meshVertexIndex = mesh.VertexIndices[triIndex + 1];
                return mesh.P[meshVertexIndex];
            }
        }

        public Point3F P2
        {
            get
            {
                var meshVertexIndex = mesh.VertexIndices[triIndex + 2];
                return mesh.P[meshVertexIndex];
            }
        }

        public Triangle(Transform objectToWorld, Transform worldToObject,
            bool reverseOrientation, TriangleMesh mesh, int triNumber)
            : base(objectToWorld, worldToObject, reverseOrientation)
        {
            this.mesh = mesh;
            triIndex = 3 * triNumber;
            faceIndex = mesh.FaceIndices != null && mesh.FaceIndices.Any() ? mesh.FaceIndices[triNumber] : 0;

            // Get triangle vertices in p0, p1, and p2
            var p0 = worldToObject.Apply(P0);
            var p1 = worldToObject.Apply(P1);
            var p2 = worldToObject.Apply(P2);
            ObjectBound = new Bounds3F(p0, p1).Union(p2);
        }

        // Triangle Private Methods
        public void GetUVs(Point2F[] uv)
        {
            if (mesh.Uv != null)
            {
                uv[0] = mesh.Uv[triIndex];
                uv[1] = mesh.Uv[triIndex + 1];
                uv[2] = mesh.Uv[triIndex + 2];
            }
            else
            {
                uv[0] = new Point2F(0, 0);
                uv[1] = new Point2F(1, 0);
                uv[2] = new Point2F(1, 1);
            }
        }

        public override float Area
        {
            get
            {
                var v1 = P1 - P0;
                var v2 = P2 - P0;
                var v = v1.Cross(v2);
                return 0.5f * v.Length;
            }
        }

        public override bool Intersect(Ray ray, out float tHit, out SurfaceInteraction isect, bool testAlphaTexture = true)
        {
            tHit = 0;
            isect = null;

            // Perform ray--triangle intersection test

            // Transform triangle vertices to ray coordinate space

            // Translate vertices based on ray origin
            Point3F p0T = P0 - new Vector3F(ray.O);
            Point3F p1T = P1 - new Vector3F(ray.O);
            Point3F p2T = P2 - new Vector3F(ray.O);

            // Permute components of triangle vertices and ray direction
            int kz = ray.D.Abs().MaxDimension;
            int kx = kz + 1;
            if (kx == 3)
            {
                kx = 0;
            }

            int ky = kx + 1;
            if (ky == 3)
            {
                ky = 0;
            }

            Vector3F d = ray.D.Permute(kx, ky, kz);
            p0T = p0T.Permute(kx, ky, kz);
            p1T = p1T.Permute(kx, ky, kz);
            p2T = p2T.Permute(kx, ky, kz);

            // Apply shear transformation to translated vertex positions
            float sx = -d.X / d.Z;
            float sy = -d.Y / d.Z;
            float sz = 1f / d.Z;
            p0T.X += sx * p0T.Z;
            p0T.Y += sy * p0T.Z;
            p1T.X += sx * p1T.Z;
            p1T.Y += sy * p1T.Z;
            p2T.X += sx * p2T.Z;
            p2T.Y += sy * p2T.Z;

            // Compute edge function coefficients e0, e1, and e2
            float e0 = p1T.X * p2T.Y - p1T.Y * p2T.X;
            float e1 = p2T.X * p0T.Y - p2T.Y * p0T.X;
            float e2 = p0T.X * p1T.Y - p0T.Y * p1T.X;

            // Fall back to double precision test at triangle edges
            if ((e0 == 0.0f || e1 == 0.0f || e2 == 0.0f))
            {
                double p2txp1ty = (double)p2T.X * (double)p1T.Y;
                double p2typ1tx = (double)p2T.Y * (double)p1T.X;
                e0 = (float)(p2typ1tx - p2txp1ty);
                double p0txp2ty = (double)p0T.X * (double)p2T.Y;
                double p0typ2tx = (double)p0T.Y * (double)p2T.X;
                e1 = (float)(p0typ2tx - p0txp2ty);
                double p1txp0ty = (double)p1T.X * (double)p0T.Y;
                double p1typ0tx = (double)p1T.Y * (double)p0T.X;
                e2 = (float)(p1typ0tx - p1txp0ty);
            }

            // Perform triangle edge and determinant tests
            if ((e0 < 0 || e1 < 0 || e2 < 0) && (e0 > 0 || e1 > 0 || e2 > 0))
            {
                return false;
            }

            float det = e0 + e1 + e2;
            if (det == 0)
            {
                return false;
            }

            // Compute scaled hit distance to triangle and test against ray t range
            p0T.Z *= sz;
            p1T.Z *= sz;
            p2T.Z *= sz;
            float tScaled = e0 * p0T.Z + e1 * p1T.Z + e2 * p2T.Z;
            if (det < 0 && (tScaled >= 0 || tScaled < ray.TMax * det))
            {
                return false;
            }

            if (det > 0 && (tScaled <= 0 || tScaled > ray.TMax * det))
            {
                return false;
            }

            // Compute barycentric coordinates and t value for triangle intersection
            float invDet = 1 / det;
            float b0 = e0 * invDet;
            float b1 = e1 * invDet;
            float b2 = e2 * invDet;
            float t = tScaled * invDet;

            // Ensure that computed triangle t is conservatively greater than zero

            // Compute delta_z term for triangle t error bounds
            float maxZt = new Vector3F(p0T.Z, p1T.Z, p2T.Z).Abs().MaxComponent;
            float deltaZ = MathUtils.Gamma(3) * maxZt;

            // Compute delta_x and delta_y terms for triangle t error bounds
            float maxXt = new Vector3F(p0T.X, p1T.X, p2T.X).Abs().MaxComponent;
            float maxYt = new Vector3F(p0T.Y, p1T.Y, p2T.Y).Abs().MaxComponent;
            float deltaX = MathUtils.Gamma(5) * (maxXt + maxZt);
            float deltaY = MathUtils.Gamma(5) * (maxYt + maxZt);

            // Compute delta_e term for triangle t error bounds
            float deltaE = 2 * (MathUtils.Gamma(2) * maxXt * maxYt + deltaY * maxXt + deltaX * maxYt);

            // Compute delta_t term for triangle t error bounds and check t
            float maxE = new Vector3F(e0, e1, e2).Abs().MaxComponent;
            float deltaT = 3 * MathF.Abs(invDet);
            var x1 = MathUtils.Gamma(3) * maxE * maxZt;
            var x2 = deltaE * maxZt;
            var x3 = deltaZ * maxE;
            deltaT *= (x1 + x2 + x3);
            if (t <= deltaT)
            {
                return false;
            }

            // Compute triangle partial derivatives
            Vector3F dpdu = new Vector3F(0, 0, 0);
            Vector3F dpdv = new Vector3F(0, 0, 0);
            Point2F[] uv = new Point2F[3];
            GetUVs(uv);

            // Compute deltas for triangle partial derivatives
            Vector2F duv02 = uv[0] - uv[2];
            Vector2F duv12 = uv[1] - uv[2];
            Vector3F dp02 = P0 - P2;
            Vector3F dp12 = P1 - P2;
            float determinant = duv02[0] * duv12[1] - duv02[1] * duv12[0];
            bool degenerateUv = MathF.Abs(determinant) < 1e-8;

            if (!degenerateUv)
            {
                float invdet = 1 / determinant;
                dpdu = (duv12[1] * dp02 - duv02[1] * dp12) * invdet;
                dpdv = (-duv12[0] * dp02 + duv02[0] * dp12) * invdet;
            }

            if (degenerateUv || dpdu.Cross(dpdv).LengthSquared == 0)
            {
                // Handle zero determinant for triangle partial derivative matrix
                Vector3F ng = (P2 - P0).Cross(P1 - P0);
                if (ng.LengthSquared == 0)
                {
                    // The triangle is actually degenerate; the intersection is bogus.
                    return false;
                }

                Vector3F.CoordinateSystem(ng.Normalized(), out dpdu, out dpdv);
            }

            // Compute error bounds for triangle intersection
            float xAbsSum = (MathF.Abs(b0 * P0.X) + MathF.Abs(b1 * P1.X) + MathF.Abs(b2 * P2.X));
            float yAbsSum = (MathF.Abs(b0 * P0.Y) + MathF.Abs(b1 * P1.Y) + MathF.Abs(b2 * P2.Y));
            float zAbsSum = (MathF.Abs(b0 * P0.Z) + MathF.Abs(b1 * P1.Z) + MathF.Abs(b2 * P2.Z));
            Vector3F pError = MathUtils.Gamma(7) * new Vector3F(xAbsSum, yAbsSum, zAbsSum);

            // Interpolate (u,v) parametric coordinates and hit point
            Point3F pHit = b0 * P0 + b1 * P1 + b2 * P2;
            Point2F uvHit = b0 * uv[0] + b1 * uv[1] + b2 * uv[2];

            // Test intersection against alpha texture, if present
            if (testAlphaTexture && mesh.AlphaMask != null)
            {
                SurfaceInteraction isectLocal = new SurfaceInteraction(pHit, new Vector3F(0, 0, 0), uvHit, -ray.D, dpdu, dpdv, new Normal3F(0, 0, 0), new Normal3F(0, 0, 0), ray.Time, this);
                if (mesh.AlphaMask.Evaluate(isectLocal) == 0)
                {
                    return false;
                }
            }

            // Fill in SurfaceInteraction from triangle hit
            isect = new SurfaceInteraction(pHit, pError, uvHit, -ray.D, dpdu, dpdv, new Normal3F(0, 0, 0), new Normal3F(0, 0, 0), ray.Time, this, faceIndex);

            // Override surface normal in isect for triangle
            isect.N = isect.Shading.N = new Normal3F(dp02.Cross(dp12).Normalized());
            if (ReverseOrientation ^ TransformSwapsHandedness)
            {
                isect.Shading.N = -isect.N;
                isect.N = isect.Shading.N;
            }

            if (mesh.N != null || mesh.S != null)
            {
                // Initialize Triangle shading geometry

                // Compute shading normal ns for triangle
                Normal3F ns;
                if (mesh.N != null)
                {
                    ns = (b0 * mesh.N[triIndex] + b1 * mesh.N[triIndex + 1] + b2 * mesh.N[triIndex + 2]);
                    if (ns.LengthSquared > 0)
                    {
                        ns = ns.Normalize();
                    }
                    else
                    {
                        ns = isect.N;
                    }
                }
                else
                {
                    ns = isect.N;
                }

                // Compute shading tangent ss for triangle
                Vector3F ss;
                if (mesh.S != null)
                {
                    ss = (b0 * mesh.S[triIndex] + b1 * mesh.S[triIndex + 1] + b2 * mesh.S[triIndex + 2]);
                    if (ss.LengthSquared > 0)
                    {
                        ss = ss.Normalized();
                    }
                    else
                    {
                        ss = isect.DpDu.Normalized();
                    }
                }
                else
                {
                    ss = isect.DpDu.Normalized();
                }

                // Compute shading bitangent ts for triangle and adjust ss
                Vector3F ts = ss.Cross((Vector3F)ns);
                if (ts.LengthSquared > 0f)
                {
                    ts = ts.Normalized();
                    ss = ts.Cross((Vector3F)ns);
                }
                else
                {
                    Vector3F.CoordinateSystem((Vector3F)(ns), out ss, out ts);
                }

                // Compute dndu and dndv for triangle shading geometry
                Normal3F dndu, dndv;
                if (mesh.N != null)
                {
                    // Compute deltas for triangle partial derivatives of normal
                    duv02 = uv[0] - uv[2];
                    duv12 = uv[1] - uv[2];
                    Normal3F dn1 = mesh.N[triIndex] - mesh.N[triIndex + 2];
                    Normal3F dn2 = mesh.N[triIndex + 1] - mesh.N[triIndex + 2];
                    determinant = duv02[0] * duv12[1] - duv02[1] * duv12[0];
                    degenerateUv = MathF.Abs(determinant) < 1e-8;
                    if (degenerateUv)
                    {
                        // We can still compute dndu and dndv, with respect to the
                        // same arbitrary coordinate system we use to compute dpdu
                        // and dpdv when this happens. It's important to do this
                        // (rather than giving up) so that ray differentials for
                        // rays reflected from triangles with degenerate
                        // parameterizations are still reasonable.
                        Vector3F dn = ((Vector3F)(mesh.N[triIndex + 2] - mesh.N[triIndex])).Cross((Vector3F)(mesh.N[triIndex + 1] - mesh.N[triIndex]));
                        if (dn.LengthSquared == 0)
                        {
                            dndu = dndv = new Normal3F(0, 0, 0);
                        }
                        else
                        {
                            Vector3F.CoordinateSystem(dn, out var dnu, out var dnv);
                            dndu = new Normal3F(dnu);
                            dndv = new Normal3F(dnv);
                        }
                    }
                    else
                    {
                        invDet = 1 / determinant;
                        dndu = (duv12[1] * dn1 - duv02[1] * dn2) * invDet;
                        dndv = (-duv12[0] * dn1 + duv02[0] * dn2) * invDet;
                    }
                }
                else
                {
                    dndu = dndv = new Normal3F(0, 0, 0);
                }

                if (ReverseOrientation)
                {
                    ts = -ts;
                }

                isect.SetShadingGeometry(ss, ts, dndu, dndv, true);
            }

            tHit = t;
            return true;
        }

        public override Interaction Sample(Point2F u)
        {
            Point2F b = MathUtils.UniformSampleTriangle(u);
            // Get triangle vertices in p0, p1, and p2
            Interaction it = new Interaction();
            it.P = b[0] * P0 + b[1] * P1 + (1 - b[0] - b[1]) * P2;
            // Compute surface normal for sampled point on triangle
            it.N = (new Normal3F((P1 - P0).Cross(P2 - P0))).Normalize();
            // Ensure correct orientation of the geometric normal; follow the same
            // approach as was used in Triangle::Intersect().
            if (mesh.N != null)
            {
                Normal3F ns = new Normal3F(b[0] * mesh.N[triIndex] + b[1] * mesh.N[triIndex + 1] + (1 - b[0] - b[1]) * mesh.N[triIndex + 2]);
                it.N = it.N.FaceForward(ns);
            }
            else if (ReverseOrientation ^ TransformSwapsHandedness)
            {
                it.N *= -1;
            }

            // Compute error bounds for sampled point on triangle
            Point3F pAbsSum = (b[0] * P0).Abs() + (b[1] * P1).Abs() + ((1 - b[0] - b[1]) * P2).Abs();
            it.PError = MathUtils.Gamma(6) * new Vector3F(pAbsSum.X, pAbsSum.Y, pAbsSum.Z);
            return it;
        }

        public override bool IntersectP(Ray ray, bool testAlphaTexture = true)
        {
            // Perform ray--triangle intersection test

            // Transform triangle vertices to ray coordinate space

            // Translate vertices based on ray origin
            Point3F p0t = P0 - new Vector3F(ray.O);
            Point3F p1t = P1 - new Vector3F(ray.O);
            Point3F p2t = P2 - new Vector3F(ray.O);

            // Permute components of triangle vertices and ray direction
            int kz = (ray.D.Abs()).MaxDimension;
            int kx = kz + 1;
            if (kx == 3)
            {
                kx = 0;
            }

            int ky = kx + 1;
            if (ky == 3)
            {
                ky = 0;
            }

            Vector3F d = ray.D.Permute(kx, ky, kz);
            p0t = p0t.Permute(kx, ky, kz);
            p1t = p1t.Permute(kx, ky, kz);
            p2t = p2t.Permute(kx, ky, kz);

            // Apply shear transformation to translated vertex positions
            float Sx = -d.X / d.Z;
            float Sy = -d.Y / d.Z;
            float Sz = 1f / d.Z;
            p0t.X += Sx * p0t.Z;
            p0t.Y += Sy * p0t.Z;
            p1t.X += Sx * p1t.Z;
            p1t.Y += Sy * p1t.Z;
            p2t.X += Sx * p2t.Z;
            p2t.Y += Sy * p2t.Z;

            // Compute edge function coefficients e0, e1, and e2
            float e0 = p1t.X * p2t.Y - p1t.Y * p2t.X;
            float e1 = p2t.X * p0t.Y - p2t.Y * p0t.X;
            float e2 = p0t.X * p1t.Y - p0t.Y * p1t.X;

            // Fall back to double precision test at triangle edges
            if (e0 == 0.0f || e1 == 0.0f || e2 == 0.0f)
            {
                double p2txp1ty = (double)p2t.X * (double)p1t.Y;
                double p2typ1tx = (double)p2t.Y * (double)p1t.X;
                e0 = (float)(p2typ1tx - p2txp1ty);
                double p0txp2ty = (double)p0t.X * (double)p2t.Y;
                double p0typ2tx = (double)p0t.Y * (double)p2t.X;
                e1 = (float)(p0typ2tx - p0txp2ty);
                double p1txp0ty = (double)p1t.X * (double)p0t.Y;
                double p1typ0tx = (double)p1t.Y * (double)p0t.X;
                e2 = (float)(p1typ0tx - p1txp0ty);
            }

            // Perform triangle edge and determinant tests
            if ((e0 < 0 || e1 < 0 || e2 < 0) && (e0 > 0 || e1 > 0 || e2 > 0))
            {
                return false;
            }

            float det = e0 + e1 + e2;
            if (det == 0)
            {
                return false;
            }

            // Compute scaled hit distance to triangle and test against ray t range
            p0t.Z *= Sz;
            p1t.Z *= Sz;
            p2t.Z *= Sz;
            float tScaled = e0 * p0t.Z + e1 * p1t.Z + e2 * p2t.Z;
            if (det < 0 && (tScaled >= 0 || tScaled < ray.TMax * det))
            {
                return false;
            }

            if (det > 0 && (tScaled <= 0 || tScaled > ray.TMax * det))
            {
                return false;
            }

            // Compute barycentric coordinates and t value for triangle intersection
            float invDet = 1 / det;
            float b0 = e0 * invDet;
            float b1 = e1 * invDet;
            float b2 = e2 * invDet;
            float t = tScaled * invDet;

            // Ensure that computed triangle t is conservatively greater than zero

            // Compute delta_z term for triangle t error bounds
            float maxZt = new Vector3F(p0t.Z, p1t.Z, p2t.Z).Abs().MaxComponent;
            float deltaZ = MathUtils.Gamma(3) * maxZt;

            // Compute delta_x and delta_y terms for triangle t error bounds
            float maxXt = new Vector3F(p0t.X, p1t.X, p2t.X).Abs().MaxComponent;
            float maxYt = new Vector3F(p0t.Y, p1t.Y, p2t.Y).Abs().MaxComponent;
            float deltaX = MathUtils.Gamma(5) * (maxXt + maxZt);
            float deltaY = MathUtils.Gamma(5) * (maxYt + maxZt);

            // Compute delta_e term for triangle t error bounds
            float deltaE = 2 * (MathUtils.Gamma(2) * maxXt * maxYt + deltaY * maxXt + deltaX * maxYt);

            // Compute delta_t term for triangle t error bounds and check t
            float maxE = new Vector3F(e0, e1, e2).Abs().MaxComponent;
            float deltaT = 3 * (MathUtils.Gamma(3) * maxE * maxZt + deltaE * maxZt + deltaZ * maxE) * MathF.Abs(invDet);
            if (t <= deltaT)
            {
                return false;
            }

            // Test shadow ray intersection against alpha texture, if present
            if (testAlphaTexture && (mesh.AlphaMask != null || mesh.ShadowAlphaMask != null))
            {
                // Compute triangle partial derivatives
                Vector3F dpdu = new Vector3F(0,0,0);
                Vector3F dpdv = new Vector3F(0,0,0);
                Point2F[] uv = new Point2F[3];
                GetUVs(uv);

                // Compute deltas for triangle partial derivatives
                Vector2F duv02 = uv[0] - uv[2], duv12 = uv[1] - uv[2];
                Vector3F dp02 = P0 - P2;
                Vector3F dp12 = P1 - P2;
                float determinant = duv02[0] * duv12[1] - duv02[1] * duv12[0];
                bool degenerateUV = MathF.Abs(determinant) < 1e-8;
                if (!degenerateUV)
                {
                    float invdet = 1 / determinant;
                    dpdu = (duv12[1] * dp02 - duv02[1] * dp12) * invdet;
                    dpdv = (-duv12[0] * dp02 + duv02[0] * dp12) * invdet;
                }

                if (degenerateUV || dpdu.Cross(dpdv).LengthSquared == 0)
                {
                    // Handle zero determinant for triangle partial derivative matrix
                    Vector3F ng = (P2 - P0).Cross(P1 - P0);
                    // The triangle is actually degenerate; the intersection is bogus.
                    if (ng.LengthSquared == 0)
                    {
                        return false;
                    }

                    Vector3F.CoordinateSystem(((P2 - P0).Cross(P1 - P0)).Normalized(), out dpdu, out dpdv);
                }

                // Interpolate (u,v) parametric coordinates and hit point
                Point3F pHit = b0 * P0 + b1 * P1 + b2 * P2;
                Point2F uvHit = b0 * uv[0] + b1 * uv[1] + b2 * uv[2];
                SurfaceInteraction isectLocal = new SurfaceInteraction(pHit, new Vector3F(0, 0, 0), uvHit, -ray.D, dpdu, dpdv, new Normal3F(0, 0, 0), new Normal3F(0, 0, 0), ray.Time, this);
                if (mesh.AlphaMask != null && mesh.AlphaMask.Evaluate(isectLocal) == 0)
                {
                    return false;
                }

                if (mesh.ShadowAlphaMask != null && mesh.ShadowAlphaMask.Evaluate(isectLocal) == 0)
                {
                    return false;
                }
            }

            return true;
        }
    }
}