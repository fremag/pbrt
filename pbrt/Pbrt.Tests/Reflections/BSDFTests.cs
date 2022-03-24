using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Spectrums;
using Pbrt.Tests.Core;

namespace Pbrt.Tests.Reflections
{
    [TestFixture]
    public class BSDFTests
    {
        Vector3F dpdu = new Vector3F(1, 0, 0);
        Vector3F dpdv = new Vector3F(0, 1, 0);
        private BSDF bsdf;
        private SurfaceInteraction si;

        [SetUp]
        public void SetUp()
        {
            si = new SurfaceInteraction(null, null, null, null, dpdu, dpdv, null, null, 0f, null);
            bsdf = new BSDF(si, 1.5f);
            Check.ThatCode(() => bsdf.Sample_f(null, out _, null, out _, BxDFType.BSDF_ALL)).Throws<NotImplementedException>();
        }
        
        [Test]
        public void ReflectTest()
        {
            Vector3F wo = new Vector3F(1, 1, 0);
            Vector3F n = new Vector3F(0, 1, 0);
            var reflected = BSDF.Reflect(wo, n);
            Check.That(reflected).Check((-1, 1, 0));
        }

        [Test]
        public void RefractTest()
        {
            Vector3F wi = new Vector3F(1, 1, 0);
            Normal3F n = new Normal3F(0, 1, 0);
            var refracted = BSDF.Refract(wi, n, 1f/2, out var wt);
            Check.That(refracted).IsTrue();
            Check.That(wt).Check((-0.5f, -1f, 0));
        }

        [Test]
        public void Reflected_RefractTest()
        {
            Vector3F wi = new Vector3F(1, 1, 0).Normalized();
            Normal3F n = new Normal3F(0, 1, 0);
            var refracted = BSDF.Refract(wi, n, 2, out var wt);
            Check.That(refracted).IsFalse();
            Check.That(wt).IsNull();
        }

        [Test]
        public void BasicTest()
        {
            Check.That(bsdf.Eta).IsEqualTo(1.5f);
            Check.That(bsdf.Ng).IsEqualTo(new Normal3F(0, 0, 1));
            Check.That(bsdf.Ns).IsEqualTo(new Normal3F(0, 0, 1));
            Check.That(bsdf.Ss).IsEqualTo(dpdu);
            Check.That(bsdf.Ts).IsEqualTo(dpdv);
        }

        [Test]
        public void AddTest()
        {
            bsdf.Add(new SpecularReflection(new Spectrum(1f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(2f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(3f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(4f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(5f), new FresnelNoOp()));
            Check.That(bsdf.NbBxDFs).IsEqualTo(5);
            bsdf.Add(new SpecularReflection(new Spectrum(6f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(7f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(8f), new FresnelNoOp()));
            Check.ThatCode( () => bsdf.Add(new SpecularReflection(new Spectrum(9f), new FresnelNoOp()))).Throws<InvalidOperationException>();
            Check.That(bsdf.NbBxDFs).IsEqualTo(8);
        }

        [Test]
        public void WorldToLocalTest()
        {
            var v = new Vector3F(1,2,3);
            Check.That(bsdf.WorldToLocal(v)).IsEqualTo(v);
        }

        [Test]
        public void LocalToWorldTest()
        {
            var v = new Vector3F(1,2,3);
            Check.That(bsdf.LocalToWorld(v)).IsEqualTo(v);
        }

        [Test]
        public void F_Test()
        {
            bsdf.Add(new LambertianReflection(new Spectrum(1f)));
            bsdf.Add(new LambertianReflection(new Spectrum(2f)));
            bsdf.Add(new LambertianTransmission(new Spectrum(3f)));
            bsdf.Add(new LambertianTransmission(new Spectrum(4f)));

            Vector3F woW = new Vector3F(1,1,1);
            Vector3F wiW = new Vector3F(-1,1,1);
            
            // woW . woI > 0 =>  reflection with type transmission => no match, spectrum = 0 
            var spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_TRANSMISSION);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));

            woW = new Vector3F(1,1,1);
            wiW = new Vector3F(1,1,-1);
            // woW . woI < 0 =>  transmission with type transmission => match, spectrum = sum(Transmission) 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_TRANSMISSION);
            Check.That(spectrum).IsEqualTo(new Spectrum((3f+4f)/MathF.PI));

            woW = new Vector3F(1,1,1);
            wiW = new Vector3F(-1,1,1);
            // woW . woI > 0 =>  reflection with ype treflection =>  match, spectrum = sum(reflection) 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_REFLECTION);
            var f = (1f + 2f) / MathF.PI;
            Check.That(spectrum.C[0]).IsCloseTo(f, 1e-6);

            woW = new Vector3F(1,1,1);
            wiW = new Vector3F(1,1,-1);
            // woW . woI < 0 =>  transmission with type reflection => no match,  spectrum = 0 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_REFLECTION);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
        }
    }
}