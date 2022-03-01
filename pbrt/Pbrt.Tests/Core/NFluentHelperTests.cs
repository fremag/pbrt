using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class NFluentHelperTests
    {
        [Test]
        public void AllTest()
        {
            Check.ThatCode(() => Check.That(new Point3F(1, 2, 3)).Check((0f, 0f, 0f))).ThrowsAny();
            Check.ThatCode(() => Check.That(new Vector3F(1, 2, 3)).Check((0f, 0f, 0f))).ThrowsAny();
            Check.ThatCode(() => Check.That(new Normal3F(1, 2, 3)).Check((0f, 0f, 0f))).ThrowsAny();
            Check.ThatCode(() => Check.That(new Point3F(1, 2, 3)).Check((0, 0, 0))).ThrowsAny();
            Check.ThatCode(() => Check.That(new Vector3F(1, 2, 3)).Check((0, 0, 0))).ThrowsAny();
            Check.ThatCode(() => Check.That(new Normal3F(1, 2, 3)).Check((0, 0, 0))).ThrowsAny();

            Check.That(new Point3F(1, 2, 3)).Check((1f, 2f, 3f));
            Check.That(new Point3F(1, 2, 3)).Check((1, 2, 3));
            Check.That(new Vector3F(1, 2, 3)).Check((1f, 2f, 3f));
            Check.That(new Vector3F(1, 2, 3)).Check((1, 2, 3));
            Check.That(new Normal3F(1, 2, 3)).Check((1f, 2f, 3f));
            Check.That(new Normal3F(1, 2, 3)).Check((1, 2, 3));
        }
    }
}
