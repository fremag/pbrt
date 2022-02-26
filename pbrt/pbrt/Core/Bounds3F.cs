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
        
        public static Bounds3F Union(Bounds3F bounds1, Bounds3F bounds2) {
            return new Bounds3F(new Point3F(MathF.Min(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Min(bounds1.PMin.Y, bounds2.PMin.Y),
                    MathF.Min(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3F(MathF.Max(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Max(bounds1.PMax.Y, bounds2.PMax.Y),
                    MathF.Max(bounds1.PMax.Z, bounds2.PMax.Z)));
        }

        public Bounds3F Union(Bounds3F bounds) => Union(this, bounds);

        public Bounds3F Intersect(Bounds3F bounds) => Intersect(this, bounds);
        public static Bounds3F Intersect( Bounds3F bounds1,  Bounds3F bounds2) {
            return new Bounds3F(new Point3F(MathF.Max(bounds1.PMin.X, bounds2.PMin.X),
                    MathF.Max(bounds1.PMin.Y, bounds2.PMin.Y),
                    MathF.Max(bounds1.PMin.Z, bounds2.PMin.Z)),
                new Point3F(MathF.Min(bounds1.PMax.X, bounds2.PMax.X),
                    MathF.Min(bounds1.PMax.Y, bounds2.PMax.Y),
                    MathF.Min(bounds1.PMax.Z, bounds2.PMax.Z)));
        }

        public bool Overlaps(Bounds3F b) => Overlaps(this, b);
        public static bool Overlaps(Bounds3F b1, Bounds3F b2) {
            bool x = (b1.PMax.X >= b2.PMin.X) && (b1.PMin.X <= b2.PMax.X);
            bool y = (b1.PMax.Y >= b2.PMin.Y) && (b1.PMin.Y <= b2.PMax.Y);
            bool z = (b1.PMax.Z >= b2.PMin.Z) && (b1.PMin.Z <= b2.PMax.Z);
            return (x && y && z);
        }        
        
        public static bool Inside(Point3F p, Bounds3F b) {
            return (p.X >= b.PMin.X && p.X <= b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y <= b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z <= b.PMax.Z);
        }

        public bool Inside(Point3F p) => Inside(p, this);

        public bool InsideExclusive(Point3F p) => InsideExclusive(p, this);
        
        public static bool InsideExclusive(Point3F p, Bounds3F b) {
            return (p.X >= b.PMin.X && p.X < b.PMax.X &&
                    p.Y >= b.PMin.Y && p.Y < b.PMax.Y &&
                    p.Z >= b.PMin.Z && p.Z < b.PMax.Z);
        }
        
        public Bounds3F Expand(float delta) => new Bounds3F(PMin - new Vector3F(delta, delta, delta), PMax + new Vector3F(delta, delta, delta));

        public Vector3F Diagonal() => PMax - PMin;
        
        public float SurfaceArea {
            get
            {
                Vector3F d = Diagonal();
                return 2f * (d.X * d.Y + d.X * d.Z + d.Y * d.Z);
            }
        }
        
        public float Volume {
            get
            {
                Vector3F d = Diagonal();
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
        
        public bool IntersectP(Ray ray, out float hitT0, out float hitT1) 
        {
            hitT0 = 0;
            hitT1 = ray.TMax;
                
            var gammaFactor = 1 + 2 * MathUtils.Gamma(3);

            for (int i = 0; i < 3; ++i) 
            {
                float invRayDir = 1 / ray.D[i];
                var pMin = PMin[i];
                var pMax = PMax[i];
                var o = ray.O[i];

                var near = pMin - o;
                var far = pMax - o;

                float tNear = near * invRayDir;
                float tFar  = far * invRayDir;

                if (tNear > tFar)
                {
                    (tFar, tNear) = (tNear, tFar);
                }

                tFar *= gammaFactor;
                hitT0 = tNear > hitT0 ? tNear : hitT0;
                hitT1 = tFar  < hitT1 ? tFar  : hitT1;
                
                if (hitT0 > hitT1)
                {
                    return false;
                }
            }

            return true;
        }        
        
        public bool IntersectP(Ray ray, Vector3F invDir, int[] dirIsNeg)
        {
            Bounds3F bounds = this;
            // Check for ray intersection against $x$ and $y$ slabs
            float tMin = (bounds[dirIsNeg[0]].X - ray.O.X) * invDir.X;
            float tMax = (bounds[1 - dirIsNeg[0]].X - ray.O.X) * invDir.X;
            float tyMin = (bounds[dirIsNeg[1]].Y - ray.O.Y) * invDir.Y;
            float tyMax = (bounds[1 - dirIsNeg[1]].Y - ray.O.Y) * invDir.Y;

            // Update _tMax_ and _tyMax_ to ensure robust bounds intersection
            var errGamma = 1 + 2 * MathUtils.Gamma(3);
            tMax *= errGamma;
            tyMax *= errGamma;
            if (tMin > tyMax || tyMin > tMax) return false;
            if (tyMin > tMin) tMin = tyMin;
            if (tyMax < tMax) tMax = tyMax;

            // Check for ray intersection against $z$ slab
            float tzMin = (bounds[dirIsNeg[2]].Z - ray.O.Z) * invDir.Z;
            float tzMax = (bounds[1 - dirIsNeg[2]].Z - ray.O.Z) * invDir.Z;

            // Update _tzMax_ to ensure robust bounds intersection
            tzMax *= errGamma;
            if (tMin > tzMax || tzMin > tMax) return false;
            if (tzMin > tMin) tMin = tzMin;
            if (tzMax < tMax) tMax = tzMax;
            return (tMin < ray.TMax) && (tMax > 0);
        }

        public override string ToString() => $"Min[{PMin.X}, {PMin.Y}, {PMin.Z}] Max[{PMax.X}, {PMax.Y}, {PMax.Z}]";
    }
}