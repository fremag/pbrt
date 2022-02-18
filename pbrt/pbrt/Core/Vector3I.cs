using System;

namespace pbrt.Core
{
    public class Vector3I
    {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }

        public Vector3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public int this[int i] {
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

        public static Vector3I operator +(Vector3I v1, Vector3I v2) => new(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z); 
        public static Vector3I operator -(Vector3I v1, Vector3I v2) => new(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        public static Vector3F operator *(Vector3I v, float f) => new(v.X *f, v.Y *f, v.Z *f);
        public static Vector3F operator *(float f, Vector3I v) => v * f;
        public static Vector3F operator /(Vector3I v, float f) => new(v.X /f, v.Y /f, v.Z /f);
        public static Vector3I operator -(Vector3I v) => new(-v.X , -v.Y , -v.Z );

        public int LengthSquared => X*X + Y*Y + Z*Z;
        public float Length => MathF.Sqrt(LengthSquared);

        public Vector3I Abs()
        {
            return new Vector3I(Math.Abs(X), Math.Abs(Y), Math.Abs(Z));
        }

        public static int Dot(Vector3I v1, Vector3I v2) => v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z;
        public static int AbsDot(Vector3I v1, Vector3I v2) => Math.Abs(Dot(v1,v2));

        public int Dot(Vector3I v) => Dot(this, v);
        public int AbsDot(Vector3I v) => AbsDot(this, v);

        public Vector3I Cross(Vector3I v) => Cross(this, v);
        public static Vector3I Cross(Vector3I v1, Vector3I v2) => new(
            v1.Y*v2.Z-v1.Z*v2.Y, 
            v1.Z*v2.X-v1.X*v2.Z, 
            v1.X*v2.Y-v1.Y*v2.X);
        public static Vector3F Normalize(Vector3I v) => v / v.Length;
        public Vector3F Normalized() => Normalize(this);

        public int MinComponent => Math.Min(X, Math.Min(Y, Z));
        public int MaxComponent => Math.Max(X, Math.Max(Y, Z));
        public int MaxDimension
        {
            get
            {
                if (X > Y)
                    return X > Z ? 0 : 2;
                return Y > Z ? 1 : 2;
            }
        }

        public static Vector3I Min(Vector3I v1, Vector3I v2) => new(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y), Math.Min(v1.Z, v2.Z));
        public static Vector3I Max(Vector3I v1, Vector3I v2) => new(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y), Math.Max(v1.Z, v2.Z));
        public Vector3I Permute(int x, int y, int z) => new(this[x], this[y], this[z]);
        
        public static bool operator ==(Vector3I v1, Vector3I v2)
        {
            if (ReferenceEquals(v1, null) && ! ReferenceEquals(v2, null) || (! ReferenceEquals(v1, null) && ReferenceEquals(v2, null)))
            {
                return false;
            }

            if (ReferenceEquals(v1, v2))
            {
                return true;
            }

            return v1.X == v2.X && v1.Y == v2.Y && v1.Z == v2.Z;
        }

        public static bool operator !=(Vector3I v1, Vector3I v2)
        {
            return !(v1 == v2);
        }
        protected bool Equals(Vector3I other)
        {
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector3I)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}