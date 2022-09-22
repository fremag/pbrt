using System;
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Samplers;

namespace Pbrt.Tests.Samplers
{
    [TestFixture]
    public class HaltonSamplerTests
    {
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
    }
}