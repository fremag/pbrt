using NFluent;
using NUnit.Framework;
using pbrt.Lights;

namespace Pbrt.Tests.Lights
{
    [TestFixture]
    public class LightUtilsTests
    {
        [Test]
        public void BlackBodyTest()
        {
            float[] le = new float[4];
            float[] lambda = new float[] {400, 500, 600, 700};
            LightUtils.BlackBody(lambda, 4, 5000, le);
            Check.That(lambda).ContainsExactly(400, 500, 600, 700);
            Check.That(le).ContainsExactly(8.74357404E+12f, 1.21071836E+13f, 1.2762381E+13f, 1.18119685E+13f);
        }

        [Test]
        public void BlackBodyNormalizedTest()
        {
            float[] le = new float[4];
            float[] lambda = new float[] {400, 500, 600, 700};
            var t = 5000; // Kelvin
            LightUtils.BlackBodyNormalized(lambda, 4, t, le);
            Check.That(lambda).ContainsExactly(400, 500, 600, 700);
            Check.That(le[0]).IsCloseTo(0.683146477f, 1e-5);
            Check.That(le[1]).IsCloseTo(0.945949495f, 1e-5);
            Check.That(le[2]).IsCloseTo(0.997140944f, 1e-5);
            Check.That(le[3]).IsCloseTo(0.9228839871f, 1e-5);
        }
        
    }
}