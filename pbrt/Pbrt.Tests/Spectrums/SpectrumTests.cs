using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class SpectrumTests
    {
        [Test]
        public void AddTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec2 = new Spectrum(new[] { 2f, 3f });
            var spec = spec1 + spec2;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 3.23f, 5.34f }));
        }

        [Test]
        public void SubTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec2 = new Spectrum(new[] { 2f, 3f });
            var spec = spec2 - spec1;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 0.77f, 0.6600001f }));
        }

        [Test]
        public void DivTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec2 = new Spectrum(new[] { 2f, 3f });
            var spec = spec1 / spec2;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 0.615f, 0.78f }));
        }

        [Test]
        public void MulTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec2 = new Spectrum(new[] { 2f, 3f });
            var spec = spec1 * spec2;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 2.46f, 7.0199995f }));
        }

        [Test]
        public void MulFloatTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec = spec1 * 2;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 2.46f, 4.68f }));
        }

        [Test]
        public void DivFloatTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec = spec1 / 2;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 0.615f, 1.17f }));
        }

        [Test]
        public void InvDivFloatTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec = 2 / spec1;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { 2 / 1.23f, 2 / 2.34f }));
        }

        [Test]
        public void NegTest()
        {
            var spec1 = new Spectrum(new[] { 1.23f, 2.34f });
            var spec = -spec1;
            Check.That(spec).IsEqualTo(new Spectrum(new[] { -1.23f, -2.34f }));
        }

        [Test]
        public void FromSampledTest()
        {
            float[] lambdas = new[] { 500f, 400f, 600f, 700f };
            float[] values = new[] { 2f, 1f, 2f, 1f };

            var spec = Spectrum.FromSampled(lambdas, values, lambdas.Length);

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
        public void LerpTest()
        {
            var spec1 = new Spectrum(new[] { 100, 400f });
            var spec2 = new Spectrum(new[] { 200, 500f });
            
            Check.That(Spectrum.Lerp(0, spec1, spec2)).IsEqualTo(new Spectrum(new[] { 100, 400f }));
            Check.That(Spectrum.Lerp(1, spec1, spec2)).IsEqualTo(new Spectrum(new[] { 200, 500f }));
            Check.That(Spectrum.Lerp(0.5f, spec1, spec2)).IsEqualTo(new Spectrum(new[] { 150, 450f }));
        }
        
    }
}