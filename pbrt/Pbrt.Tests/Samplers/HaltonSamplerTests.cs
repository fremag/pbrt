using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Samplers;

namespace Pbrt.Tests.Samplers
{
    [TestFixture]
    public class HaltonSamplerTests
    {
        Point2I p1 = new Point2I(0,0);
        Point2I p2 = new Point2I(15,15);
        private HaltonSampler haltonSampler;
        
        [SetUp]
        public void SetUp()
        {
            haltonSampler = new HaltonSampler(4, new Bounds2I(p1, p2));
        }
        
        [Test]
        public void Reverse32Test()
        {
            var value = HaltonSampler.ReverseBits32(0b000000000000000011110000);
            Check.That(value).IsEqualTo(0b00001111000000000000000000000000);
        }

        [Test]
        public void Reverse64Test()
        {
            var value = HaltonSampler.ReverseBits64(0b00000000000000000000000000000000000000000000000011110000);
            Check.That(value).IsEqualTo(0b0000111100000000000000000000000000000000000000000000000000000000);
        }

        [Test]
        public void RadicalInverse_Base2_Test()
        {
            var nums = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var inv = nums.Select(i => HaltonSampler.RadicalInverse(0, i)).ToArray();
            Check.That(inv).ContainsExactly(0, 1 / 2f, 1 / 4f, 3 / 4F, 1 / 8f, 5 / 8f, 3 / 8f, 7 / 8f);
        }

        [Test]
        public void RadicalInverse_Base3_Test()
        {
            var nums = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var values = nums.Select(i => HaltonSampler.RadicalInverse(1, i)).ToArray();
            var expectedValues = new[] { 0, 1 / 3f, 2 / 3f, 1 / 9F, 4 / 9f, 7 / 9f, 2 / 9f, 5 / 9f };
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                Check.That(value).IsCloseTo(expectedValues[i], 1e-6f);
            }
        }

        [Test]
        public void RadicalInverse_Base5_Test()
        {
            var nums = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var values = nums.Select(i => HaltonSampler.RadicalInverse(2, i)).ToArray();
            var expectedValues = new[] { 0, 1 / 5f, 2 / 5f, 3 / 5F, 4 / 5f, 1 / 25f, 6 / 25f, 11 / 25f };
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                Check.That(value).IsCloseTo(expectedValues[i], 1e-6f);
            }
        }

        [Test]
        public void RadicalInverse_Base7_Test()
        {
            var nums = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var values = nums.Select(i => HaltonSampler.RadicalInverse(3, i)).ToArray();
            var expectedValues = new[] { 0, 1 / 7f, 2 / 7f, 3 / 7F, 4 / 7f, 5 / 7f, 6 / 7f, 1 / 49f };
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                Check.That(value).IsCloseTo(expectedValues[i], 1e-6f);
            }
        }

        [Test]
        public void RadicalInverse_Base11_Test()
        {
            var nums = new uint[] { 0, 1, 2, 3, 4, 5, 6, 7 };
            var values = nums.Select(i => HaltonSampler.RadicalInverse(4, i)).ToArray();
            var expectedValues = new[] { 0, 1 / 11f, 2 / 11f, 3 / 11F, 4 / 11f, 5 / 11f, 6 / 11f, 7 / 11f };
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i];
                Check.That(value).IsCloseTo(expectedValues[i], 1e-6f);
            }
        }

        [Test]
        public void RadicalInverse_Error_Test()
        {
            Check.ThatCode(() => HaltonSampler.RadicalInverse(1_000_000_000, 0)).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void InverseRadicalInverse_Base2_Test()
        {
            var x = HaltonSampler.InverseRadicalInverse(1234, 4, 10);
            Check.That(x).IsEqualTo(4321);
            x = HaltonSampler.InverseRadicalInverse(12340000, 8, 10);
            Check.That(x).IsEqualTo(4321);
        }

        [Test]
        public void ComputeRadicalInversePermutationsTest()
        {
            var rng = new Random(0);
            ushort[] perms = HaltonSampler.ComputeRadicalInversePermutations(rng);
            Check.That(perms).CountIs(HaltonSampler.Primes.Sum());
            Check.That(perms[0..2]).Contains(0, 1);
            Check.That(perms[2..5]).Contains(0, 1, 2);
            Check.That(perms[6..11]).Contains(0, 1, 2, 3, 4);
            Check.That(perms[12..19]).Contains(0, 1, 2, 3, 4, 5, 6);
            Check.That(perms[12..19]).Not.ContainsExactly(0, 1, 2, 3, 4, 5, 6);
        }

        [Test]
        public void ScrambledRadicalInverseTest()
        {
            var rng = new Random(0);
            ushort[] perms = HaltonSampler.ComputeRadicalInversePermutations(rng);

            var scrambledRadicalInverse = HaltonSampler.ScrambledRadicalInverse(0, 1234, perms);
            Check.That(scrambledRadicalInverse).IsCloseTo(0.70654297, 1e-4);
            
            scrambledRadicalInverse = HaltonSampler.ScrambledRadicalInverse(1, 1234, perms);
            Check.That(scrambledRadicalInverse).IsCloseTo(0.12780067, 1e-4);
            
            scrambledRadicalInverse = HaltonSampler.ScrambledRadicalInverse(2, 1234, perms);
            Check.That(scrambledRadicalInverse).IsCloseTo(0.41928002, 1e-4);

            Check.ThatCode(() => HaltonSampler.ScrambledRadicalInverse(1_000_000_000, 1234, perms)).Throws<IndexOutOfRangeException>();
        }

        [Test]
        public void PrimeSumsTest()
        {
            Check.That(HaltonSampler.PrimeSums[0]).IsEqualTo(0);
            Check.That(HaltonSampler.PrimeSums[1]).IsEqualTo(2);
            Check.That(HaltonSampler.PrimeSums[2]).IsEqualTo(5);
            Check.That(HaltonSampler.PrimeSums[3]).IsEqualTo(10);
            Check.That(HaltonSampler.PrimeSums[4]).IsEqualTo(17);
            Check.That(HaltonSampler.PrimeSums[5]).IsEqualTo(28);
            Check.That(HaltonSampler.PrimeSums[6]).IsEqualTo(41);
            Check.That(HaltonSampler.PrimeSums[7]).IsEqualTo(58);
            Check.That(HaltonSampler.PrimeSums[8]).IsEqualTo(77);
            Check.That(HaltonSampler.PrimeSums[9]).IsEqualTo(100);
        }
        
        [Test]
        public void CloneTest()
        {
            var clone = haltonSampler.Clone(0) as HaltonSampler;
            Check.That(clone.SamplesPerPixel).IsEqualTo(4);
            Check.That(clone.SampleBounds.PMin).IsEqualTo(p1);
            Check.That(clone.SampleBounds.PMax).IsEqualTo(p2);
        }

        [Test]
        [TestCase( (ulong)0 ,0, 0)]
        [TestCase( (ulong)1 ,0, 0)]
        [TestCase( (ulong)2 ,0, 0)]
        [TestCase( (ulong)0 ,1, 0)]
        [TestCase( (ulong)1 ,1, 0)]
        [TestCase( (ulong)2 ,1, 0)]
        [TestCase( (ulong)0 ,3, 1f/6)]
        [TestCase( (ulong)1 ,3, 1f/6)]
        [TestCase( (ulong)2 ,3, 0.309523f)]
        public void SampleDimensionTest(ulong index, int dimension, float expected)
        {
            var s = haltonSampler.SampleDimension(index, dimension);
            Check.That(s).IsCloseTo(expected, 1e-5);
        }

        [Test]
        public void GetIndexForSampleTest()
        {
            haltonSampler.StartPixel(new Point2I(0, 0));
            var idx = haltonSampler.GetIndexForSample(0);
            Check.That(idx).IsEqualTo(0);
            
            haltonSampler.StartPixel(new Point2I(5, 7));
            idx = haltonSampler.GetIndexForSample(0);
            Check.That(idx).IsEqualTo(42);
        }

        [Test]
        public void StartPixelTest()
        {
            haltonSampler.Request1DArray(5);
            haltonSampler.Request2DArray(3);
            haltonSampler.StartPixel(p1);
            Check.That(haltonSampler.SamplesPerPixel).IsEqualTo(4);
            Check.That(haltonSampler.SampleStride).IsEqualTo(432ul);
            Check.That(haltonSampler.SampleArray1D).CountIs(1);
            Check.That(haltonSampler.SampleArray1D[0]).CountIs(5*haltonSampler.SamplesPerPixel);
            Check.That(haltonSampler.SampleArray2D).CountIs(1);
            Check.That(haltonSampler.SampleArray2D[0]).CountIs(3*haltonSampler.SamplesPerPixel);
            Check.That(haltonSampler.Samples1DArraySizes).CountIs(1);
            Check.That(haltonSampler.Samples1DArraySizes[0]).IsEqualTo(5);
            Check.That(haltonSampler.Samples2DArraySizes).CountIs(1);
            Check.That(haltonSampler.Samples2DArraySizes[0]).IsEqualTo(3);
        }

        [Test]
        public void Get1DTest()
        {
            haltonSampler.Request1DArray(5);
            haltonSampler.Request2DArray(3);
            haltonSampler.StartPixel(p1);

            Check.That(haltonSampler.Get1D()).IsEqualTo(0);
            Check.That(haltonSampler.Get1D()).IsEqualTo(0);
            Check.That(haltonSampler.Get1D()).IsEqualTo(1/4f);
            Check.That(haltonSampler.Get1D()).IsEqualTo(1/6f);
            Check.That(haltonSampler.Get1D()).IsCloseTo(1/10f, 1e-5f);
            Check.That(haltonSampler.Get1D()).IsCloseTo(1/22f, 1e-5f);
            Check.That(haltonSampler.Get1D()).IsCloseTo(1/28f, 1e-5f);
        }
        
        [Test]
        public void Get2DTest()
        {
            haltonSampler.Request1DArray(5);
            haltonSampler.Request2DArray(3);
            haltonSampler.StartPixel(p1);

            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(0, 0));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/4f, 1/6f));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/22f, 1/28f));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/30f, 1/36f));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/40f, 1/42f));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/46f, 1/52f));
            Check.That(haltonSampler.Get2D()).IsEqualTo(new Point2F(1/58f, 1/60f));
        }

        [Test]
        public void StartNextSampleTest()
        {
            haltonSampler.Request1DArray(5);
            haltonSampler.Request2DArray(3);
            haltonSampler.StartPixel(p1);
            haltonSampler.Get1D();
            haltonSampler.Get1D();
            haltonSampler.Get1D();
            Check.That(haltonSampler.Dimension).IsEqualTo(3);
            haltonSampler.StartNextSample();
            Check.That(haltonSampler.Dimension).IsEqualTo(0);
        }

        [Test]
        public void SetSampleNumberTest()
        {
            haltonSampler.Request1DArray(5);
            haltonSampler.Request2DArray(3);
            haltonSampler.StartPixel(p1);
            haltonSampler.Get1D();
            haltonSampler.Get1D();
            haltonSampler.Get1D();
            Check.That(haltonSampler.Dimension).IsEqualTo(3);
            haltonSampler.SetSampleNumber(0);
            Check.That(haltonSampler.Dimension).IsEqualTo(0);
        }
    }
}