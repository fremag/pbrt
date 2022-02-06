namespace pbrt.Core
{
    public class RayDifferential : Ray
    {
        public Point3F RxO, RyO;
        public Vector3F RxD, RyD;
        public bool HasDifferentials;

        public RayDifferential(Point3F origin, Vector3F direction, float tMax = float.PositiveInfinity, float time = 0.0f, Medium medium=null)
            : base(origin, direction, tMax, time, medium)
        {
            // initially set to false because the neighboring rays are not known
            HasDifferentials = false;
        }

        public void ScaleDifferentials(float spacing)
        {
            RxO = O + (RxO - O) * spacing;
            RxD = D + (RxD - D) * spacing;
            RyO = O + (RyO - O) * spacing;
            RyD = D + (RyD - D) * spacing;
        }
    }
}