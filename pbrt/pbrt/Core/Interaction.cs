namespace pbrt.Core
{
    public class Interaction
    {
        const float ShadowEpsilon = 0.0001f;

        public Point3F P { get; set; }
        public float Time { get; set; }
        public Vector3F PError { get; set; }
        public Vector3F Wo { get; set; }
        public Normal3F N { get; set; }
        public MediumInterface MediumInterface { get; set; }
        
        public Interaction() {}
        
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
        bool IsMediumInteraction() => !IsSurfaceInteraction();

        public Medium GetMedium(Vector3F w) => Vector3F.Dot(w, N) > 0 ? MediumInterface.Outside : MediumInterface.Inside;
        Medium GetMedium() => MediumInterface.Inside;

        Point3F OffsetRayOrigin(Point3F p, Vector3F pError, Normal3F n, Vector3F w)
        {
            float d = n.AbsDot(pError);
            Vector3F offset = new Vector3F(d * n.X, d * n.Y, d * n.Z);
            if (w.Dot(n) < 0)
                offset = - offset;
            Point3F po = p + offset;
            return po;
        }
        
        public Ray SpawnRay(Vector3F d)
        {
            Point3F o = OffsetRayOrigin(P, PError, N, d);
            return new Ray(o, d, float.PositiveInfinity, Time, GetMedium(d));
        }

        Ray SpawnRayTo(Point3F point) 
        {
            Point3F origin = OffsetRayOrigin(P, PError, N, point - P);
            Vector3F d = point - origin;
            return new Ray(origin, d, 1 - ShadowEpsilon, Time, GetMedium(d));
        }
        
        Ray SpawnRayTo(Interaction interaction) 
        {
            Point3F origin = OffsetRayOrigin(P, PError, N, interaction.P - P);
            Point3F target = OffsetRayOrigin(interaction.P, interaction.PError, interaction.N, origin - interaction.P);
            Vector3F direction = target - origin;
            return new Ray(origin, direction, 1-ShadowEpsilon, Time, GetMedium(direction));
        }
    }
}