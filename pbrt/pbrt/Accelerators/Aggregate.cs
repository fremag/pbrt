using System;
using pbrt.Core;

namespace pbrt.Accelerators
{
    public abstract class Aggregate : IPrimitive
    {
        public abstract Bounds3F WorldBound();
        public abstract bool Intersect(Ray r, out SurfaceInteraction isect);
        public abstract bool IntersectP(Ray r);
        
        public AreaLight GetAreaLight()
        {
            throw new NotImplementedException("method called; should have gone to GeometricPrimitive");
        }

        public IMaterial GetMaterial()
        {
            throw new NotImplementedException("method called; should have gone to GeometricPrimitive");
        }

        public void ComputeScatteringFunctions(SurfaceInteraction isect, MemoryArena arena, TransportMode mode, bool allowMultipleLobes)
        {
            throw new NotImplementedException("method called; should have gone to GeometricPrimitive");
        }
    }
}