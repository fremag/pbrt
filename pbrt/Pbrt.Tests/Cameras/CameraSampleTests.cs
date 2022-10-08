using NFluent;
using NUnit.Framework;
using pbrt.Cameras;
using pbrt.Core;

namespace Pbrt.Tests.Cameras
{
    [TestFixture]
    public class CameraSampleTests
    {
        [Test]
        public void BasicTest()
        {
            var sample = new CameraSample { PFilm = new Point2F(1, 2), PLens = new Point2F(3, 4), Time = 1.23f };
            var sampleCopy = sample;
            Check.That(sample).Not.IsSameReferenceAs(sampleCopy);
            Check.That(sampleCopy.PFilm).IsSameReferenceAs(sample.PFilm);
            Check.That(sampleCopy.PLens).IsSameReferenceAs(sample.PLens);

            Check.That(sample.PFilm.X).IsEqualTo(1);
            Check.That(sampleCopy.PFilm.X).IsEqualTo(1);

            sample.PFilm.X++;

            Check.That(sample.PFilm.X).IsEqualTo(2);
            Check.That(sample.PFilm.X).IsEqualTo(2);
            Check.That(sample.Time).IsEqualTo(1.23f);
            Check.That(sampleCopy.Time).IsEqualTo(1.23f);
        }
    }
}