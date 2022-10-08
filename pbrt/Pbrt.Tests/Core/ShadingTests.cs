using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class ShadingTests
    {
        [Test]
        public void BasicTest()
        {
            Normal3F n = new Normal3F(0, 0, 0);
            Normal3F dndu = new Normal3F(0, 0, 0);
            Normal3F dndv = new Normal3F(0, 0, 0);
            Vector3F dpdu = new Vector3F(0, 0, 0);
            Vector3F dpdv = new Vector3F(0, 0, 0);
            Shading shading = new Shading
            {
                N = n,
                DnDu = dndu,
                DnDv = dndv,
                DpDu = dpdu,
                DpDv = dpdv
            };

            Check.That(shading.N).IsEqualTo(n);
            Check.That(shading.DnDu).IsEqualTo(dndu);
            Check.That(shading.DnDv).IsEqualTo(dndv);
            Check.That(shading.DpDu).IsEqualTo(dpdu);
            Check.That(shading.DpDv).IsEqualTo(dpdv);
        }
    }
}