using pbrt.Core;

namespace pbrt.Accelerators
{
    public class BVHBuildNode 
    {
        public Bounds3F Bounds { get; protected set; }
        public BVHBuildNode[] Children { get; protected set;}
        public int SplitAxis { get; protected set;}
        public int FirstPrimOffset{ get; protected set;}
        public int NPrimitives{ get; protected set;}

        public BVHBuildNode()
        {
            Children = new BVHBuildNode[2];
        }

        public void InitLeaf(int first, int n, Bounds3F b)
        {
            FirstPrimOffset = first;
            NPrimitives = n;
            Bounds = b;
        }
        
        public void InitInterior(int axis, BVHBuildNode c0, BVHBuildNode c1) 
        {
            Children[0] = c0;
            Children[1] = c1;
            Bounds = c0.Bounds.Union(c1.Bounds);
            SplitAxis = axis;
            NPrimitives = 0;
        }        
    }
}