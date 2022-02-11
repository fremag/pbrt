using System;
using System.Numerics;

namespace pbrt.Core
{
    public class Transform
    {
        Matrix4x4 m, mInv;

        public Transform(float[][] mat)
        {
            m = new Matrix4x4(mat[0][0], mat[0][1], mat[0][2], mat[0][3],
                mat[1][0], mat[1][1], mat[1][2], mat[1][3],
                mat[2][0], mat[2][1], mat[2][2], mat[2][3],
                mat[3][0], mat[3][1], mat[3][2], mat[3][3]);
            Matrix4x4.Invert(m, out mInv);
        }

        public Transform(Matrix4x4 m)
        {
            this.m = m;
            Matrix4x4.Invert(m, out mInv);
        }

        public Transform(Matrix4x4 m, Matrix4x4 mInv)
        {
            this.m = m;
            this.mInv = mInv;
        }

        public Transform Inverse() => new Transform(mInv, m);
        public Transform Transpose() => new Transform(Matrix4x4.Transpose(m), Matrix4x4.Transpose(mInv));

        public static bool operator ==(Transform t1, Transform t2)
        {
            if (t1 == null && t2 != null)
            {
                return false;
            }

            if (t2 == null && t1 != null)
            {
                return false;
            }

            if (t1 == null)
            {
                return true;
            }

            return t1.m.Equals(t2.m) && t1.mInv.Equals(t2.mInv);
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
                                return m.M11;
                            case 1:
                                return m.M12;
                            case 2:
                                return m.M13;
                            case 3:
                                return m.M14;
                        }

                        break;

                    case 1:
                        switch (j)
                        {
                            case 0:
                                return m.M21;
                            case 1:
                                return m.M22;
                            case 2:
                                return m.M23;
                            case 3:
                                return m.M24;
                        }

                        break;

                    case 2:
                        switch (j)
                        {
                            case 0:
                                return m.M31;
                            case 1:
                                return m.M32;
                            case 2:
                                return m.M33;
                            case 3:
                                return m.M34;
                        }

                        break;

                    case 3:
                        switch (j)
                        {
                            case 0:
                                return m.M41;
                            case 1:
                                return m.M42;
                            case 2:
                                return m.M43;
                            case 3:
                                return m.M44;
                        }

                        break;

                }

                throw new IndexOutOfRangeException();
            }
        }

        public static bool operator <(Transform t1, Transform t2)
        {
                for (int i = 0; i < 4; ++i)
                for (int j = 0; j < 4; ++j) {
                    if (t1[i,j] < t2[i,j]) return true;
                    if (t1[i,j] > t2[i,j]) return false;
                }
                return false;
            }

        public static bool operator >(Transform t1, Transform t2)
        {
            return t2 < t1;
        }

        public bool IsIdentity => m.IsIdentity;
        
        
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
                MathF.Cos(rad), 0, -MathF.Sin(rad), 0,
                MathF.Sin(rad), 0, MathF.Cos(rad), 0,
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
    }
}