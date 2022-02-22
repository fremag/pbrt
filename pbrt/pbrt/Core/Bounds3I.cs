using System;

namespace pbrt.Core
{
    public class Bounds3I
    {
        public Point3I PMin { get; private set; }
        public Point3I PMax { get; private set; }
        
        public Bounds3I()
        {
            int minNum = int.MinValue;
            int maxNum = int.MaxValue;
            PMin = new Point3I(maxNum, maxNum, maxNum);
            PMax = new Point3I(minNum, minNum, minNum );
       }

        public Bounds3I(Point3I p)
        {
            PMin = new Point3I(p);
            PMax = new Point3I(p);
        }
        
        public Bounds3I(Point3I p1, Point3I p2)
        {
            PMin = new Point3I(Math.Min(p1.X, p2.X), Math.Min(p1.Y, p2.Y), Math.Min(p1.Z, p2.Z));
            PMax = new Point3I(Math.Max(p1.X, p2.X), Math.Max(p1.Y, p2.Y), Math.Max(p1.Z, p2.Z));
        }

        public Point3I this[int i] {
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
        
        public Point3I Corner(int corner) => new Point3I(this[corner & 1].X, this[(corner & 2) == 0 ? 1 : 0].Y, this[(corner & 4) == 0 ? 1 : 0].Z);

        public Bounds3I Union(Bounds3I bounds) => Union(this, bounds);
        public static Bounds3I Union(Bounds3I bounds1, Bounds3I bounds2) {
            return new Bounds3I(new Point3I(Math.Min(bounds1.PMin.X, bounds2.PMin.X),
                    Math.Min(bounds1.PMin.Y, bounds2.PMin.Y),
                    Math.Min(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3I(Math.Max(bounds1.PMax.X, bounds2.PMax.X),
                    Math.Max(bounds1.PMax.Y, bounds2.PMax.Y),
                    Math.Max(bounds1.PMax.Z, bounds2.PMax.Z)));
        }

        public Bounds3I Union(Point3I p) {
            return new Bounds3I(new Point3I(Math.Min(PMin.X, p.X),
                    Math.Min(PMin.Y, p.Y),
                    Math.Min(PMin.Z, p.Z)),
                new Point3I(Math.Max(PMax.X, p.X),
                    Math.Max(PMax.Y, p.Y),
                    Math.Max(PMax.Z, p.Z)));
        }        

        public Bounds3I Intersect(Bounds3I bounds) => Intersect(this, bounds);
        public static Bounds3I Intersect( Bounds3I bounds1,  Bounds3I bounds2) {
            return new Bounds3I(new Point3I(Math.Max(bounds1.PMin.X, bounds2.PMin.X),
                    Math.Max(bounds1.PMin.Y, bounds2.PMin.Y),
                    Math.Max(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3I(Math.Min(bounds1.PMax.X, bounds2.PMax.X),
                    Math.Min(bounds1.PMax.Y, bounds2.PMax.Y),
                    Math.Min(bounds1.PMax.Z, bounds2.PMax.Z)));
        }

        public bool Overlaps(Bounds3I bounds) => Overlaps(this, bounds);
        public static bool Overlaps(Bounds3I b1, Bounds3I b2) {
            bool x = (b1.PMax.X >= b2.PMin.X) && (b1.PMin.X <= b2.PMax.X);
            bool y = (b1.PMax.Y >= b2.PMin.Y) && (b1.PMin.Y <= b2.PMax.Y);
            bool z = (b1.PMax.Z >= b2.PMin.Z) && (b1.PMin.Z <= b2.PMax.Z);
            return (x && y && z);
        }

        public bool Inside(Point3I p) => Inside(p, this); 
        public static bool Inside(Point3I p, Bounds3I b) {
            return (p.X >= b.PMin.X && p.X <= b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y <= b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z <= b.PMax.Z);
        }

        public bool InsideExclusive(Point3I p) => InsideExclusive(p, this);
        public static bool InsideExclusive(Point3I p, Bounds3I b) {
            return (p.X >= b.PMin.X && p.X < b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y < b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z < b.PMax.Z);
        }

        public Bounds3I Expand(int delta) => Expand(this, delta);
        public static Bounds3I Expand(Bounds3I b, int delta) {
            return new Bounds3I(b.PMin - new Vector3I(delta, delta, delta),
                b.PMax + new Vector3I(delta, delta, delta));
        }

        public Vector3I Diagonal() => PMax - PMin;
        
        public int SurfaceArea {
            get
            {
                Vector3I d = Diagonal();
                return 2 * (d.X * d.Y + d.X * d.Z + d.Y * d.Z);
            }
        }
        
        public float Volume {
            get
            {
                Vector3I d = Diagonal();
                return d.X * d.Y * d.Z;
            }
        }

        public int MaximumExtent
        {
            get
            {
                var d = Diagonal();
                if (d.X > d.Y && d.X > d.Z)
                    return 0;
                if (d.Y > d.Z)
                    return 1;
                return 2;
            }
        }

        private static float Lerp(float t, float x1, float x2) => (1 - t) * x1 + t * x2;
        
        public Point3I Lerp(Point3F t) =>  new Point3I((int)Lerp(t.X, PMin.X, PMax.X), (int)Lerp(t.Y, PMin.Y, PMax.Y), (int)Lerp(t.Z, PMin.Z, PMax.Z));
        
        public Vector3I Offset(Point3I p) 
        {
            var oX = p.X - PMin.X;
            var oY = p.Y - PMin.Y;
            var oZ = p.Z - PMin.Z;
            
            if (PMax.X > PMin.X) oX /= PMax.X - PMin.X;
            if (PMax.Y > PMin.Y) oY /= PMax.Y - PMin.Y;
            if (PMax.Z > PMin.Z) oZ /= PMax.Z - PMin.Z;
            
            return new Vector3I(oX, oY, oZ);
        }
        
        public void BoundingSphere(out Point3I center, out float radius) 
        {
            center = (PMin + PMax) / 2;
            radius = Inside(center, this) ? Point3I.Distance(center, PMax) : 0;
        }        
    }
}