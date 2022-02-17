using System;
using System.Collections.Generic;
using System.Linq;
using pbrt.Core;

namespace pbrt.Accelerators
{
    public enum SplitMethod
    {
        Sah,
        Hlbvh,
        Middle,
        EqualCounts
    }

    public class BvhAccel
    {
        public int MaxPrimsInNode { get; }
        public SplitMethod SplitMethod { get; }
        public IPrimitive[] Primitives { get; }

        BvhAccel(IEnumerable<IPrimitive> primitives, int maxPrimsInNode, SplitMethod splitMethod)
        {
            MaxPrimsInNode = maxPrimsInNode;
            SplitMethod = splitMethod;
            if (primitives == null)
            {
                return;
            }

            Primitives = primitives.ToArray();

            List<BVHPrimitiveInfo> primitiveInfo = new List<BVHPrimitiveInfo>();
            for (var i = 0; i < Primitives.Length; i++)
            {
                var primitive = Primitives[i];
                primitiveInfo.Add(new BVHPrimitiveInfo(i, primitive.WorldBound()));
            }
            
            int totalNodes = 0;
            List<IPrimitive> orderedPrims = new List<IPrimitive>();
            BVHBuildNode root;
            if (splitMethod == SplitMethod.Hlbvh)
            {
                //root = HLBVHBuild(primitiveInfo, totalNodes, orderedPrims);
                throw new NotImplementedException();
            }
            else
            {
                root = RecursiveBuild(primitiveInfo, 0, Primitives.Count(), ref totalNodes, ref orderedPrims);
            }

            throw new NotImplementedException();
        }
        
        BVHBuildNode RecursiveBuild(List<BVHPrimitiveInfo> primitiveInfo, int start, int end, ref int totalNodes, ref List<IPrimitive> orderedPrims) 
        {
            BVHBuildNode node = new BVHBuildNode();
            totalNodes++;
            Bounds3F bounds = new Bounds3F();
            for (int i = start; i < end; ++i)
            {
                bounds = bounds.Union(primitiveInfo[i].Bounds);
            }

            int nPrimitives = end - start;
            if (nPrimitives == 1) {
                int firstPrimOffset = orderedPrims.Count;
                for (int i = start; i < end; ++i) 
                {
                    int primNum = primitiveInfo[i].PrimitiveNumber;
                    orderedPrims.Add(Primitives[primNum]);
                } 
                node.InitLeaf(firstPrimOffset, nPrimitives, bounds);
                return node;
            } 
            else 
            {
                Bounds3F centroidBounds = new Bounds3F();
                for (int i = start; i < end; ++i)
                {
                    centroidBounds = centroidBounds.Union(primitiveInfo[i].Centroid);
                }

                int dim = centroidBounds.MaximumExtent;
                
                int mid = (start + end) / 2;
                if (Math.Abs(centroidBounds.PMax[dim] - centroidBounds.PMin[dim]) < float.Epsilon) 
                {
                    // Create leaf _BVHBuildNode_
                    int firstPrimOffset = orderedPrims.Count;
                    for (int i = start; i < end; ++i) 
                    {
                        int primNum = primitiveInfo[i].PrimitiveNumber;
                        orderedPrims.Add(Primitives[primNum]);
                    }
                    node.InitLeaf(firstPrimOffset, nPrimitives, bounds);
                    return node; 
                }
                else
                {
                    switch (SplitMethod) {
                        case SplitMethod.Middle:
                            throw new NotImplementedException(); 
                        case SplitMethod.EqualCounts:
                        {
                            throw new NotImplementedException(); 
                        }
                        case SplitMethod.Sah:
                        default: {
                            throw new NotImplementedException(); 
                        }
                    }                    
                    // <<Partition primitives based on splitMethod>> 
                    var c0 = RecursiveBuild(primitiveInfo, start, mid, ref totalNodes, ref orderedPrims);
                    var c1 = RecursiveBuild(primitiveInfo, mid, end, ref totalNodes, ref orderedPrims);
                    node.InitInterior(dim, c0, c1);
                }                
            }
            return node;
        }        
    }
}