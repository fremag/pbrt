using System;
using System.Diagnostics;

namespace pbrt.Core
{
    [DebuggerDisplay("Nx[{X}] Ny[{Y}] Nz[{Z}]")]
    public class Normal3F
    {
        private const double Epsilon = 1e-9;

        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Normal3F() : this(0, 0, 0)
        {
        }

        public Normal3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Normal3F(Normal3F n) : this(n.X, n.Y, n.Z)
        {
        }

        public Normal3F(Vector3F n) : this(n.X, n.Y, n.Z)
        {
        }

        public static Normal3F operator -(Normal3F n) => new Normal3F(-n.X, -n.Y, -n.Z);
        public static Normal3F operator +(Normal3F n1, Normal3F n2) => new Normal3F(n1.X + n2.X, n1.Y + n2.Y, n1.Z + n2.Z);
        public static Normal3F operator -(Normal3F n1, Normal3F n2) => new Normal3F(n1.X - n2.X, n1.Y - n2.Y, n1.Z - n2.Z);

        public bool HasNaNs => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);

        public static Normal3F operator *(float f, Normal3F n) => new Normal3F(f * n.X, f * n.Y, f * n.Z);
        public static Normal3F operator *(Normal3F n, float f) => new Normal3F(f * n.X, f * n.Y, f * n.Z);
        public static Normal3F operator /(Normal3F n, float f) => new Normal3F(n.X / f,  n.Y / f, n.Z /f);

        public float LengthSquared => X * X + Y * Y + Z * Z;
        public float Length => MathF.Sqrt(LengthSquared);

        public Normal3F Normalize() => this / Length;

        public static bool operator ==(Normal3F n1, Normal3F n2)
        {
            if (ReferenceEquals(n1, null) && !ReferenceEquals(n2, null) || (!ReferenceEquals(n1, null) && ReferenceEquals(n2, null)))
            {
                return false;
            }

            if (ReferenceEquals(n1, n2))
            {
                return true;
            }

            return Math.Abs(n1.X - n2.X) < Epsilon && Math.Abs(n1.Y - n2.Y) < Epsilon && Math.Abs(n1.Z - n2.Z) < Epsilon;
        }

        public static bool operator !=(Normal3F n1, Normal3F n2)
        {
            return !(n1 == n2);
        }

        public bool Equals(Normal3F other)
        {
            if (ReferenceEquals(this, other)) return true;
            return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Normal3F)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public float this[int i]
        {
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

        public float Dot(Vector3F v) => Vector3F.Dot(this, v);
        public float Dot(Normal3F n) => X * n.X + Y * n.Y + Z * n.Z;
        public float AbsDot(Vector3F v) => MathF.Abs(Dot(v));

        public Normal3F FaceForward(Vector3F v) => Dot(v) < 0f ? -this : this;
        public Normal3F FaceForward(Normal3F n) => Dot(n) < 0f ? -this : this;
        public override string ToString() => $"Nx[{X}] Ny[{Y}] Nz[{Z}]";
    }
}