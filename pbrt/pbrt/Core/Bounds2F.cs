using System;

namespace pbrt.Core
{
    public class Bounds2F
    {
        public Point2F PMin { get; private set; }
        public Point2F PMax { get; private set; }
        
        public Bounds2F()
        {
            float minNum = float.MinValue;
            float maxNum = float.MaxValue;
            PMin = new Point2F(maxNum, maxNum);
            PMax = new Point2F(minNum, minNum);
       }

        public Bounds2F(Point2F p)
        {
            PMin = new Point2F(p);
            PMax = new Point2F(p);
        }
        
        public Bounds2F(Point2F p1, Point2F p2)
        {
            PMin = new Point2F(MathF.Min(p1.X, p2.X), MathF.Min(p1.Y, p2.Y));
            PMax = new Point2F(MathF.Max(p1.X, p2.X), MathF.Max(p1.Y, p2.Y));
        }

        public Point2F this[int i] {
            get
            {
                return i switch
                {
                    0 => PMin,
                    1 => PMax,
                    _ => throw new IndexOutOfRangeException()
                };
            }
        }
        
        public Point2F Corner(int corner) => new Point2F(this[corner & 1].X, this[(corner & 2) == 0 ? 1 : 0].Y);

        public Bounds2F Union(Bounds2F bounds1, Bounds2F bounds2) {
            return new Bounds2F(new Point2F(MathF.Min(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Min(bounds1.PMin.Y, bounds2.PMin.Y)),
                new Point2F(MathF.Max(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Max(bounds1.PMax.Y, bounds2.PMax.Y)));
        }
        
        public Bounds2F Intersect( Bounds2F bounds1,  Bounds2F bounds2) {
            return new Bounds2F(new Point2F(MathF.Max(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Max(bounds1.PMin.Y, bounds2.PMin.Y)),
                new Point2F(MathF.Min(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Min(bounds1.PMax.Y, bounds2.PMax.Y)));
        }
        
        public static bool Overlaps(Bounds2F b1, Bounds2F b2) {
            bool x = (b1.PMax.X >= b2.PMin.X) && (b1.PMin.X <= b2.PMax.X);
            bool y = (b1.PMax.Y >= b2.PMin.Y) && (b1.PMin.Y <= b2.PMax.Y);
            return (x && y);
        }        
        
        public bool Inside(Point2F p, Bounds2F b) {
            return (p.X >= b.PMin.X && p.X <= b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y <= b.PMax.Y);
        }
        public bool InsideExclusive(Point2F p, Bounds2F b) {
            return (p.X >= b.PMin.X && p.X < b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y < b.PMax.Y );
        }
        
        public Bounds2F Expand(Bounds2F b, float delta) {
            return new Bounds2F(b.PMin - new Vector2F(delta, delta),
                b.PMax + new Vector2F(delta, delta));
        }

        public Vector2F Diagonal() => PMax - PMin;
        
        float SurfaceArea {
            get
            {
                Vector2F d = Diagonal();
                return 2 * (d.X * d.Y + d.X);
            }
        }
        float Volume {
            get
            {
                Vector2F d = Diagonal();
                return d.X * d.Y;
            }
        }

        int MaximumExtent
        {
            get
            {
                var d = Diagonal();
                if (d.X > d.Y )
                    return 0;
                return 1;
            }
        }

        private static float Lerp(float t, float x1, float x2) => (1 - t) * x1 + t * x2;
        
        public Point2F Lerp(Point2F t) =>  new Point2F(Lerp(t.X, PMin.X, PMax.X), Lerp(t.Y, PMin.Y, PMax.Y));
        
        public Vector2F Offset(Point2F p) 
        {
            var oX = p.X - PMin.X;
            var oY = p.Y - PMin.Y;
            
            if (PMax.X > PMin.X) oX /= PMax.X - PMin.X;
            if (PMax.Y > PMin.Y) oY /= PMax.Y - PMin.Y;

            return new Vector2F(oX, oY);
        }        
    }
}