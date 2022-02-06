namespace pbrt.Core
{
    public class RayDifferential : Ray
    {
        public Point3F RxOrigin { get; private set;}
        public Point3F RyOrigin { get; private set;} 
        public Vector3F RxDirection { get; private set; }
        public Vector3F RyDirection { get; private set; }
        public bool HasDifferentials { get; set; }

        public RayDifferential(Point3F origin, Vector3F direction, float tMax = float.PositiveInfinity, float time = 0.0f, Medium medium=null)
            : base(origin, direction, tMax, time, medium)
        {
            // The constructor sets HasDifferentials to false because the neighboring rays if any, are not known
            HasDifferentials = false;
        }

        public void ScaleDifferentials(float spacing)
        {
            RxOrigin = O + (RxOrigin - O) * spacing;
            RyOrigin = O + (RyOrigin - O) * spacing;
            RxDirection = D + (RxDirection - D) * spacing;
            RyDirection = D + (RyDirection - D) * spacing;
        }
    }
}