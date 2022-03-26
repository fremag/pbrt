using System;
using NFluent;
using NSubstitute;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class LambertianTransmissionTests
    {
        [Test]
        public void BasicTest()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.T).IsEqualTo(t);
            Check.That(lambTrans.BxdfType).IsEqualTo(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_DIFFUSE);
        }

        [Test]
        public void F_Test()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.F(null, null)).IsEqualTo(new Spectrum(3f/MathF.PI));
        }
        
        
        [Test]
        public void SampleF_Test()
        {
            Spectrum r = new Spectrum(3f);
            var lambertianTransmission = new LambertianTransmission(r);
            var wo = new Vector3F(0, 1, 0);
            Point2F u = new Point2F(0, 0);
            
            var f = lambertianTransmission.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            
            Check.That(wi.X).IsEqualTo(-MathF.Sqrt(2)/2);
            Check.That(wi.Y).IsEqualTo(-MathF.Sqrt(2)/2);
            Check.That(wi.Z).IsCloseTo(0, 1e-3);
            Check.That(f).IsEqualTo(new Spectrum(3f / MathF.PI));
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_NONE);
            Check.That(pdf).IsEqualTo(0f);
        }
        
        [Test]
        public void SampleF_SameHemisphere_Test()
        {
            Spectrum r = new Spectrum(3f);
            var lambertianTransmission = new LambertianTransmission(r);
            var wo = new Vector3F(0, 1, -1);
            Point2F u = new Point2F(0, 0);
            
            var f = lambertianTransmission.Sample_f(wo, out var wi, u, out var pdf, out var sampledType);
            
            Check.That(wi.X).IsEqualTo(-MathF.Sqrt(2)/2);
            Check.That(wi.Y).IsEqualTo(-MathF.Sqrt(2)/2);
            Check.That(wi.Z).IsCloseTo(0, 1e-3);
            Check.That(f).IsEqualTo(new Spectrum(3f / MathF.PI));
            Check.That(sampledType).IsEqualTo(BxDFType.BSDF_NONE);
            Check.That(pdf).IsCloseTo(0f, 1e-4);
        }

        [Test]
        public void Rho_Test()
        {
            Spectrum t = new Spectrum(3f);
            var lambTrans = new LambertianTransmission(t);
            Check.That(lambTrans.Rho(null, 0, null)).IsEqualTo(t);
        }
    }
}