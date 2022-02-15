using pbrt.Shapes;

namespace pbrt.Core
{
    public class GeometricPrimitive : IPrimitive 
    {
        public IShape Shape { get; set; }
        public IMaterial Material { get; set; }
        public AreaLight AreaLight { get; set; }
        public MediumInterface MediumInterface { get; set; }   
        
        public bool Intersect(Ray r, out SurfaceInteraction isect) 
        {
            if (!Shape.Intersect(r, out var tHit, out isect))
            {
                return false;
            }

            r.TMax = tHit;
            isect.Primitive = this;
            if (MediumInterface.IsMediumTransition())
            {
                isect.MediumInterface = MediumInterface;
            }
            else
            {
                isect.MediumInterface = new MediumInterface(r.Medium);
            }

            return true;
        }

        public Bounds3F WorldBound() => Shape.WorldBound();
        public bool IntersectP(Ray r) => Shape.IntersectP(r);
        public AreaLight GetAreaLight() => AreaLight;
        public IMaterial GetMaterial() => Material;

        public void ComputeScatteringFunctions(SurfaceInteraction isect, MemoryArena arena, TransportMode mode, bool allowMultipleLobes)
        {
            if (Material != null)
            {
                Material.ComputeScatteringFunctions(isect, arena, mode, allowMultipleLobes);
            } 
        }
    }
}