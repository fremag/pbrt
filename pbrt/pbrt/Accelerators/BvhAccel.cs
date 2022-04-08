using System;
using System.Collections.Generic;
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

    public class BvhAccel : Aggregate
    {
        public int MaxPrimsInNode { get; }
        public SplitMethod SplitMethod { get; }
        public IPrimitive[] Primitives { get; }
        public LinearBVHNode[] Nodes { get; }
        public List<BVHPrimitiveInfo> PrimitiveInfos { get; }
        
        public BvhAccel(List<IPrimitive> primitives, int maxPrimsInNode, SplitMethod splitMethod)
        {
            MaxPrimsInNode = maxPrimsInNode;
            SplitMethod = splitMethod;
            if (primitives == null)
            {
                return;
            }

            Primitives = primitives.ToArray();
            
            PrimitiveInfos = new List<BVHPrimitiveInfo>();
            for (var i = 0; i < primitives.Count; i++)
            {
                var primitive = primitives[i];
                PrimitiveInfos.Add(new BVHPrimitiveInfo(i, primitive.WorldBound()));
            }
            
            int totalNodes = 0;
            List<IPrimitive> orderedPrims = new List<IPrimitive>();
            BVHBuildNode root;
            if (splitMethod == SplitMethod.Hlbvh)
            {
                HLBVHBuild(PrimitiveInfos, ref totalNodes, orderedPrims);
                return;
            }
            else
            {
                root = RecursiveBuild(PrimitiveInfos, 0, primitives.Count, ref totalNodes, orderedPrims);
            }
            Primitives = orderedPrims.ToArray();
            
            Nodes = new LinearBVHNode[totalNodes];
            for (var i = 0; i < totalNodes; ++i)
                Nodes[i] = new LinearBVHNode();

            int offset = 0;
            FlattenBvhTree(root, ref offset);
        }

        public int FlattenBvhTree(BVHBuildNode node, ref int offset)
        {
            LinearBVHNode linearNode = Nodes[offset];
            linearNode.Bounds = node.Bounds;
            int myOffset = offset++;
            if (node.NPrimitives > 0) 
            {
                linearNode.PrimitivesOffset = node.FirstPrimOffset;
                linearNode.NPrimitives = node.NPrimitives;
            } 
            else 
            {
                //Create interior flattened BVH node
                linearNode.Axis = node.SplitAxis;
                linearNode.NPrimitives = 0;
                FlattenBvhTree(node.Children[0], ref offset);
                linearNode.SecondChildOffset = FlattenBvhTree(node.Children[1], ref offset);                
            }
            return myOffset;            
        }

        private BVHBuildNode HLBVHBuild(List<BVHPrimitiveInfo> primitiveInfo, ref int totalNodes, List<IPrimitive> orderedPrims)
        {
            // Do nothing
            // throw new NotImplementedException();
            return null;
        }

        public BVHBuildNode RecursiveBuild(List<BVHPrimitiveInfo> primitiveInfos, int start, int end, ref int totalNodes, List<IPrimitive> orderedPrims) 
        {
            BVHBuildNode node = new BVHBuildNode();
            totalNodes++;
            // Compute bounds of all primitives in BVH node 
            Bounds3F bounds = new Bounds3F();
            for (int i = start; i < end; ++i)
            {
                bounds = bounds.Union(primitiveInfos[i].Bounds);
            }

            int nPrimitives = end - start;
            if (nPrimitives == 1) {
                // Create leaf BVHBuildNode
                int firstPrimOffset = orderedPrims.Count;
                for (int i = start; i < end; ++i) 
                {
                    int primNum = primitiveInfos[i].PrimitiveNumber;
                    orderedPrims.Add(Primitives[primNum]);
                } 
                node.InitLeaf(firstPrimOffset, nPrimitives, bounds);
                return node;
            }

            // Compute bound of primitive centroids, choose split dimension dim 
            Bounds3F centroidBounds = new Bounds3F();
            for (int i = start; i < end; ++i)
            {
                centroidBounds = centroidBounds.Union(primitiveInfos[i].Centroid);
            }
              
            // Partition primitives into two sets and build children
            int dim = centroidBounds.MaximumExtent;
            if (Math.Abs(centroidBounds.PMax[dim] - centroidBounds.PMin[dim]) < float.Epsilon) 
            {
                // Create leaf _BVHBuildNode_
                int firstPrimOffset = orderedPrims.Count;
                for (int i = start; i < end; ++i) 
                {
                    int primNum = primitiveInfos[i].PrimitiveNumber;
                    orderedPrims.Add(Primitives[primNum]);
                }
                node.InitLeaf(firstPrimOffset, nPrimitives, bounds);
                return node; 
            }

            // Partition primitives based on splitMethod
            switch (SplitMethod) {
                case SplitMethod.Middle:
                    // Partition primitives through node’s midpoint 
                    float pmid = (centroidBounds.PMin[dim] + centroidBounds.PMax[dim]) / 2;
                    primitiveInfos.StdPartition(start, end,  pi => pi.Centroid[dim] < pmid);
                    break;
                case SplitMethod.EqualCounts:
                case SplitMethod.Sah:
                case SplitMethod.Hlbvh:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException(nameof(SplitMethod), SplitMethod, "Invalid SplitMethod value");
            }                    
            // Partition primitives based on splitMethod 
            int mid = (start + end) / 2;
            var c0 = RecursiveBuild(primitiveInfos, start, mid, ref totalNodes, orderedPrims);
            var c1 = RecursiveBuild(primitiveInfos, mid, end, ref totalNodes, orderedPrims);
            node.InitInterior(dim, c0, c1);
            return node;
        }
        
        // BVHAccel Method Definitions 
        public override bool Intersect(Ray ray, out SurfaceInteraction isect) 
        {
            bool hit = false;
            isect = null;
            Vector3F invDir = new Vector3F(1 / ray.D.X, 1 / ray.D.Y, 1 / ray.D.Z);
            int[] dirIsNeg = { invDir.X < 0 ? 1 : 0 , invDir.Y < 0 ? 1 : 0, invDir.Z < 0  ? 1 : 0};
            // Follow ray through BVH nodes to find primitive intersections
            int toVisitOffset = 0;
            int  currentNodeIndex = 0;
            int[] nodesToVisit = new int[64];
            while (true) 
            {
                LinearBVHNode node = Nodes[currentNodeIndex];
                // Check ray against BVH node
                if (node.Bounds.IntersectP(ray, invDir, dirIsNeg)) 
                {
                    if (node.NPrimitives > 0) 
                    {
                        // Intersect ray with primitives in leaf BVH node
                        for (int i = 0; i < node.NPrimitives; ++i)
                        {
                            if (Primitives[node.PrimitivesOffset + i].Intersect(ray, out var primSurfInteraction))
                            {
                                isect = primSurfInteraction;
                                hit = true;
                            }
                        }

                        if (toVisitOffset == 0)
                        {
                            break;
                        }

                        currentNodeIndex = nodesToVisit[--toVisitOffset];
                    }
                    else 
                    {
                        // Put far BVH node on _nodesToVisit_ stack, advance to near node
                        if (dirIsNeg[node.Axis] == 1)
                        {
                            nodesToVisit[toVisitOffset++] = currentNodeIndex + 1;
                            currentNodeIndex = node.SecondChildOffset;
                        }
                        else 
                        {
                            nodesToVisit[toVisitOffset++] =  node.SecondChildOffset;
                            currentNodeIndex = currentNodeIndex + 1;
                        }
                    }
                } 
                else
                {
                    if (toVisitOffset == 0)
                    {
                        break;
                    }

                    currentNodeIndex = nodesToVisit[--toVisitOffset];
                }
            }
            
            return hit;
        }
        
        public override Bounds3F WorldBound()
        {
            return Nodes != null ? Nodes[0].Bounds : new Bounds3F();
        }

        public override bool IntersectP(Ray ray) 
        {
            if (Nodes == null)
            {
                return false;
            }

            Vector3F invDir = new Vector3F(1f / ray.D.X, 1 / ray.D.Y, 1 / ray.D.Z);
            int[] dirIsNeg = {invDir.X < 0 ? 1 : 0, invDir.Y < 0 ? 1 : 0, invDir.Z < 0 ? 1 : 0};
            int[] nodesToVisit = new int[64];
            
            int toVisitOffset = 0, currentNodeIndex = 0;
            while (true) 
            {
                LinearBVHNode node = Nodes[currentNodeIndex];
                if (node.Bounds.IntersectP(ray, invDir, dirIsNeg)) 
                {
                    // Process BVH node _node_ for traversal
                    if (node.NPrimitives > 0) 
                    {
                        for (int i = 0; i < node.NPrimitives; ++i) 
                        {
                            if (Primitives[node.PrimitivesOffset + i].IntersectP(ray)) 
                            {
                                return true;
                            }
                        }
                        if (toVisitOffset == 0)
                        {
                            break;
                        }

                        currentNodeIndex = nodesToVisit[--toVisitOffset];
                    } 
                    else 
                    {
                        if (dirIsNeg[node.Axis] == 0) 
                        {
                            // second child first
                            nodesToVisit[toVisitOffset++] = currentNodeIndex + 1;
                            currentNodeIndex = node.SecondChildOffset;
                        }
                        else 
                        {
                            nodesToVisit[toVisitOffset++] = node.SecondChildOffset;
                            currentNodeIndex = currentNodeIndex + 1;
                        }
                    }
                }
                else 
                {
                    if (toVisitOffset == 0)
                    {
                        break;
                    }

                    currentNodeIndex = nodesToVisit[--toVisitOffset];
                }
            }
            return false;
        }
    }
}