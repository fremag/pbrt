using System;

namespace pbrt.Core
{
    public class Point3I
    {
        public int X { get; } 
        public int Y { get; } 
        public int Z { get; }

        public Point3I() : this(0, 0, 0)
        {
            
        }

        public Point3I(Point3I p) : this(p.X, p.Y, p.Z)
        {
            
        } 
        
        public Point3I(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static explicit operator Point2F(Point3I p) => new Point2F(p.X, p.Y);
        public static explicit operator Point3F(Point3I p) => new Point3F(p.X, p.Y, p.Z);
        
        public static Point3I operator +(Point3I p1, Point3I p2) => new Point3I(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);
        public static Vector3I operator -(Point3I p1, Point3I p2) => new Vector3I(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);
        public static Vector3I operator -(Point3I p) => new Vector3I(- p.X, -p.Y, -p.Z);
        
        public static bool operator == (Point3I p1, Point3I p2)
        {
            if (ReferenceEquals(p1, null) && ! ReferenceEquals(p2, null) || (! ReferenceEquals(p1, null) && ReferenceEquals(p2, null)))
            {
                return false;
            }

            if (ReferenceEquals(p1, p2))
            {
                return true;
            }
            
            return p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;
        }

        public static bool operator !=(Point3I p1, Point3I p2) => !(p1 == p2);

        public static Point3I operator +(Point3I p, Vector3I v) => new Point3I(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
        public static Point3I operator -(Point3I p, Vector3I v) => new Point3I(p.X - v.X, p.Y - v.Y, p.Z - v.Z);

        public static Point3I operator *(Point3I p, int i) => new(p.X *i, p.Y *i, p.Z *i);
        public static Point3I operator *(int f, Point3I p) => new(p.X *f, p.Y *f, p.Z *f);

        public static Point3F operator *(Point3I p, float f) => new(p.X *f, p.Y *f, p.Z *f);
        public static Point3F operator *(float f, Point3I p) => new(p.X *f, p.Y *f, p.Z *f);

        public static Point3I operator /(Point3I p, int f) => new(p.X /f, p.Y /f, p.Z /f);
        public static Point3I operator /(int f, Point3I p) => new(p.X /f, p.Y /f, p.Z /f);
     
        public static float Distance(Point3I p1, Point3I p2) => (p1 - p2).Length;
        public static float DistanceSquared(Point3I p1, Point3I p2) => (p1 - p2).LengthSquared;
        
        public static Point3F Lerp(float t, Point3I p0, Point3I p1) => (1 - t) * p0 + t * p1;

        public static Point3I Min(Point3I p0, Point3I p1) => new Point3I(Math.Min(p0.X, p1.X), Math.Min(p0.Y, p1.Y), Math.Min(p0.Z, p1.Z));
        public static Point3I Max(Point3I p0, Point3I p1) => new Point3I(Math.Max(p0.X, p1.X), Math.Max(p0.Y, p1.Y), Math.Max(p0.Z, p1.Z));

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
        
        private bool Equals(Point3I other)
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
    }
}