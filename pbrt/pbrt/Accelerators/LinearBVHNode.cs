using pbrt.Core;

namespace pbrt.Accelerators
{
    public class LinearBVHNode 
    {
        public Bounds3F Bounds { get; set; }
        public int PrimitivesOffset { get; set; }    // leaf
        public int SecondChildOffset { get; set; }   // interior
        public int NPrimitives { get; set; }  // 0 -> interior node
        public int Axis { get; set; }          // interior node: xyz
    };
}