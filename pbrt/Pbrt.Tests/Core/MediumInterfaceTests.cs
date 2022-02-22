using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class MediumInterfaceTests
    {
        [Test]
        public void BasicTest()
        {
            Medium outside = new Medium();
            Medium inside = new Medium();
            MediumInterface mediumInterface = new MediumInterface(inside, outside);
            Check.That(mediumInterface.Inside).IsSameReferenceAs(inside);
            Check.That(mediumInterface.Outside).IsSameReferenceAs(outside);
            Check.That(mediumInterface.IsMediumTransition()).IsTrue();

            mediumInterface = new MediumInterface(inside);
            Check.That(mediumInterface.Inside).IsSameReferenceAs(inside);
            Check.That(mediumInterface.Outside).IsSameReferenceAs(inside);
            Check.That(mediumInterface.IsMediumTransition()).IsFalse();
        }
    }
}