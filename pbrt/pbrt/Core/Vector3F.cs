using System;

namespace pbrt.Core
{
    public class Vector3F
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }
        private const double Epsilon = 1e-9;

        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3F(Vector3F v) : this (v.X, v.Y, v.Z)
        { }

        public Vector3F(Normal3F n) : this (n.X, n.Y, n.Z)
        { }

        public float this[int i] {
            get
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    2 => Z,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }

        public static Vector3F operator +(Vector3F v1, Vector3F v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); 
        public static Vector3F operator -(Vector3F v1, Vector3F v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        public static Vector3F operator *(Vector3F v, float f) => new(v.X *f, v.Y *f, v.Z *f);
        public static Vector3F operator /(Vector3F v, float f) => new(v.X /f, v.Y /f, v.Z /f);
        public static Vector3F operator -(Vector3F v) => new(-v.X , -v.Y , -v.Z );

        public float LengthSquared => X*X + Y*Y + Z*Z;
        public float Length => MathF.Sqrt(LengthSquared);

        public Vector3F Abs()
        {
            return new Vector3F(MathF.Abs(X), MathF.Abs(Y), MathF.Abs(Z));
        }

        public static float Dot(Vector3F v1, Vector3F v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        public static float AbsDot(Vector3F v, Vector3F v2) => MathF.Abs(Dot(v,v2));
        public static float Dot(Vector3F v1, Normal3F n) => v1.X * n.X + v1.Y * n.Y + v1.Z * n.Z;
        public static float AbsDot(Vector3F v, Normal3F n) => MathF.Abs(Dot(v,n));
        public static float Dot(Normal3F n1, Vector3F v) => n1.X * v.X + n1.Y * v.Y + n1.Z * v.Z;
        public static float AbsDot(Normal3F n, Vector3F v) => MathF.Abs(Dot(n,v));

        public float Dot(Vector3F v) => Dot(this, v);
        public float AbsDot(Vector3F v) => MathF.Abs(Dot(v));

        public static Vector3F Cross(Vector3F v1, Vector3F v2) => new(
            v1.Y*v2.Z-v1.Z*v2.Y, 
            v1.Z*v2.X-v1.X*v2.Z, 
            v1.X*v2.Y-v1.Y*v2.X);
        public static Vector3F Normalize(Vector3F v) => v / v.Length;
        public Vector3F Normalized() => Normalize(this);

        public float MinComponent => MathF.Min(X, MathF.Min(Y, Z));
        public float MaxComponent => MathF.Max(X, MathF.Max(Y, Z));
        public float MaxDimension
        {
            get
            {
                if (X > Y)
                    return X > Z ? 0 : 2;
                return Y > Z ? 1 : 2;
            }
        }

        public static Vector3F Min(Vector3F v1, Vector3F v2) => new(MathF.Min(v1.X, v2.X), MathF.Min(v1.Y, v2.Y), MathF.Min(v1.Z, v2.Z));
        public static Vector3F Max(Vector3F v1, Vector3F v2) => new(MathF.Max(v1.X, v2.X), MathF.Max(v1.Y, v2.Y), MathF.Max(v1.Z, v2.Z));
        public Vector3F Permute(int x, int y, int z) => new(this[x], this[y], this[z]);
        
        public static bool operator ==(Vector3F v1, Vector3F v2)
        {
            if ((v1 == null && v2 != null) || (v1 != null && v2 == null))
            {
                return false;
            }

            if (v1 == null)
            {
                return true;
            }

            return Math.Abs(v1.X - v2.X) < Epsilon && Math.Abs(v1.Y - v2.Y) < Epsilon && Math.Abs(v1.Z - v2.Z) < Epsilon;
        }

        public static bool operator !=(Vector3F v1, Vector3F v2)
        {
            return !(v1 == v2);
        }
        protected bool Equals(Vector3F other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector3F)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public void CoordinateSystem(Vector3F v1, out Vector3F v2, out Vector3F v3)
        {
            if (MathF.Abs(v1.X) > MathF.Abs(v1.Y))
            {
                v2 = new Vector3F(-v1.Z, 0, v1.X) / MathF.Sqrt(v1.X*v1.X+v1.Z*v1.Z);
            }
            else
            {
                v2 = new Vector3F(0, v1.Z, -v1.Y) / MathF.Sqrt(v1.Y*v1.Y+v1.Z*v1.Z);
            }

            v3 = Cross(v1, v2);
        }
        
        public static Normal3F Faceforward(Normal3F n, Vector3F v) => Dot(n, v) < 0 ? -n : n;
    }
}