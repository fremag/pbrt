using pbrt.Lights;

namespace pbrt.Core
{
    public interface IPrimitive
    {
        Bounds3F WorldBound();
        bool Intersect(Ray r, out SurfaceInteraction isect);
        bool IntersectP(Ray r);
        AreaLight GetAreaLight();
        IMaterial GetMaterial();
        void ComputeScatteringFunctions(SurfaceInteraction isect, MemoryArena arena, TransportMode mode, bool allowMultipleLobes);
    }
}