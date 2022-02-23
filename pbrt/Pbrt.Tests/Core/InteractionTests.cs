using NFluent;
using NUnit.Framework;
using pbrt.Core;

namespace Pbrt.Tests.Core
{
    [TestFixture]
    public class InteractionTests
    {
        private Interaction interaction;
        private MediumInterface mediumInterface;
        private readonly Medium inside = new Medium();
        private readonly Medium outside = new Medium();
        private readonly Point3F point = new Point3F(1, 1, 1);
        private readonly Normal3F normal = new Normal3F(0, 1, 0);
        private readonly Vector3F pError = new Vector3F(0.1f, 0.1f, 0.1f);
        private readonly Vector3F wo = new Vector3F(0.1f, 0.1f, 0.1f);
        private readonly float time = 1.23f;

        [SetUp]
        public void SetUp()
        {
            mediumInterface = new MediumInterface(inside, outside);
            interaction = new Interaction(point, normal, pError, wo, time, mediumInterface);
        }

        [Test]
        public void BasicTest()
        {
            Check.That(interaction.N).IsEqualTo(normal);
            Check.That(interaction.P).IsEqualTo(point);
            Check.That(interaction.Time).IsEqualTo(time);
            Check.That(interaction.Wo).IsEqualTo(wo);
            Check.That(interaction.MediumInterface).IsEqualTo(mediumInterface);
            Check.That(interaction.PError).IsEqualTo(pError);
        }

        [Test]
        public void IsSurfaceOrMediumInteractionTest()
        {
            Check.That(interaction.IsSurfaceInteraction()).IsTrue();
            Check.That(interaction.IsMediumInteraction()).IsFalse();
            interaction.N = new Normal3F(0, 0, 0);
            Check.That(interaction.IsSurfaceInteraction()).IsFalse();
            Check.That(interaction.IsMediumInteraction()).IsTrue();
            interaction.N = new Normal3F(0, 1, 1);
            Check.That(interaction.IsSurfaceInteraction()).IsTrue();
            Check.That(interaction.IsMediumInteraction()).IsFalse();
            interaction.N = new Normal3F(1, 0, 1);
            Check.That(interaction.IsSurfaceInteraction()).IsTrue();
            Check.That(interaction.IsMediumInteraction()).IsFalse();
            interaction.N = new Normal3F(0, 1, 1);
            Check.That(interaction.IsSurfaceInteraction()).IsTrue();
            Check.That(interaction.IsMediumInteraction()).IsFalse();
        }

        [Test]
        public void GetMediumTest()
        {
            Check.That(interaction.GetMedium()).IsSameReferenceAs(mediumInterface.Inside);
        }

        [Test]
        public void GetMediumVectorTest()
        {
            var v1 = new Vector3F(0, -1, 0);
            Check.That(interaction.GetMedium(v1)).IsSameReferenceAs(mediumInterface.Inside);
            var v2 = new Vector3F(1, 0, 0);
            Check.That(interaction.GetMedium(v2)).IsSameReferenceAs(mediumInterface.Inside);
            var v3 = new Vector3F(1, 1, 0);
            Check.That(interaction.GetMedium(v3)).IsSameReferenceAs(mediumInterface.Outside);
        }

        [Test]
        public void OffsetRayOrigin()
        {
            var p = interaction.OffsetRayOrigin(point, pError, normal, wo);
            Check.That(p).IsEqualTo(new Point3F(1, 1.1f, 1));
            p = interaction.OffsetRayOrigin(point, pError, normal, -wo);
            Check.That(p).IsEqualTo(new Point3F(1, 0.9f, 1));
        }

        [Test]
        public void SpawnRayTest()
        {
            Vector3F d = new Vector3F(0, 1, 0);
            var spawn = interaction.SpawnRay(d);
            Check.That(spawn.D).IsEqualTo(d);
            Check.That(spawn.O).IsEqualTo(new Point3F(1f, 1.1f, 1f));
            Check.That(spawn.TMax).Not.IsFinite();
            Check.That(spawn.TMax).IsStrictlyPositive();
            Check.That(spawn.Time).IsEqualTo(time);
            Check.That(spawn.Medium).IsEqualTo(outside);

            d = new Vector3F(0, -1, 0);
            spawn = interaction.SpawnRay(d);
            Check.That(spawn.D).IsEqualTo(d);
            Check.That(spawn.O).IsEqualTo(new Point3F(1f, 0.9f, 1f));
            Check.That(spawn.TMax).Not.IsFinite();
            Check.That(spawn.TMax).IsStrictlyPositive();
            Check.That(spawn.Time).IsEqualTo(time);
            Check.That(spawn.Medium).IsEqualTo(inside);
        }

        [Test]
        public void SpawnRayToTest()
        {
            Point3F p = new Point3F(1, 1, 1);
            Ray spawnTo = interaction.SpawnRayTo(p);
            Check.That(spawnTo.O).IsEqualTo(new Point3F(1, 1.1f, 1));
            Check.That(spawnTo.D.X).IsEqualTo(0);
            Check.That(spawnTo.D.Y).IsCloseTo(-0.1f, 1e-6);
            Check.That(spawnTo.D.Z).IsEqualTo(0);
            Check.That(spawnTo.Medium).IsEqualTo(inside);
            Check.That(spawnTo.Time).IsEqualTo(1.23f );
            Check.That(spawnTo.TMax).IsEqualTo(1f-Interaction.ShadowEpsilon);

            p = new Point3F(0, 1.1f, 1);
            spawnTo = interaction.SpawnRayTo(p);
            Check.That(spawnTo.O).IsEqualTo(new Point3F(1, 1.1f, 1));
            Check.That(spawnTo.D.X).IsEqualTo(-1);
            Check.That(spawnTo.D.Y).IsCloseTo(-0f, 1e-6);
            Check.That(spawnTo.D.Z).IsEqualTo(0);
            Check.That(spawnTo.Medium).IsEqualTo(inside);
            Check.That(spawnTo.Time).IsEqualTo(1.23f );
            Check.That(spawnTo.TMax).IsEqualTo(1f-Interaction.ShadowEpsilon);
        }

        [Test]
        public void SpawnRayToInteractionTest()
        {
            var anotherInteraction = new Interaction(point, normal, pError, wo, time, mediumInterface);
            var spawnTo = interaction.SpawnRayTo(anotherInteraction);
            Check.That(spawnTo.O).IsEqualTo(new Point3F(1, 1.1f, 1));
            Check.That(spawnTo.D.X).IsEqualTo(0);
            Check.That(spawnTo.D.Y).IsCloseTo(-0f, 1e-6);
            Check.That(spawnTo.D.Z).IsEqualTo(0);
            Check.That(spawnTo.Medium).IsEqualTo(inside);
            Check.That(spawnTo.Time).IsEqualTo(1.23f );
            Check.That(spawnTo.TMax).IsEqualTo(1f-Interaction.ShadowEpsilon);
        }
    }
}