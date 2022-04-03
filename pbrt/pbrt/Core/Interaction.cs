using pbrt.Media;

namespace pbrt.Core
{
    public class Interaction
    {
        public const float ShadowEpsilon = 0.0001f;

        public Point3F P { get; set; }
        public float Time { get; set; }
        public Vector3F PError { get; set; }
        public Vector3F Wo { get; set; }
        public Normal3F N { get; set; }
        public MediumInterface MediumInterface { get; set; }
        
        public Interaction() {}

        public Interaction(Point3F p, Vector3F wo, float time, MediumInterface mediumInterface)
            : this(p, null, null, wo, time, mediumInterface)
        {
            
        }

        public Interaction(Point3F p, float time, MediumInterface mediumInterface) : this(p, null, time, mediumInterface)
        {
            
        }        
        public Interaction(Point3F p, Normal3F n, Vector3F pError, Vector3F wo, float time,  MediumInterface mediumInterface)
        {
            P = p;
            Time = time;
            PError = pError;
            Wo = wo;
            N = n;
            MediumInterface = mediumInterface;
        }

        public bool IsSurfaceInteraction() => N.X != 0 || N.Y != 0 || N.Z != 0;
        public bool IsMediumInteraction() => !IsSurfaceInteraction();

        public Medium GetMedium(Vector3F w) => Vector3F.Dot(w, N) > 0 ? MediumInterface.Outside : MediumInterface.Inside;
        public Medium GetMedium() => MediumInterface.Inside;

        // https://github.com/mmp/pbrt-v3/blob/aaa552a4b9cbf9dccb71450f47b268e0ed6370e2/src/core/geometry.h#L1440
        public static Point3F OffsetRayOrigin(Point3F p, Vector3F pError, Normal3F n, Vector3F w)
        {
            float d = n.AbsDot(pError);
            float offsetX = d * n.X;
            float offsetY = d * n.Y;
            float offsetZ = d * n.Z;

            if (w.Dot(n) < 0)
            {
                offsetX = -offsetX;
                offsetY = -offsetY;
                offsetZ = -offsetZ;
            }

            float poX = p.X + offsetX;
            float poY = p.Y + offsetY;
            float poZ = p.Z + offsetZ;

            // Round offset point _po_ away from _p_
            float RoundOffset(float po, float offset)
            {
                if (offset > 0) return MathUtils.NextFloatUp(po);
                if (offset < 0) return MathUtils.NextFloatDown(po);
                return po;
            }

            poX = RoundOffset(poX, offsetX);
            poY = RoundOffset(poY, offsetY);
            poZ = RoundOffset(poZ, offsetZ);
            
            var po = new Point3F(poX, poY, poZ);
            return po;
        }
        
        public Ray SpawnRay(Vector3F d)
        {
            Point3F o = OffsetRayOrigin(P, PError, N, d);
            return new Ray(o, d, float.PositiveInfinity, Time, GetMedium(d));
        }

        public Ray SpawnRayTo(Point3F point) 
        {
            Point3F origin = OffsetRayOrigin(P, PError, N, point - P);
            Vector3F d = point - origin;
            return new Ray(origin, d, 1 - ShadowEpsilon, Time, GetMedium(d));
        }
        
        public Ray SpawnRayTo(Interaction interaction) 
        {
            var w = interaction.P - P;
            Point3F origin = OffsetRayOrigin(P, PError, N, w);
            var w2 = origin - interaction.P;
            Point3F target = OffsetRayOrigin(interaction.P, interaction.PError, interaction.N, w2);
            Vector3F direction = target - origin;
            return new Ray(origin, direction, 1-ShadowEpsilon, Time, GetMedium(direction));
        }
    }
}