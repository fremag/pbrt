using pbrt.Core;

namespace pbrt.Accelerators
{
    public class BVHPrimitiveInfo 
    {
        public int PrimitiveNumber { get; }
        public Bounds3F Bounds { get; }
        public Point3F Centroid { get; }
        
        public BVHPrimitiveInfo(int primitiveNumber, Bounds3F bounds)
        {
            PrimitiveNumber = primitiveNumber;
            Bounds = bounds;
            Centroid = new Point3F(.5f * Bounds.PMin + .5f * Bounds.PMax);
        }
    }
}