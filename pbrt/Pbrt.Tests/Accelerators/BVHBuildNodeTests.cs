using NFluent;
using NUnit.Framework;
using pbrt.Accelerators;
using pbrt.Core;

namespace Pbrt.Tests.Accelerators
{
    [TestFixture]
    public class BVHBuildNodeTests
    {
        private BVHBuildNode node;
        private Bounds3F bounds = new Bounds3F(Point3F.Zero, new Point3F(1, 2, 3));

        [SetUp]
        public void SetUp()
        {
            node = new BVHBuildNode();
        }

        [Test]
        public void BasicTest()
        {
            Check.That(node.Bounds).IsNull();
            Check.That(node.Children).ContainsExactly(null, null);
            Check.That(node.NPrimitives).IsZero();
            Check.That(node.SplitAxis).IsZero();
            Check.That(node.FirstPrimOffset).IsZero();
        }

        [Test]
        public void InitLeafTest()
        {
            node.InitLeaf(123, 456, bounds);
            Check.That(node.Bounds).IsSameReferenceAs(bounds);
            Check.That(node.Children).ContainsExactly(null, null);
            Check.That(node.FirstPrimOffset).IsEqualTo(123);
            Check.That(node.NPrimitives).IsEqualTo(456);
            Check.That(node.SplitAxis).IsZero();
        }

        [Test]
        public void InitInteriorTest()
        {
            BVHBuildNode node0 = new BVHBuildNode();
            node0.InitLeaf(123, 456, bounds);

            BVHBuildNode node1 = new BVHBuildNode();
            var boundsBis = new Bounds3F(new Point3F(-1, -1, -1), new Point3F(3, 2, 1));
            node1.InitLeaf(234, 567, boundsBis);

            node.InitInterior(42, node0, node1);

            Check.That(node.Bounds.PMin).IsEqualTo(new Point3F(-1, -1, -1));
            Check.That(node.Bounds.PMax).IsEqualTo(new Point3F(3, 2, 3));
            Check.That(node.Children).ContainsExactly(node0, node1);
            Check.That(node.SplitAxis).IsEqualTo(42);
        }
    }
}