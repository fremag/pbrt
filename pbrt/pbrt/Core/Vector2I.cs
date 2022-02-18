using System;

namespace pbrt.Core
{
    public class Vector2I
    {
        public int X { get; }
        public int Y { get; }

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2I(Vector2I v) : this(v.X, v.Y)
        { }

        public Vector2I(Point2I p) : this(p.X, p.Y)
        { }
        
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

        public static Vector2I operator +(Vector2I v1, Vector2I v2) => new(v1.X + v2.X, v1.Y + v2.Y); 
        public static Vector2I operator -(Vector2I v1, Vector2I v2) => new(v1.X - v2.X, v1.Y - v2.Y);
        public static Vector2I operator *(Vector2I v, int f) => new(v.X *f, v.Y *f);
        public static Vector2I operator *(int f, Vector2I v) => new(v.X *f, v.Y *f);
        public static Vector2I operator /(Vector2I v, int f) => new(v.X /f, v.Y /f);
        public static Vector2I operator -(Vector2I v) => new(-v.X , -v.Y);

        public int LengthSquared => X*X + Y*Y;
        public float Length => MathF.Sqrt(LengthSquared);

        public Vector2I Abs()
        {
            return new Vector2I(Math.Abs(X), Math.Abs(Y));
        }

        public static int Dot(Vector2I v1, Vector2I v2) => v1.X * v2.X + v1.Y * v2.Y;
        public static int AbsDot(Vector2I v1, Vector2I v2) => Math.Abs(Dot(v1,v2));

        public int Dot(Vector2I v) => Dot(this, v);
        public int AbsDot(Vector2I v) => AbsDot(this, v);

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            if (ReferenceEquals(v1, null) && ! ReferenceEquals(v2, null) || (! ReferenceEquals(v1, null) && ReferenceEquals(v2, null)))
            {
                return false;
            }

            if (ReferenceEquals(v1, v2))
            {
                return true;
            }

            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return !(v1 == v2);
        }
        
        protected bool Equals(Vector2I other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector2I)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}