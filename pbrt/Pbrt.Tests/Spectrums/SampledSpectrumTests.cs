using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class SampledSpectrumTests
    {
        [Test]
        public void ConstructorTest()
        {
            SampledSpectrum spec = new Spectrum(4, 1.23f);
            Check.That(spec.NSpectrumSamples).IsEqualTo(4);
            Check.That(spec.C).ContainsExactly(1.23f, 1.23f, 1.23f, 1.23f);

            Check.That(SampledSpectrum.NSpectralSamples).IsEqualTo(60);
            Check.That(SampledSpectrum.SampledLambdaStart).IsEqualTo(400);
            Check.That(SampledSpectrum.SampledLambdaEnd).IsEqualTo(700);
        } 

        [Test]
        public void ConstructorCopyTest()
        {
            SampledSpectrum spec = new SampledSpectrum(4, 1.23f);
            SampledSpectrum spec2 = new SampledSpectrum(spec);
            
            Check.That(spec2.NSpectrumSamples).IsEqualTo(4);
            Check.That(spec2.C).ContainsExactly(1.23f, 1.23f, 1.23f, 1.23f);
        }

        [Test]
        public void SpectrumSamplesSortedTest()
        {
            Check.That(SampledSpectrum.SpectrumSamplesSorted(new [] {1f, 2f, 3f}, 3)).IsTrue();
            Check.That(SampledSpectrum.SpectrumSamplesSorted(new [] {1f, 4f, 3f}, 3)).IsFalse();
        }

        [Test]
        public void SortSpectrumSamplesTest()
        {
            float[] lambdas = new [] {1.23f, 3.45f, 0.12f, 2.34f};
            float[] values = new [] {1f, 3f, 0f, 2f};
            SampledSpectrum.SortSpectrumSamples(lambdas, values, lambdas.Length, out var sortedLambdas, out var sortedValues);
            Check.That(sortedLambdas).ContainsExactly(0.12f, 1.23f, 2.34f, 3.45f);
            Check.That(sortedValues).ContainsExactly(0, 1,2,3);
        }

        [Test]
        public void LowerThanMinLambda_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 0, 50);
            Check.That(avg).IsEqualTo(1f);
        }
        
        [Test]
        public void GreaterThanMAxLambda_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 500, 600);
            Check.That(avg).IsEqualTo(4f);
        }
        
        [Test]
        public void DiracDistribution_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100};
            float[] values = {1.23f};
            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, 1, 0, 200);
            Check.That(avg).IsEqualTo(1.23f);
        }
        
        [Test]
        public void FullSpectrum_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 0, 500);
            Check.That(avg).IsEqualTo(2.5f);
        }

        [Test]
        public void OneBandAverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 200.00001f, 299.99999f);
            var expected = 0.5f*(3+2)*(300-200)/(300-200);
            Check.That(avg).IsEqualTo(expected);
        }

        [Test]
        public void InterpAverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 2f, 3f, 4f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 150, 350f);
            Check.That(avg).IsEqualTo(2.5);
        }

        [Test]
        public void ConstantSpectrum_AverageSpectrumSamplesTest()
        {
            float[] lambdas = {100, 200, 300, 400};
            float[] values = {1f, 1f, 1f, 1f};

            var avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 150, 350f);
            Check.That(avg).IsEqualTo(1);

            avg = SampledSpectrum.AverageSpectrumSamples(lambdas, values, lambdas.Length, 50, 500f);
            Check.That(avg).IsEqualTo(1);
        }

        [Test]
        public void FromSampledTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);

            float[] expected =
            {
                1.025f, 1.075f, 1.125f, 1.175f, 1.225f, 1.275f, 1.325f, 1.375f,
                1.425f, 1.475f, 1.525f, 1.575f, 1.625f, 1.675f, 1.725f, 1.775f, 1.825f, 1.875f, 1.925f, 1.975f,
                2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f, 2f,
                1.975f, 1.925f, 1.875f, 1.825f, 1.775f, 1.725f, 1.675f, 1.625f, 1.575f, 1.525f, 1.475f,
                1.425f, 1.375f, 1.325f, 1.275f, 1.225f, 1.175f, 1.125f, 1.075f, 1.025f
            };
            for (int i = 0; i < spec.C.Length; i++)
            {
                Check.That(spec.C[i]).IsCloseTo(expected[i], 1e-3);
            }
        }

        [Test]
        public void ToXyzTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var xyz = spec.ToXYZ();
            Check.That(xyz[0]).IsCloseTo(1.81246662, 1e-5);
            Check.That(xyz[1]).IsCloseTo(1.94576085, 1e-5);
            Check.That(xyz[2]).IsCloseTo(1.53008139, 1e-5);
        }

        [Test]
        public void YTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var y = spec.Y();
            Check.That(y).IsCloseTo(207.91795, 1e-4);
        }

        [Test]
        public void ToRgbTest()
        {
            float[] lambdas = new [] {500f, 400f, 600f, 700f};
            float[] values = new [] {2f, 1f, 2f, 1f};

            var spec = SampledSpectrum.FromSampled(lambdas, values, lambdas.Length);
            var rgb = spec.ToRgb();
            Check.That(rgb[0]).IsCloseTo(2.11953449, 1e-4);
            Check.That(rgb[1]).IsCloseTo(1.95706964, 1e-4);
            Check.That(rgb[2]).IsCloseTo(1.32161331, 1e-4);
        }
    }
}