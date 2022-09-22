using System;

namespace pbrt.Core
{
    public class Point2I
    {
        public static readonly Point2I Zero = new Point2I(0, 0);
        public int X { get; } 
        public int Y { get; } 

        public Point2I() : this(0, 0)
        {
            
        }

        public Point2I(Point2I p) : this(p.X, p.Y)
        {
            
        } 
        
        public Point2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static explicit operator Point2I(Point3I p) => new Point2I(p.X, p.Y);
        public static explicit operator Point2F(Point2I p) => new Point2F(p.X, p.Y);
        
        public static Point2I operator +(Point2I p1, Point2I p2) => new Point2I(p1.X + p2.X, p1.Y + p2.Y);
        public static Vector2I operator -(Point2I p1, Point2I p2) => new Vector2I(p1.X - p2.X, p1.Y - p2.Y);
        public static Vector2I operator -(Point2I p) => new Vector2I(- p.X, -p.Y);
        
        public static bool operator == (Point2I p1, Point2I p2)
        {
            if (ReferenceEquals(p1, null) && ! ReferenceEquals(p2, null) || (! ReferenceEquals(p1, null) && ReferenceEquals(p2, null)))
            {
                return false;
            }

            if (ReferenceEquals(p1, p2))
            {
                return true;
            }
            
            return p1.X == p2.X&&p1.Y ==p2.Y;
        }

        public static bool operator !=(Point2I p1, Point2I p2) => !(p1 == p2);

        public static Point2I operator +(Point2I p, Vector2I v) => new Point2I(p.X + v.X, p.Y + v.Y);
        public static Point2I operator -(Point2I p, Vector2I v) => new Point2I(p.X - v.X, p.Y - v.Y);

        public static Point2I operator *(Point2I p, int i) => new(p.X *i, p.Y *i);
        public static Point2I operator *(int i, Point2I p) => new(p.X *i, p.Y *i);
        
        public static Point2F operator *(Point2I p, float f) => new(p.X *f, p.Y *f);
        public static Point2F operator *(float f, Point2I p) => new(p.X *f, p.Y *f);

        public static Point2I operator /(Point2I p, int i) => new(p.X /i, p.Y /i);
     
        public static float Distance(Point2I p1, Point2I p2) => (p1 - p2).Length;
        public static float DistanceSquared(Point2I p1, Point2I p2) => (p1 - p2).LengthSquared;
        public float Distance(Point2I p) => Distance(this, p);
        public float DistanceSquared(Point2I p) => DistanceSquared(this, p);
        
        public static Point2F Lerp(float t, Point2I p0, Point2I p1) => (1 - t) * p0 + t * p1;

        public static Point2I Min(Point2I p0, Point2I p1) => new Point2I(Math.Min(p0.X, p1.X), Math.Min(p0.Y, p1.Y));
        public static Point2I Max(Point2I p0, Point2I p1) => new Point2I(Math.Max(p0.X, p1.X), Math.Max(p0.Y, p1.Y));

        public Point2I Abs() => new Point2I(Math.Abs(X), Math.Abs(Y));
        
        public int this[int i] {
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
        
        private bool Equals(Point2I other)
        {
            return this == other;
        }
        
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Point2I)obj);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}