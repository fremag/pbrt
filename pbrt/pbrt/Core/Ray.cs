using pbrt.Media;

namespace pbrt.Core
{
    public class Ray
    {
        public Point3F O { get; set; }
        public Vector3F D { get; set; }
        public float TMax { get; set; }
        public float Time { get; set; }
        public Medium Medium { get; set; }

        public Ray()
        {
            TMax = float.PositiveInfinity;
        }

        public Ray(Point3F o, Vector3F d, float max = float.PositiveInfinity, float time = 0f, Medium medium = null)
        {
            O = o;
            D = d;
            TMax = max;
            Time = time;
            Medium = medium;
        }

        public bool HasNaNs => O.HasNaNs || D.HasNaNs || double.IsNaN(TMax);

        // C++:  Point3f operator()(Float t) const { return o + d * t; }
        public Point3F At(float t)
        {
            return O + D * t;
        }
    }
}