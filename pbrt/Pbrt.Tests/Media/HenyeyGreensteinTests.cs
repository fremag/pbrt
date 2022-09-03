using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Media;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Media
{
    [TestFixture]
    public class HenyeyGreensteinPhaseFunctionTests
    {
        [Test]
        public void BasicTest()
        {
            HenyeyGreensteinPhaseFunction phase = new HenyeyGreensteinPhaseFunction(1.23f);
            Check.That(phase.G).IsEqualTo(1.23f);
            Vector3F wo = new Vector3F(1, 2, 3);
            Vector3F wi = new Vector3F(3, 4, 5);
            Check.That(phase.P(wo, wi)).IsEqualTo(-7.5310556E-05f);
        }

        [Test]
        public void Sample_p_Test()
        {
            HenyeyGreensteinPhaseFunction phase = new HenyeyGreensteinPhaseFunction(1.23f);
            Vector3F wo = new Vector3F(1, 2, 3);
            Point2F u = new(4, 5) ;
            var sampleP = phase.Sample_P(wo, out var wi, u);
            Check.That(sampleP).IsEqualTo(-133.3115f);
            Check.That(wi).IsCloseTo((-1.0196574f, -2.0393147f,-3.0589721f), 1e-5f);

            phase.G = 1e-4f;
            sampleP = phase.Sample_P(wo, out wi, u);
            Check.That(sampleP).IsCloseTo(0.079744875f, 1e-6f);
            Check.That(wi).IsCloseTo((-7, -14f, -21f), 1e-5f);
        }
    }
}