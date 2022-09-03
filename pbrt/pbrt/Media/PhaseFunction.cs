using pbrt.Core;

namespace pbrt.Media
{
    public abstract class PhaseFunction
    {
        public abstract float P(Vector3F wo, Vector3F wi);
        public abstract float Sample_P(Vector3F wo, out Vector3F wi, Point2F u);
    }
}