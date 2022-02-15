namespace pbrt.Core
{
    public class Ray
    {
        public Point3F O { get; }
        public Vector3F D { get; }
        public float TMax { get; set; }
        public float Time { get; }
        public Medium Medium { get; }

        public Ray()
        {
            TMax = float.PositiveInfinity;
        }

        public Ray(Point3F o, Vector3F d, float max, float time, Medium medium)
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