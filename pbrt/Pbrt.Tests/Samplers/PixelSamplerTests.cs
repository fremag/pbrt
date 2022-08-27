using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Samplers;

namespace Pbrt.Tests.Samplers
{
    [TestFixture]
    public class PixelSamplerTests
    {
        private PixelSampler pixSamp;

        [SetUp]
        public void SetUp()
        {
            pixSamp = new PixelSampler(2, 3);
        }

        [Test]
        public void StartNextSampleTest()
        {
            Check.That(pixSamp.NSampledDimensions).IsEqualTo(3);
            Check.That(pixSamp.SamplesPerPixel).IsEqualTo(2);
            Check.That(pixSamp.Current1DDimension).IsEqualTo(0);
            Check.That(pixSamp.Current2DDimension).IsEqualTo(0);
            Check.That(pixSamp.CurrentPixel).IsNull();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsZero();

            Check.That(pixSamp.StartNextSample()).IsTrue();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(1);

            Check.That(pixSamp.StartNextSample()).IsFalse();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(2);

            Check.That(pixSamp.StartNextSample()).IsFalse();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(3);
        }

        [Test]
        public void SetSampleNumberTest()
        {
            pixSamp.StartNextSample();
            pixSamp.StartNextSample();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(2);
            Check.That(pixSamp.StartNextSample()).IsFalse();
            pixSamp.SetSampleNumber(0);
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(0);
            Check.That(pixSamp.StartNextSample()).IsTrue();
            Check.That(pixSamp.CurrentPixelSampleIndex).IsEqualTo(1);
        }

        [Test]
        public void Get1DTest()
        {
            var oneD = pixSamp.Get1D();
            Check.That(oneD).IsEqualTo(0);
            oneD = pixSamp.Get1D();
            Check.That(oneD).IsEqualTo(0);
            oneD = pixSamp.Get1D();
            Check.That(oneD).IsEqualTo(0);
            oneD = pixSamp.Get1D();
            Check.That(oneD).Not.IsEqualTo(0);
        }
        
        [Test]
        public void Get2DTest()
        {
            var twoD = pixSamp.Get2D();
            Check.That(twoD).Not.IsNull();
            Check.That(twoD.X).IsZero();
            Check.That(twoD.Y).IsZero();
            pixSamp.Get2D();
            pixSamp.Get2D();
            twoD = pixSamp.Get2D();
            Check.That(twoD).Not.IsNull();
            Check.That(twoD.X).Not.IsZero();
            Check.That(twoD.Y).Not.IsZero();
        }
        [Test]
        public void CloneTest()
        {
            var clone = pixSamp.Clone(123);
            Check.That(clone).IsInstanceOf<PixelSampler>();
        }

        [Test]
        public void StartPixelTest()
        {
            Point2I pixel = new Point2I(1, 2);
            pixSamp.StartPixel(pixel);
            Check.That(pixSamp.CurrentPixel).IsSameReferenceAs(pixel);
            Check.That(pixSamp.Array1DOffset).IsZero();
            Check.That(pixSamp.Array2DOffset).IsZero();
        }

        [Test]
        public void GetCameraSampleTest()
        {
            Point2I pixel = new Point2I(1, 2);
            pixSamp.StartPixel(pixel);
            var a = pixSamp.GetCameraSample(Point2I.Zero);
            Check.That(a.PFilm.X).IsZero();
            Check.That(a.PFilm.Y).IsZero();
            Check.That(a.PLens.X).IsZero();
            Check.That(a.PLens.Y).IsZero();
            
            var b = pixSamp.GetCameraSample(Point2I.Zero);
            Check.That(b.PFilm.X).IsZero();
            Check.That(b.PFilm.Y).IsZero();
            Check.That(b.PLens.X).Not.IsZero();
            Check.That(b.PLens.Y).Not.IsZero();
        }

        [Test]
        public void RoundCountTest()
        {
            Check.That(pixSamp.RoundCount(5)).IsEqualTo(5);
        }

        [Test]
        public void MiscTest()
        {
            Check.That(AbstractSampler.OneMinusEpsilon + float.Epsilon).IsEqualTo(1);
        }

        [Test]
        public void Get_Request1DArrayTest()
        {
            pixSamp.Request1DArray(5);
            var floats = pixSamp.Get1DArray(5);
            Check.That(floats).IsNotNull();
            Check.That(floats).CountIs(5*pixSamp.SamplesPerPixel);
            
            floats = pixSamp.Get1DArray(5);
            Check.That(floats).IsNull();
            
            floats = pixSamp.Get1DArray(5);
            Check.That(floats).IsNull();
        }

        [Test]
        public void Get_Request2DArrayTest()
        {
            pixSamp.Request2DArray(5);
            var point2Fs = pixSamp.Get2DArray(5);
            Check.That(point2Fs).IsNotNull();
            Check.That(point2Fs).CountIs(5*pixSamp.SamplesPerPixel);
            
            point2Fs = pixSamp.Get2DArray(5);
            Check.That(point2Fs).IsNull();
            
            point2Fs = pixSamp.Get2DArray(5);
            Check.That(point2Fs).IsNull();
        }
    }
}