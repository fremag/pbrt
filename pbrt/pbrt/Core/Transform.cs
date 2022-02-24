using System;
using System.Numerics;

namespace pbrt.Core
{
    public class Transform
    {
        private Matrix4x4 M { get; }
        private Matrix4x4 MInv { get; }

        public Transform(float[][] mat)
        {
            M = new Matrix4x4(mat[0][0], mat[0][1], mat[0][2], mat[0][3],
                mat[1][0], mat[1][1], mat[1][2], mat[1][3],
                mat[2][0], mat[2][1], mat[2][2], mat[2][3],
                mat[3][0], mat[3][1], mat[3][2], mat[3][3]);
            Matrix4x4.Invert(M, out var mInv);
            MInv = mInv;
        }

        public Transform(Matrix4x4 m)
        {
            M = m;
            Matrix4x4.Invert(m, out var mInv);
            MInv = mInv;
        }

        public Transform(Matrix4x4 m, Matrix4x4 mInv)
        {
            M = m;
            MInv = mInv;
        }

        public Transform Inverse() => new Transform(MInv, M);
        public Transform Transpose() => new Transform(Matrix4x4.Transpose(M), Matrix4x4.Transpose(MInv));

        public static bool operator ==(Transform t1, Transform t2)
        {
            var t1IsNull = ReferenceEquals(t1, null);
            var t2IsNull = ReferenceEquals(t2, null);
            if (t1IsNull && ! t2IsNull)
            {
                return false;
            }

            if (t2IsNull && ! t1IsNull)
            {
                return false;
            }

            if (t1IsNull)
            {
                return true;
            }

            return t1.M.Equals(t2.M) && t1.MInv.Equals(t2.MInv);
        }

        public static bool operator !=(Transform t1, Transform t2)
        {
            return !(t1 == t2);
        }

        public float this[int i, int j]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        switch (j)
                        {
                            case 0:
                                return M.M11;
                            case 1:
                                return M.M12;
                            case 2:
                                return M.M13;
                            case 3:
                                return M.M14;
                        }

                        break;

                    case 1:
                        switch (j)
                        {
                            case 0:
                                return M.M21;
                            case 1:
                                return M.M22;
                            case 2:
                                return M.M23;
                            case 3:
                                return M.M24;
                        }

                        break;

                    case 2:
                        switch (j)
                        {
                            case 0:
                                return M.M31;
                            case 1:
                                return M.M32;
                            case 2:
                                return M.M33;
                            case 3:
                                return M.M34;
                        }

                        break;

                    case 3:
                        switch (j)
                        {
                            case 0:
                                return M.M41;
                            case 1:
                                return M.M42;
                            case 2:
                                return M.M43;
                            case 3:
                                return M.M44;
                        }

                        break;
                }

                throw new IndexOutOfRangeException();
            }
        }

        public static bool operator <(Transform t1, Transform t2)
        {
            for (int i = 0; i < 4; ++i)
            for (int j = 0; j < 4; ++j)
            {
                if (t1[i, j] < t2[i, j]) return true;
                if (t1[i, j] > t2[i, j]) return false;
            }

            return false;
        }

        public static bool operator >(Transform t1, Transform t2)
        {
            return t2 < t1;
        }

        public bool IsIdentity => M.IsIdentity;


        public static Transform Translate(Vector3F delta)
        {
            return Translate(delta.X, delta.Y, delta.Z);
        }

        public static Transform Translate(float x, float y, float z)
        {
            var m = new Matrix4x4(
                1, 0, 0, x,
                0, 1, 0, y,
                0, 0, 1, z,
                0, 0, 0, 1);

            var mInv = new Matrix4x4(
                1, 0, 0, -x,
                0, 1, 0, -y,
                0, 0, 1, -z,
                0, 0, 0, 1);

            return new Transform(m, mInv);
        }


        public static Transform Scale(float x, float y, float z)
        {
            var m = new Matrix4x4(
                x, 0, 0, 0,
                0, y, 0, 0,
                0, 0, z, 0,
                0, 0, 0, 1);

            var mInv = new Matrix4x4(
                1 / x, 0, 0, 0,
                0, 1 / y, 0, 0,
                0, 0, 1 / z, 0,
                0, 0, 0, 1);

            return new Transform(m, mInv);
        }

        public static Transform RotateX(float deg)
        {
            var rad = deg * MathF.PI / 180;

            var m = new Matrix4x4(
                1, 0, 0, 0,
                0, MathF.Cos(rad), -MathF.Sin(rad), 0,
                0, MathF.Sin(rad), MathF.Cos(rad), 0,
                0, 0, 0, 1);

            var t = Matrix4x4.Transpose(m);
            return new Transform(m, t);
        }

        public static Transform RotateY(float deg)
        {
            var rad = deg * MathF.PI / 180;

            var m = new Matrix4x4(
                MathF.Cos(rad), 0, MathF.Sin(rad), 0,
                0, 1, 0, 0,
                -MathF.Sin(rad), 0, MathF.Cos(rad), 0,
                0, 0, 0, 1);

            var t = Matrix4x4.Transpose(m);
            return new Transform(m, t);
        }

        public static Transform RotateZ(float deg)
        {
            var rad = deg * MathF.PI / 180;

            var m = new Matrix4x4(
                MathF.Cos(rad), -MathF.Sin(rad), 0, 0,
                MathF.Sin(rad), MathF.Cos(rad), 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);

            var t = Matrix4x4.Transpose(m);
            return new Transform(m, t);
        }

        public static Transform LookAt(Point3F pos, Point3F look, Vector3F up)
        {
            Matrix4x4 cameraToWorld = new Matrix4x4();

            cameraToWorld.M14 = pos.X;
            cameraToWorld.M24 = pos.Y;
            cameraToWorld.M34 = pos.Z;
            cameraToWorld.M44 = 1;

            var dir = (look - pos).Normalized();
            var left = Vector3F.Cross(up.Normalized(), dir);
            up = Vector3F.Cross(dir, left);

            cameraToWorld.M11 = left.X;
            cameraToWorld.M21 = left.Y;
            cameraToWorld.M31 = left.Z;
            cameraToWorld.M41 = 0;

            cameraToWorld.M12 = up.X;
            cameraToWorld.M22 = up.Y;
            cameraToWorld.M32 = up.Z;
            cameraToWorld.M42 = 0;

            cameraToWorld.M13 = dir.X;
            cameraToWorld.M23 = dir.Y;
            cameraToWorld.M33 = dir.Z;
            cameraToWorld.M43 = 0;

            Matrix4x4.Invert(cameraToWorld, out var inverted);
            return new Transform(inverted, cameraToWorld);
        }

        protected bool Equals(Transform other)
        {
            return M.Equals(other.M) && MInv.Equals(other.MInv);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Transform)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(M, MInv);
        }

        public Point3F Apply(Point3F p)
        {
            float x = p.X;
            float y = p.Y;
            float z = p.Z;
            const float w = 1;
            float xp = M.M11 * x + M.M12 * y + M.M13 * z + M.M14 * w;
            float yp = M.M21 * x + M.M22 * y + M.M23 * z + M.M24 * w;
            float zp = M.M31 * x + M.M32 * y + M.M33 * z + M.M34 * w;
            float wp = M.M41 * x + M.M42 * y + M.M43 * z + M.M44 * w;

            if (Math.Abs(wp - 1) < float.Epsilon)
            {
                return new Point3F(xp, yp, zp);
            }

            return new Point3F(xp / wp, yp / wp, zp / wp);
        }

        public Vector3F Apply(Vector3F v)
        {
            float x = v.X;
            float y = v.Y;
            float z = v.Z;

            float xp = M.M11 * x + M.M12 * y + M.M13 * z;
            float yp = M.M21 * x + M.M22 * y + M.M23 * z;
            float zp = M.M31 * x + M.M32 * y + M.M33 * z;

            return new Vector3F(xp, yp, zp);
        }

        public Normal3F Apply(Normal3F n)
        {
            float x = n.X;
            float y = n.Y;
            float z = n.Z;

            var nX = MInv.M11 * x + MInv.M21 * y + MInv.M31 * z;
            var nY = MInv.M12 * x + MInv.M22 * y + MInv.M32 * z;
            var nZ = MInv.M13 * x + MInv.M23 * y + MInv.M33 * z;
            
            return new Normal3F(nX, nY, nZ);
        }

        public Point3F Apply(Point3F p, out Vector3F pError)
        {
            float xAbsSum = MathF.Abs(M.M11 * p.X) + MathF.Abs(M.M12 * p.Y) + MathF.Abs(M.M13 * p.Z) + MathF.Abs(M.M14);
            float yAbsSum = (MathF.Abs(M.M21 * p.X) + MathF.Abs(M.M22 * p.Y) + MathF.Abs(M.M23 * p.Z) + MathF.Abs(M.M24));
            float zAbsSum = (MathF.Abs(M.M31 * p.X) + MathF.Abs(M.M32 * p.Y) + MathF.Abs(M.M33 * p.Z) + MathF.Abs(M.M34));
            var g3 = MathUtils.Gamma(3);
            pError = new Vector3F(g3 * xAbsSum, g3 * yAbsSum, g3 * zAbsSum);

            return Apply(p);
        }

        public Ray Apply(Ray r)
        {
            Point3F o = Apply(r.O, out var oError);
            Vector3F d = Apply(r.D);
            float lengthSquared = d.LengthSquared;
            float tMax = r.TMax;
            if (lengthSquared > 0)
            {
                float dt = d.Abs().Dot(oError) / lengthSquared;
                o += d * dt;
                tMax -= dt;
            }

            return new Ray(o, d, tMax, r.Time, r.Medium);
        }

        public Bounds3F Apply(Bounds3F b)
        {
            var p0 = Apply(b.PMin);
            var p1 = Apply(new Point3F(b.PMax.X, b.PMin.Y, b.PMin.Z));
            var p2 = Apply(new Point3F(b.PMin.X, b.PMax.Y, b.PMin.Z));
            var p3 = Apply(new Point3F(b.PMin.X, b.PMin.Y, b.PMax.Z));
            var p4 = Apply(new Point3F(b.PMin.X, b.PMax.Y, b.PMax.Z));
            var p5 = Apply(new Point3F(b.PMax.X, b.PMax.Y, b.PMin.Z));
            var p6 = Apply(new Point3F(b.PMax.X, b.PMin.Y, b.PMax.Z));
            var p7 = Apply(new Point3F(b.PMax.X, b.PMax.Y, b.PMax.Z));

            var tb = new Bounds3F(p0);
            tb = tb.Union(p1);
            tb = tb.Union(p2);
            tb = tb.Union(p3);
            tb = tb.Union(p4);
            tb = tb.Union(p5);
            tb = tb.Union(p6);
            tb = tb.Union(p7);
            return tb;
        }

        public static Point3F operator *(Transform t, Point3F p) => t.Apply(p);
        public static Vector3F operator *(Transform t, Vector3F v) => t.Apply(v);
        public static Normal3F operator *(Transform t, Normal3F n) => t.Apply(n);
        public static Ray operator *(Transform t, Ray r) => t.Apply(r);
        public static Bounds3F operator *(Transform t, Bounds3F b) => t.Apply(b);
        public static Transform operator *(Transform t1, Transform t2) => new Transform(t1.M * t2.M, t2.MInv * t1.MInv);

        public bool SwapsHandedness()
        {
            // determinant of the transformation’s upper-left 3x3 sub matrix
            var a = M.M11 * (M.M22 * M.M33 - M.M23 * M.M32);
            var b = M.M12 * (M.M21 * M.M33 - M.M23 * M.M31);
            var c = M.M13 * (M.M21 * M.M32 - M.M22 * M.M31);
            float det = a - b + c;
            return det < 0;
        }

        public Vector3F Apply(Vector3F v, out Vector3F absError)
        {
            float x = v.X;
            float y = v.Y;
            float z = v.Z;
            var absErrorX = MathF.Abs(M.M11 * v.X) + MathF.Abs(M.M12 * v.Y) + MathF.Abs(M.M13 * v.Z);
            var absErrorY = MathF.Abs(M.M21 * v.X) + MathF.Abs(M.M22 * v.Y) + MathF.Abs(M.M23 * v.Z);
            var absErrorZ = MathF.Abs(M.M31 * v.X) + MathF.Abs(M.M32 * v.Y) + MathF.Abs(M.M33 * v.Z);

            var g3 = MathUtils.Gamma(3);
            absError = new Vector3F(g3 * absErrorX, g3 * absErrorY, g3 * absErrorZ);

            var vX = M.M11 * x + M.M12 * y + M.M13 * z;
            var vY = M.M21 * x + M.M22 * y + M.M23 * z;
            var vZ = M.M31 * x + M.M32 * y + M.M33 * z;

            return new Vector3F(vX, vY, vZ);
        }

        public Ray Apply(Ray ray, out Vector3F oError, out Vector3F dError)
        {
            Point3F o = Apply(ray.O, out oError);
            Vector3F d = Apply(ray.D, out dError);
            float tMax = ray.TMax;
            float lengthSquared = d.LengthSquared;
            if (lengthSquared > 0)
            {
                float dt = d.Abs().Dot(oError) / lengthSquared;
                o += d * dt;
            }

            return new Ray(o, d, tMax, ray.Time, ray.Medium);
        }

        public Point3F Apply(Point3F pt, Vector3F ptError, out Vector3F absError)
        {
            float x = pt.X, y = pt.Y, z = pt.Z;
            float xp = (M.M11 * x + M.M12 * y) + (M.M13 * z + M.M14);
            float yp = (M.M21 * x + M.M22 * y) + (M.M23 * z + M.M24);
            float zp = (M.M31 * x + M.M32 * y) + (M.M33 * z + M.M34);
            float wp = (M.M41 * x + M.M42 * y) + (M.M43 * z + M.M44);
            var absErrorX =
                (MathUtils.Gamma(3) + (float)1) *
                (MathF.Abs(M.M11) * ptError.X
                 + MathF.Abs(M.M12) * ptError.Y
                 + MathF.Abs(M.M13) * ptError.Z)
                + MathUtils.Gamma(3) * (MathF.Abs(M.M11 * x) + MathF.Abs(M.M12 * y) + MathF.Abs(M.M13 * z) + MathF.Abs(M.M14));

            var absErrorY =
                (MathUtils.Gamma(3) + (float)1) *
                (MathF.Abs(M.M21) * ptError.X + MathF.Abs(M.M22) * ptError.Y +
                 MathF.Abs(M.M23) * ptError.Z) +
                MathUtils.Gamma(3) * (MathF.Abs(M.M21 * x) + MathF.Abs(M.M22 * y) +
                                      MathF.Abs(M.M23 * z) + MathF.Abs(M.M24));
            var absErrorZ =
                (MathUtils.Gamma(3) + (float)1) *
                (MathF.Abs(M.M31) * ptError.X + MathF.Abs(M.M32) * ptError.Y +
                 MathF.Abs(M.M33) * ptError.Z) +
                MathUtils.Gamma(3) * (MathF.Abs(M.M31 * x) + MathF.Abs(M.M32 * y) +
                                      MathF.Abs(M.M33 * z) + MathF.Abs(M.M34));

            absError = new Vector3F(absErrorX, absErrorY, absErrorZ);
            if (Math.Abs(wp - 1) < float.Epsilon)
                return new Point3F(xp, yp, zp);
            return new Point3F(xp, yp, zp) / wp;
        }

        // https://github.com/mmp/pbrt-v3/blob/aaa552a4b9cbf9dccb71450f47b268e0ed6370e2/src/core/transform.cpp#L262
        public SurfaceInteraction Apply(SurfaceInteraction si)
        {
            // Transform _p_ and _pError_ in _SurfaceInteraction_
            var p = Apply(si.P, si.PError, out var pError);

            // Transform remaining members of _SurfaceInteraction_
            var n = Apply(si.N).Normalize();
            var wo = Apply(si.Wo).Normalized();
            var dpdu = Apply(si.DpDu);
            var dpdv = Apply(si.DpDv);
            var dndu = Apply(si.DnDu);
            var dndv = Apply(si.DnDv);
            var shadingN = Apply(si.Shading.N).Normalize();
            var shadingDpdu = Apply(si.Shading.DpDu);
            var shadingDpdv = Apply(si.Shading.DpDv);
            var shadingDndu = Apply(si.Shading.DnDu);
            var shadingDndv = Apply(si.Shading.DnDv);

            var dpdx = Apply(si.DpDx);
            var dpdy = Apply(si.DpDy);
            //    ret.n = Faceforward(ret.n, ret.shading.n);
            shadingN = shadingN.FaceForward(n);
            var faceIndex = si.FaceIndex;

            var shading = new Shading
            {
                N = shadingN, DnDu = shadingDndu, DnDv = shadingDndv, DpDu = shadingDpdu, DpDv = shadingDpdv
            };

            var surfaceInteraction = new SurfaceInteraction(p, pError, si.Uv, wo, dpdu, dpdv, dndu, dndv, si.Time, si.Shape)
            {
                Shading = shading,
                Bsdf = si.Bsdf,
                Bssrdf = si.Bssrdf,
                DuDx = si.DuDx,
                DvDx = si.DvDx,
                DuDy = si.DuDy,
                DvDy = si.DvDy,
                DpDx = dpdx,
                DpDy = dpdy,
                Primitive = si.Primitive,
                FaceIndex = faceIndex,
                MediumInterface = si.MediumInterface
            };
            return surfaceInteraction;
        }
    }
}