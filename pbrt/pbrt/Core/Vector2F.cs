﻿using System;

namespace pbrt.Core
{
    public class Vector2F
    {
        public float X { get; }
        public float Y { get; }
        private const double Epsilon = 1e-9;

        public Vector2F(float x, float y)
        {
            X = x;
            Y = y;
        }

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

        public static Vector2F operator +(Vector2F v1, Vector2F v2) => new(v1.X + v2.X, v1.Y + v2.Y); 
        public static Vector2F operator -(Vector2F v1, Vector2F v2) => new(v1.X - v2.X, v1.Y - v2.Y);
        public static Vector2F operator *(Vector2F v, float f) => new(v.X *f, v.Y *f);
        public static Vector2F operator /(Vector2F v, float f) => new(v.X /f, v.Y /f);
        public static Vector2F operator -(Vector2F v) => new(-v.X , -v.Y);

        public float LengthSquared => X*X + Y*Y;
        public float Length => MathF.Sqrt(LengthSquared);

        public Vector2F Abs()
        {
            return new Vector2F(MathF.Abs(X), MathF.Abs(Y));
        }

        public static float Dot(Vector2F v1, Vector2F v2) => v1.X * v2.X + v1.Y * v2.Y;
        public static float AbsDot(Vector2F v1, Vector2F v2) => MathF.Abs(Dot(v1,v2));

        public float Dot(Vector2F v) => Dot(this, v);
        public float AbsDot(Vector2F v) => MathF.Abs(Dot(v));

        public static Vector2F Normalize(Vector2F v) => v / v.Length;
        public Vector2F Normalized() => Normalize(this);

        public static bool operator ==(Vector2F v1, Vector2F v2)
        {
            if ((v1 == null && v2 != null) || (v1 != null && v2 == null))
            {
                return false;
            }

            if (v1 == null)
            {
                return true;
            }

            return Math.Abs(v1.X - v2.X) < Epsilon && Math.Abs(v1.Y - v2.Y) < Epsilon;
        }

        public static bool operator !=(Vector2F v1, Vector2F v2)
        {
            return !(v1 == v2);
        }
        protected bool Equals(Vector2F other)
        {
            return Math.Abs(X - other.X) < Epsilon && Math.Abs(Y - other.Y) < Epsilon;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Vector2F)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }
}