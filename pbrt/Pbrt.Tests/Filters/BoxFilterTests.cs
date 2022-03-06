using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Filters;

namespace Pbrt.Tests.Filters
{
    [TestFixture]
    public class BoxFilterTests
    {
        Vector2F radius = new Vector2F(2,4);
        BoxFilter filter;

        [SetUp]
        public void SetUp()
        {
            filter = new BoxFilter(radius);
        }

        [Test]
        public void EvaluateTest()
        {
            Check.That(filter.Evaluate(null)).IsEqualTo(1);
            Check.That(filter.Evaluate(new Point2F(-1, 3.14f))).IsEqualTo(1);
        }
        
        [Test]
        public void BasicTest()
        {
            Check.That(filter.Radius.X).IsEqualTo(2f);
            Check.That(filter.Radius.Y).IsEqualTo(4f);
            Check.That(filter.InvRadius.X).IsEqualTo(1f/2);
            Check.That(filter.InvRadius.Y).IsEqualTo(1f/4);
        }
    }
}