using System;

namespace pbrt.Core
{
    public class Point2F
    {
        public float X { get; } 
        public float Y { get; } 

        public Point2F() : this(0, 0)
        {
            
        }

        public Point2F(Point2F p) : this(p.X, p.Y)
        {
            
        } 
        
        public Point2F(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point2I(Point2F p) => new Point2I((int)p.X, (int)p.Y);
        
        public static Point2F operator +(Point2F p1, Point2F p2) => new Point2F(p1.X + p2.X, p1.Y + p2.Y);
        public static Vector2F operator -(Point2F p1, Point2F p2) => new Vector2F(p1.X - p2.X, p1.Y - p2.Y);
        public static Vector2F operator -(Point2F p) => new Vector2F(- p.X, -p.Y);
        
        public static bool operator == (Point2F p1, Point2F p2)
        {
            if (ReferenceEquals(p1, null) && ! ReferenceEquals(p2, null) || (! ReferenceEquals(p1, null) && ReferenceEquals(p2, null)))
            {
                return false;
            }

            if (ReferenceEquals(p1, p2))
            {
                return true;
            }
            
            return MathF.Abs(p1.X - p2.X) < Epsilon && Math.Abs(p1.Y - p2.Y) < Epsilon;
        }

        public static double Epsilon => 1e-6;

        public static bool operator !=(Point2F p1, Point2F p2) => !(p1 == p2);

        public static Point2F operator +(Point2F p, Vector2F v) => new Point2F(p.X + v.X, p.Y + v.Y);
        public static Point2F operator -(Point2F p, Vector2F v) => new Point2F(p.X - v.X, p.Y - v.Y);

        public static Point2F operator *(Point2F p, float f) => new(p.X *f, p.Y *f);
        public static Point2F operator *(float f, Point2F p) => new(p.X *f, p.Y *f);

        public static Point2F operator /(Point2F p, float f) => new(p.X /f, p.Y /f);
     
        public static float Distance(Point2F p1, Point2F p2) => (p1 - p2).Length;
        public static float DistanceSquared(Point2F p1, Point2F p2) => (p1 - p2).LengthSquared;
        public float Distance(Point2F p) => Distance(this, p);
        public float DistanceSquared(Point2F p) => DistanceSquared(this, p);
        
        public static Point2F Lerp(float t, Point2F p0, Point2F p1) => (1 - t) * p0 + t * p1;

        public static Point2F Min(Point2F p0, Point2F p1) => new Point2F(MathF.Min(p0.X, p1.X), MathF.Min(p0.Y, p1.Y));
        public static Point2F Max(Point2F p0, Point2F p1) => new Point2F(MathF.Max(p0.X, p1.X), MathF.Max(p0.Y, p1.Y));

        public Point2F Ceil() => new Point2F(MathF.Ceiling(X), MathF.Ceiling(Y));
        public Point2F Floor() => new Point2F(MathF.Floor(X), MathF.Floor(Y));
        public Point2F Abs() => new Point2F(MathF.Abs(X), MathF.Abs(Y));
        
        public float this[int i] {
            get
            {
                return i switch
                {
                    0 => X,
                    1 => Y,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }
        
        private bool Equals(Point2F other)
        {
            return this == other;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point2F)obj);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}