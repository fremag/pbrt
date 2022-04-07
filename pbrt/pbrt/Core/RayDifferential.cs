using pbrt.Media;

namespace pbrt.Core
{
    public class RayDifferential : Ray
    {
        public Point3F RxOrigin { get; set;}
        public Point3F RyOrigin { get;  set;} 
        public Vector3F RxDirection { get; set; }
        public Vector3F RyDirection { get; set; }
        public bool HasDifferentials { get; set; }

        public RayDifferential(Point3F origin, Vector3F direction, float tMax = float.PositiveInfinity, float time = 0.0f, Medium medium=null)
            : base(origin, direction, tMax, time, medium)
        {
            RxOrigin = Point3F.Zero;
            RyOrigin = Point3F.Zero;
            RxDirection = Vector3F.Zero;
            RyDirection = Vector3F.Zero;
            // The constructor sets HasDifferentials to false because the neighboring rays if any, are not known
            HasDifferentials = false;
        }

        public RayDifferential(Ray ray) : this(ray.O, ray.D, ray.TMax, ray.Time, ray.Medium)
        {
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