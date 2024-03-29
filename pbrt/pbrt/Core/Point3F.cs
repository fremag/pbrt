using System;

namespace pbrt.Core
{
    public class Point3F
    {
        private static readonly double Epsilon = 1e-5;
        private float[] values = new float[3];

        public float X
        {
            get => values[0];
            set => values[0] = value;
        }
        
        public float Y
        {
            get => values[1];
            set => values[1] = value;
        }
        
        public float Z
        {
            get => values[2];
            set => values[2] = value;
        }

        public bool HasNaNs => double.IsNaN(X) || double.IsNaN(Y) || double.IsNaN(Z);
        public static Point3F Zero { get; } = new Point3F(0, 0, 0);

        public Point3F() : this(0, 0, 0)
        {
            
        }

        public Point3F(Point3F p) : this(p.X, p.Y, p.Z)
        {
            
        } 
        
        public Point3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Deconstruct(out float x, out float y, out float z)
        {
            x = X;
            y = Y;
            z = Z;
        }
        public static implicit operator Point3F((float x, float y, float z) p) => new Point3F(p.x, p.y, p.z);

        public Point3F Permute(int x, int y, int z) => new(this[x], this[y], this[z]);
        
        public static explicit operator Point2F(Point3F p) => new Point2F(p.X, p.Y);
        public static explicit operator Point3I(Point3F p) => new Point3I((int)p.X, (int)p.Y, (int)p.Z);
        
        public static Point3F operator +(Point3F p1, Point3F p2) => new Point3F(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        public static Vector3F operator -(Point3F p1, Point3F p2) => new Vector3F(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        public static Vector3F operator -(Point3F p) => new Vector3F(- p.X, -p.Y, -p.Z);
        
        public static bool operator == (Point3F p1, Point3F p2)
        {
            if (ReferenceEquals(p1, null) && ! ReferenceEquals(p2, null) || (! ReferenceEquals(p1, null) && ReferenceEquals(p2, null)))
            {
                return false;
            }

            if (ReferenceEquals(p1, p2))
            {
                return true;
            }
            
            return Math.Abs(p1.X - p2.X) < Epsilon && Math.Abs(p1.Y - p2.Y) < Epsilon && Math.Abs(p1.Z - p2.Z) < Epsilon;
        }

        public static bool operator !=(Point3F p1, Point3F p2) => !(p1 == p2);

        public static Point3F operator +(Point3F p, Vector3F v) => new Point3F(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        public static Point3F operator -(Point3F p, Vector3F v) => new Point3F(p.X - v.X, p.Y - v.Y, p.Z - v.Z);

        public static Point3F operator *(Point3F p, float f) => new(p.X *f, p.Y *f, p.Z *f);
        public static Point3F operator *(float f, Point3F p) => new(p.X *f, p.Y *f, p.Z *f);

        public static Point3F operator /(Point3F p, float f) => new(p.X /f, p.Y /f, p.Z /f);
     
        public static float Distance(Point3F p1, Point3F p2) => (p1 - p2).Length;
        public float Distance(Point3F p) => Distance(this, p);
        public static float DistanceSquared(Point3F p1, Point3F p2) => (p1 - p2).LengthSquared;
        public float DistanceSquared(Point3F p) => DistanceSquared(this, p);
        
        public static Point3F Lerp(float t, Point3F p0, Point3F p1) => (1 - t) * p0 + t * p1;

        public static Point3F Min(Point3F p0, Point3F p1) => new Point3F(MathF.Min(p0.X, p1.X), MathF.Min(p0.Y, p1.Y), MathF.Min(p0.Z, p1.Z));
        public static Point3F Max(Point3F p0, Point3F p1) => new Point3F(MathF.Max(p0.X, p1.X), MathF.Max(p0.Y, p1.Y), MathF.Max(p0.Z, p1.Z));

        public Point3F Ceil() => new Point3F(MathF.Ceiling(X), MathF.Ceiling(Y), MathF.Ceiling(Z));
        public Point3F Floor() => new Point3F(MathF.Floor(X), MathF.Floor(Y), MathF.Floor(Z));
        public Point3F Abs() => new Point3F(MathF.Abs(X), MathF.Abs(Y), MathF.Abs(Z));
        
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
        
        private bool Equals(Point3F other)
        {
            return this == other;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point3F)obj);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"X[{X}] Y[{Y}] Z[{Z}]";
    }
}