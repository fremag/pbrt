using System;

namespace pbrt.Core
{
    public class Bounds2I
    {
        public Point2I PMin { get; private set; }
        public Point2I PMax { get; private set; }
        
        public Bounds2I()
        {
            int minNum = int.MinValue;
            int maxNum = int.MaxValue;
            PMin = new Point2I(maxNum, maxNum);
            PMax = new Point2I(minNum, minNum);
       }

        public Bounds2I(Point2I p)
        {
            PMin = new Point2I(p);
            PMax = new Point2I(p);
        }
        
        public Bounds2I(Point2I p1, Point2I p2)
        {
            PMin = new Point2I(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y));
            PMax = new Point2I(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y));
        }

        public Point2I this[int i] {
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
        
        public Point2I Corner(int corner) => new Point2I(this[corner & 1].X, this[(corner & 2) == 0 ? 1 : 0].Y);

        public Bounds2I Union(Bounds2I bounds) => Union(this, bounds);
        public static Bounds2I Union(Bounds2I bounds1, Bounds2I bounds2) {
            return new Bounds2I(new Point2I(Math.Min(bounds1.PMin.X, bounds2.PMin.X),
                    Math.Min(bounds1.PMin.Y, bounds2.PMin.Y)),
                new Point2I(Math.Max(bounds1.PMax.X, bounds2.PMax.X),
                    Math.Max(bounds1.PMax.Y, bounds2.PMax.Y)));
        }

        public Bounds2I Intersect(Bounds2I bounds) => Intersect(this, bounds);
        public static Bounds2I Intersect( Bounds2I bounds1,  Bounds2I bounds2) {
            return new Bounds2I(new Point2I(Math.Max(bounds1.PMin.X, bounds2.PMin.X),
                    Math.Max(bounds1.PMin.Y, bounds2.PMin.Y)),
                new Point2I(Math.Min(bounds1.PMax.X, bounds2.PMax.X),
                    Math.Min(bounds1.PMax.Y, bounds2.PMax.Y)));
        }

        public bool Overlaps(Bounds2I bounds) => Overlaps(this, bounds);
        public static bool Overlaps(Bounds2I b1, Bounds2I b2) {
            bool x = (b1.PMax.X >= b2.PMin.X) && (b1.PMin.X <= b2.PMax.X);
            bool y = (b1.PMax.Y >= b2.PMin.Y) && (b1.PMin.Y <= b2.PMax.Y);
            return (x && y);
        }

        public bool Inside(Point2I p) => Inside(p, this);
        public static bool Inside(Point2I p, Bounds2I b) {
            return (p.X >= b.PMin.X && p.X <= b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y <= b.PMax.Y );
        }

        public bool InsideExclusive(Point2I p) => InsideExclusive(p, this);
        public static bool InsideExclusive(Point2I p, Bounds2I b) {
            return (p.X >= b.PMin.X && p.X < b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y < b.PMax.Y );
        }

        public Bounds2I Expand(int delta) => Expand(this, delta);
        public static Bounds2I Expand(Bounds2I b, int delta) {
            return new Bounds2I(b.PMin - new Vector2I(delta, delta),
                b.PMax + new Vector2I(delta, delta));
        }

        public Vector2I Diagonal() => PMax - PMin;
        
        public int SurfaceArea {
            get
            {
                Vector2I d = Diagonal();
                return d.X * d.Y;
            }
        }

        public int MaximumExtent
        {
            get
            {
                var d = Diagonal();
                if (d.X > d.Y)
                    return 0;
                return 1;
            }
        }

        private static float Lerp(float t, float x1, float x2) => (1 - t) * x1 + t * x2;
        
        public Point2I Lerp(Point2I t) =>  new Point2I((int)Lerp(t.X, PMin.X, PMax.X), (int)Lerp(t.Y, PMin.Y, PMax.Y));
        
        public Vector2I Offset(Point2I p) 
        {
            var oX = p.X - PMin.X;
            var oY = p.Y - PMin.Y;
            
            if (PMax.X > PMin.X) oX /= PMax.X - PMin.X;
            if (PMax.Y > PMin.Y) oY /= PMax.Y - PMin.Y;
            
            return new Vector2I(oX, oY);
        }        
    }
}