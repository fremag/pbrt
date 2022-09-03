using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Media;

namespace Pbrt.Tests.Media
{
    [TestFixture]
    public class MediumInteractionTests
    {
        [Test]
        public void BasicTest()
        {
            Point3F p = new Point3F(1.23f, 2.34f, 3.45f);
            Vector3F wo = new Vector3F(1, 2, 3);
            float time = 0.01f;
            Medium medium = HomogeneousMedium.Default();
            PhaseFunction phase = new MockPhaseFunction();
            MediumInteraction mediumInteraction = new MediumInteraction(p, wo, time, medium, phase);

            Check.That(mediumInteraction.P).IsEqualTo(p);
            Check.That(mediumInteraction.Wo).IsEqualTo(wo);
            Check.That(mediumInteraction.Time).IsEqualTo(time);
            Check.That(mediumInteraction.Medium).IsEqualTo(medium);
            Check.That(mediumInteraction.Phase).IsEqualTo(phase);
        }
    }

    public class MockPhaseFunction : PhaseFunction
    {
        public override float P(Vector3F wo, Vector3F wi)
        {
            return 0;
        }

        public override float Sample_P(Vector3F wo, out Vector3F wi, Point2F u)
        {
            wi = null;
            return 0;
        }
    }
}