using System;

namespace pbrt.Core
{
    public class Bounds3F
    {
        public Point3F PMin { get; private set; }
        public Point3F PMax { get; private set; }
        
        public Bounds3F()
        {
            float minNum = float.MinValue;
            float maxNum = float.MaxValue;
            PMin = new Point3F(maxNum, maxNum, maxNum);
            PMax = new Point3F(minNum, minNum, minNum );
       }

        public Bounds3F(Point3F p)
        {
            PMin = new Point3F(p);
            PMax = new Point3F(p);
        }
        
        public Bounds3F(Point3F p1, Point3F p2)
        {
            PMin = new Point3F(MathF.Min(p1.X, p2.X), MathF.Min(p1.Y, p2.Y), MathF.Min(p1.Z, p2.Z));
            PMax = new Point3F(MathF.Max(p1.X, p2.X), MathF.Max(p1.Y, p2.Y), MathF.Max(p1.Z, p2.Z));
        }

        public Point3F this[int i] {
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
        
        public Point3F Corner(int corner) => new Point3F(this[corner & 1].X, this[(corner & 2) == 0 ? 1 : 0].Y, this[(corner & 4) == 0 ? 1 : 0].Z);

        public Bounds3F Union(Point3F p) {
            return new Bounds3F(new Point3F(MathF.Min(PMin.X, p.X),
                    MathF.Min(PMin.Y, p.Y),
                    MathF.Min(PMin.Z, p.Z)),
                new Point3F(MathF.Max(PMax.X, p.X),
                    MathF.Max(PMax.Y, p.Y),
                    MathF.Max(PMax.Z, p.Z)));
        }        
        public Bounds3F Union(Bounds3F bounds1, Bounds3F bounds2) {
            return new Bounds3F(new Point3F(MathF.Min(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Min(bounds1.PMin.Y, bounds2.PMin.Y),
                    MathF.Min(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3F(MathF.Max(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Max(bounds1.PMax.Y, bounds2.PMax.Y),
                    MathF.Max(bounds1.PMax.Z, bounds2.PMax.Z)));
        }
        
        public Bounds3F Intersect( Bounds3F bounds1,  Bounds3F bounds2) {
            return new Bounds3F(new Point3F(MathF.Max(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Max(bounds1.PMin.Y, bounds2.PMin.Y),
                    MathF.Max(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3F(MathF.Min(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Min(bounds1.PMax.Y, bounds2.PMax.Y),
                    MathF.Min(bounds1.PMax.Z, bounds2.PMax.Z)));
        }
        
        public static bool Overlaps(Bounds3F b1, Bounds3F b2) {
            bool x = (b1.PMax.X >= b2.PMin.X) && (b1.PMin.X <= b2.PMax.X);
            bool y = (b1.PMax.Y >= b2.PMin.Y) && (b1.PMin.Y <= b2.PMax.Y);
            bool z = (b1.PMax.Z >= b2.PMin.Z) && (b1.PMin.Z <= b2.PMax.Z);
            return (x && y && z);
        }        
        
        public bool Inside(Point3F p, Bounds3F b) {
            return (p.X >= b.PMin.X && p.X <= b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y <= b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z <= b.PMax.Z);
        }
        public bool InsideExclusive(Point3F p, Bounds3F b) {
            return (p.X >= b.PMin.X && p.X < b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y < b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z < b.PMax.Z);
        }
        
        public Bounds3F Expand(Bounds3F b, float delta) {
            return new Bounds3F(b.PMin - new Vector3F(delta, delta, delta),
                b.PMax + new Vector3F(delta, delta, delta));
        }

        public Vector3F Diagonal() => PMax - PMin;
        
        float SurfaceArea {
            get
            {
                Vector3F d = Diagonal();
                return 2 * (d.X * d.Y + d.X * d.Z + d.Y * d.Z);
            }
        }
        float Volume {
            get
            {
                Vector3F d = Diagonal();
                return d.X * d.Y * d.Z;
            }
        }

        int MaximumExtent
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
        
        public Point3F Lerp(Point3F t) =>  new Point3F(Lerp(t.X, PMin.X, PMax.X), Lerp(t.Y, PMin.Y, PMax.Y), Lerp(t.Z, PMin.Z, PMax.Z));
        
        public Vector3F Offset(Point3F p) 
        {
            var oX = p.X - PMin.X;
            var oY = p.Y - PMin.Y;
            var oZ = p.Z - PMin.Z;
            
            if (PMax.X > PMin.X) oX /= PMax.X - PMin.X;
            if (PMax.Y > PMin.Y) oY /= PMax.Y - PMin.Y;
            if (PMax.Z > PMin.Z) oZ /= PMax.Z - PMin.Z;
            
            return new Vector3F(oX, oY, oZ);
        }
        
        public void BoundingSphere(out Point3F center, out float radius) 
        {
            center = (PMin + PMax) / 2;
            radius = Inside(center, this) ? Point3F.Distance(center, PMax) : 0;
        }        
    }
}