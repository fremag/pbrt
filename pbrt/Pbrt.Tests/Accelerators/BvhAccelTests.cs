using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Accelerators;
using pbrt.Core;
using pbrt.Media;
using pbrt.Shapes;
using pbrt.Spectrums;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Accelerators
{
    [TestFixture]
    public class BvhAccelTests
    {
        private static IPrimitive CreateSphere(float x, float y, float z, float radius = 1)
        {
            var translation = Transform.Translate(x, y, z);
            var sphere = new Sphere(translation, translation.Inverse(), false, radius, -radius, radius, 360);
            return new GeometricPrimitive(sphere, null, null, new MediumInterface(HomogeneousMedium.Default()));
        }

        [Test]
        public void HlbvhTest()
        {
            List<IPrimitive> primitives = new List<IPrimitive>();
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Hlbvh);
            Check.That(bvh.SplitMethod).IsEqualTo(SplitMethod.Hlbvh);
            Check.That(bvh.MaxPrimsInNode).IsEqualTo(2);
            Check.That(bvh.Primitives).IsEmpty();
        }
        
        [Test]
        public void NotImplementedSplitMethodsTest()
        {
            List<IPrimitive> primitives = new List<IPrimitive>();
            Check.ThatCode(() => new BvhAccel(primitives, 2, SplitMethod.Sah)).Throws<NotImplementedException>();
            Check.ThatCode(() => new BvhAccel(primitives, 2, SplitMethod.EqualCounts)).Throws<NotImplementedException>();
            Check.ThatCode(() => new BvhAccel(primitives, 2, (SplitMethod)(-1))).Throws<ArgumentOutOfRangeException>();
        }
        
        [Test]
        public void NoPrimitiveTest()
        {
            var bvh = new BvhAccel(null, 2, SplitMethod.Middle);
            Check.That(bvh.SplitMethod).IsEqualTo(SplitMethod.Middle);
            Check.That(bvh.MaxPrimsInNode).IsEqualTo(2);
            Check.That(bvh.Primitives).IsNull();
        }
        
        [Test]
        public void OneSphere_RecursiveBuildTest()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Check.That(bvh.SplitMethod).IsEqualTo(SplitMethod.Middle);
            Check.That(bvh.MaxPrimsInNode).IsEqualTo(2);
            Check.That(bvh.Primitives).ContainsExactly(sphere1);
            Check.That(bvh.Nodes).CountIs(1);
            var linearBvhNode = bvh.Nodes[0];
            Check.That(linearBvhNode.Axis).IsEqualTo(0);
            Check.That(linearBvhNode.NPrimitives).IsEqualTo(1);
            Check.That(linearBvhNode.PrimitivesOffset).IsEqualTo(0);
            Check.That(linearBvhNode.SecondChildOffset).IsEqualTo(0);
            Check.That(linearBvhNode.Bounds.PMin).IsEqualTo(new Point3F(-1, -1, -1));
            Check.That(linearBvhNode.Bounds.PMax).IsEqualTo(new Point3F(1, 1, 1));
        }

        [Test]
        public void WorldBoundsTest()
        {
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                CreateSphere(0, -1, 0),
                CreateSphere(5, 2, 1)
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            var worldBounds = bvh.WorldBound();
            Check.That(worldBounds.PMin).IsEqualTo(new Point3F(-1,-2,-1));
            Check.That(worldBounds.PMax).IsEqualTo(new Point3F(6,3,2));
        }
        [Test]
        public void TwoSpheres_RecursiveBuildTest()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Check.That(bvh.SplitMethod).IsEqualTo(SplitMethod.Middle);
            Check.That(bvh.MaxPrimsInNode).IsEqualTo(2);
            Check.That(bvh.Primitives).ContainsExactly(sphere1, sphere2);
            Check.That(bvh.Nodes).CountIs(3);

            var node0 = bvh.Nodes[0];
            var node1 = bvh.Nodes[1];
            var node2 = bvh.Nodes[2];
            CheckNode(node0, (-1, -1, -1), (6, 1, 1));
            CheckNode(node1, (-1, -1, -1), (1, 1, 1));
            CheckNode(node2, (4, -1, -1), (6, 1, 1));
        }

        [Test]
        public void ACubeOfSpheres_RecursiveBuildTest()
        {
            int n = 5;
            List<IPrimitive> primitives = new List<IPrimitive>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        var sphere = CreateSphere(2*i, 2*j, 2*k);
                        primitives.Add(sphere);
                    }
                }
            }
            
            var bvh = new BvhAccel(primitives.ToList(), 2, SplitMethod.Middle);
            Check.That(bvh.SplitMethod).IsEqualTo(SplitMethod.Middle);
            Check.That(bvh.MaxPrimsInNode).IsEqualTo(2);
            Check.That(bvh.Nodes).CountIs(249);
            Check.That(primitives).CountIs(n*n*n);
            var nbPrims = bvh.Nodes.Select(node => node.NPrimitives).Sum();
            Check.That(nbPrims).IsEqualTo(primitives.Count);
        }

        private void CheckNode(LinearBVHNode node, (float x, float y, float z) pMin, (float x, float y, float z) pMax)
        {
            Check.That(node.Bounds.PMin.X).IsEqualTo(pMin.x);
            Check.That(node.Bounds.PMin.Y).IsEqualTo(pMin.y);
            Check.That(node.Bounds.PMin.Z).IsEqualTo(pMin.z);
            Check.That(node.Bounds.PMax.X).IsEqualTo(pMax.x);
            Check.That(node.Bounds.PMax.Y).IsEqualTo(pMax.y);
            Check.That(node.Bounds.PMax.Z).IsEqualTo(pMax.z);
        }
        
        [Test]
        public void OneSphere_IntersectionTest()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);

            Point3F o = new Point3F(-5, 0, 0);
            var dir = new Vector3F(1, 0, 0);
            Ray ray = new Ray(o, dir, 1000, 1, HomogeneousMedium.Default());
            var inter = bvh.Intersect(ray, out var isec);
            Check.That(inter).IsTrue();
            Check.That(isec.P).IsEqualTo(new Point3F(-1, 0, 0));
            Check.That(isec.N).IsEqualTo(new Normal3F(-1, 0, 0));
        }
        
        [Test]
        public void OneSphere_DirIsNeg_IntersectionTest()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);

            Point3F o = new Point3F(5, 0, 0);
            var dir = new Vector3F(-1, 0, 0);
            Ray ray = new Ray(o, dir, 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.Intersect(ray, out var isec);
            
            Check.That(inter).IsTrue();
            Check.That(isec.P).IsEqualTo(new Point3F( 1, 0, 0));
            Check.That(isec.N).IsEqualTo(new Normal3F(1, 0, 0));
        }

        [Test]
        public void ACubeOfSpheres_IntersectionTest()
        {
            int n = 5;
            List<IPrimitive> primitives = new List<IPrimitive>();
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        var sphere = CreateSphere(2 * i, 2 * j, 2 * k);
                        primitives.Add(sphere);
                    }
                }
            }

            var bvh = new BvhAccel(primitives.ToList(), 2, SplitMethod.Middle);
            Point3F o = new Point3F(-5, 2, 6);
            var dir = new Vector3F(1, 0, 0);
            Ray ray = new Ray(o, dir, 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.Intersect(ray, out var isec);
            
            Check.That(inter).IsTrue();
            Check.That(isec.P).IsEqualTo((Point3F)(-1, 2, 6));
            Check.That(isec.N).IsEqualTo(new Normal3F(-1, 0, 0));
        }

        [Test]
        public void TwoSpheres_IntersectBuildTest()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((10, 0, 0), (-1, 0, 0), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.Intersect(ray, out var isec);
            Check.That(inter).IsTrue();
            Check.That(isec.P).Check((6, 0, 0));
            Check.That(isec.N).Check((1f, 0f, 0f));
        }

        [Test]
        public void SameCentroid_RecursiveBuildTest()
        {
            // two spheres with same center with different radius => same centroid
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(0, 0, 0, 10);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };

            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Check.That(bvh.Nodes).CountIs(1);
            Check.That(bvh.Nodes[0].Bounds.PMin).Check((-10, -10, -10));
            Check.That(bvh.Nodes[0].Bounds.PMax).Check((10, 10, 10));
        }
        
        [Test]
        public void TwoSpheres_IntersectP_Test()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((10, 0, 0), (-1, 0, 0), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.IntersectP(ray);
            Check.That(inter).IsTrue();
        }
        
        [Test]
        public void TwoSpheres_IntersectP_2_Test()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((0, 0, -5), (0, 0, 1), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.IntersectP(ray);
            Check.That(inter).IsTrue();
        }
        
        [Test]
        public void TwoSpheres_IntersectP_3_Test()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((0.9f, 0.9f, -5), (0, 0, 1), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.IntersectP(ray);
            Check.That(inter).IsFalse();
        }
        
        [Test]
        public void TwoSpheres_IntersectP_4_Test()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            var sphere3 = CreateSphere(4, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2, sphere3
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((4.5f, 0.9f, -5), (0, 0, 1), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.IntersectP(ray);
            Check.That(inter).IsFalse();
        }
        
        [Test]
        public void TwoSpheres_IntersectP_OutsideBounds_Test()
        {
            var sphere1 = CreateSphere(0, 0, 0);
            var sphere2 = CreateSphere(5, 0, 0);
            List<IPrimitive> primitives = new List<IPrimitive>
            {
                sphere1, sphere2
            };
            
            var bvh = new BvhAccel(primitives, 2, SplitMethod.Middle);
            Ray ray = new Ray((10, 10, 0), (-1, 0, 0), 1000, 1, HomogeneousMedium.Default());
            
            var inter = bvh.IntersectP(ray);
            Check.That(inter).IsFalse();
        }

        [Test]
        public void NoPrimitive_IntersectP_Test()
        {
            var bvh = new BvhAccel(null, 2, SplitMethod.Middle);
            var inter = bvh.IntersectP(null);
            Check.That(inter).IsFalse();
        }
    }
}