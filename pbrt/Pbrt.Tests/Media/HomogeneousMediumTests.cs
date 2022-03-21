using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Media;
using pbrt.Spectrums;

namespace Pbrt.Tests.Media
{
    [TestFixture]
    public class HomogeneousMediumTests
    {
        Spectrum sigma_a = new Spectrum(1f);
        Spectrum sigma_s = new Spectrum(2f);
        float g = 1.23f;

        [Test]
        public void BasicTest()
        {
            HomogeneousMedium medium = new HomogeneousMedium(sigma_a, sigma_s, g);
            Check.That(medium.G).IsEqualTo(1.23f);
            Check.That(medium.SigmaA).IsEqualTo(new Spectrum(1f));
            Check.That(medium.SigmaS).IsEqualTo(new Spectrum(2f));
            Check.That(medium.SigmaT).IsEqualTo(new Spectrum(3f));
        }

        [Test]
        public void TrTest()
        {
            HomogeneousMedium medium = new HomogeneousMedium(sigma_a, sigma_s, g);
            Ray ray = new Ray(Point3F.Zero, new Vector3F(1, 0, 0), 1);
            var spectrum = medium.Tr(ray, null);
            Check.That(spectrum).IsEqualTo(new Spectrum(MathF.Exp(-3)));
        }
    }
}