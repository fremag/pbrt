using System;
using NFluent;
using NSubstitute;
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
            // woW . woI < 0 =>  transmission with type transmission+diffuse => match, spectrum = sum(LambertianTransmission) 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_DIFFUSE);
            Check.That(spectrum).IsEqualTo(new Spectrum((3f+4f)/MathF.PI));

            woW = new Vector3F(1,1,1);
            wiW = new Vector3F(-1,1,1);
            // woW . woI > 0 =>  reflection with type reflection+diffuse =>  match, spectrum = sum(LambertianReflection) 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE);
            var f = (1f + 2f) / MathF.PI;
            Check.That(spectrum.C[0]).IsCloseTo(f, 1e-6);

            woW = new Vector3F(1,1,1);
            wiW = new Vector3F(1,1,-1);
            // woW . woI < 0 =>  transmission with type reflection => no match,  spectrum = 0 
            spectrum = bsdf.F(woW, wiW, BxDFType.BSDF_REFLECTION);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
        }

        [Test]
        public void NumComponentsTest()
        {
            bsdf.Add(new SpecularReflection(new Spectrum(1f), new FresnelNoOp()));
            bsdf.Add(new SpecularReflection(new Spectrum(2f), new FresnelNoOp()));
            bsdf.Add(new SpecularTransmission(new Spectrum(3f), 0, 1, TransportMode.Radiance));
            bsdf.Add(new FresnelSpecular(new Spectrum(4f), new Spectrum(1f), 1, 2, TransportMode.Importance));
            bsdf.Add(new FresnelSpecular(new Spectrum(4f), new Spectrum(1f), 1, 2, TransportMode.Importance));
            bsdf.Add(new OrenNayar(new Spectrum(4f), 1f));

            Check.That(bsdf.NumComponents()).IsEqualTo(6);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_GLOSSY)).IsEqualTo(0);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_DIFFUSE)).IsEqualTo(0);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_SPECULAR)).IsEqualTo(0);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_REFLECTION)).IsEqualTo(0);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_TRANSMISSION)).IsEqualTo(0);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR)).IsEqualTo(2);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)).IsEqualTo(1);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR)).IsEqualTo(5);
            Check.That(bsdf.NumComponents(BxDFType.BSDF_REFLECTION | BxDFType.BSDF_DIFFUSE)).IsEqualTo(1);
        }

        [Test]
        public void Pdf_NoBxDF_Test()
        {
            Vector3F woWorld = new Vector3F(1, 0, 0);
            Vector3F wiWorld = new Vector3F(-1, 0, 0);
            var pdf = bsdf.Pdf(woWorld, wiWorld, BxDFType.BSDF_GLOSSY);
            Check.That(pdf).IsEqualTo(0f);
        }
        
        [Test]
        public void Pdf_WoZ_IsZero_Test()
        {
            Vector3F woWorld = new Vector3F(1, 0, 0);
            Vector3F wiWorld = new Vector3F(-1, 0, 0);
            BxDF bxdf = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            bsdf.Add(bxdf);
            
            var pdf = bsdf.Pdf(woWorld, wiWorld, BxDFType.BSDF_GLOSSY);
            Check.That(pdf).IsEqualTo(0f);
        }
        
        [Test]
        public void Pdf_WrongBxDFType_Test()
        {
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Vector3F wiWorld = new Vector3F(-1, 0, -1);
            BxDF bxdf = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            bsdf.Add(bxdf);
            
            var pdf = bsdf.Pdf(woWorld, wiWorld, BxDFType.BSDF_GLOSSY);
            Check.That(pdf).IsEqualTo(0f);
        }
        
        [Test]
        public void Pdf_Test()
        {
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Vector3F wiWorld = new Vector3F(-1, 0, -1);
            BxDF bxdf1 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            BxDF bxdf2 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            bxdf1.Pdf(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(1.23f);
            bxdf2.Pdf(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(2.34f);
            
            bsdf.Add(bxdf1);
            bsdf.Add(bxdf2);
            
            var pdf = bsdf.Pdf(woWorld, wiWorld, BxDFType.BSDF_REFLECTION);
            Check.That(pdf).IsEqualTo((1.23f+2.34f)/2);
        }

        [Test]
        public void Sample_f_NoMatchingComponent_Test()
        {
            BxDF bxdf1 = Substitute.For<BxDF>(BxDFType.BSDF_TRANSMISSION);
            BxDF bxdf2 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            bsdf.Add(bxdf1);
            bsdf.Add(bxdf2);

            BxDFType sampledType = BxDFType.BSDF_NONE;
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Point2F u = new Point2F(0.5f, 0.5f);
            var spectrum = bsdf.Sample_f(woWorld, out var wiWorld, u, out var pdf, BxDFType.BSDF_DIFFUSE, ref sampledType);
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(wiWorld).IsEqualTo(new Vector3F(0,0,0));
            Check.That(pdf).IsEqualTo(0);
        }
        
        [Test]
        public void Sample_f_NoPdf_Test()
        {
            BxDF bxdf1 = Substitute.For<BxDF>(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
            BxDF bxdf2 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            bsdf.Add(bxdf1);
            bsdf.Add(bxdf2);

            BxDFType sampledType = BxDFType.BSDF_NONE;
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Point2F u = new Point2F(0.5f, 0.5f);
            
            var spectrum = bsdf.Sample_f(woWorld, out var wiWorld, u, out var pdf, BxDFType.BSDF_TRANSMISSION, ref sampledType);
            
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(wiWorld).IsEqualTo(new Vector3F(0,0,0));
            Check.That(pdf).IsEqualTo(0);
        }

        [Test]
        public void Sample_f_Test()
        {
            BxDF bxdf1 = Substitute.For<BxDF>(BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR);
            BxDF bxdf2 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);
            BxDF bxdf3 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);

            bxdf2.Pdf(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(3.45f);
            bxdf3.Pdf(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(4.56f);
            bxdf2.F(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(new Spectrum(3.45f));
            bxdf3.F(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(new Spectrum(4.56f));
            
            bsdf.Add(bxdf1);
            bsdf.Add(bxdf2);
            bsdf.Add(bxdf3);
            
            Vector3F wi = new Vector3F(1, 2, 3);
            
            bxdf3.Sample_f(Arg.Any<Vector3F>(), out Arg.Any<Vector3F>(), Arg.Any<Point2F>(), out Arg.Any<float>(), out Arg.Any<BxDFType>()).Returns(info =>
            {
                info[1] = wi;
                info[3] = 2.34f;
                info[4] = bxdf2.BxdfType;
                return new Spectrum(1.23f);
            });
            
            BxDFType sampledType = BxDFType.BSDF_NONE;
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Point2F u = new Point2F(0.5f, 0.5f);
            
            var spectrum = bsdf.Sample_f(woWorld, out var wiWorld, u, out var pdf, BxDFType.BSDF_REFLECTION, ref sampledType);
            
            Check.That(spectrum).IsEqualTo(new Spectrum(3.45f+4.56f));
            Check.That(wiWorld).IsEqualTo(wi);
            Check.That(pdf).IsEqualTo((2.34f+3.45f)/2f);
        }

        [Test]
        public void Sample_f_PdfZero_Test()
        {
            BxDF bxdf2 = Substitute.For<BxDF>(BxDFType.BSDF_REFLECTION);

            bxdf2.Pdf(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(3.45f);
            bxdf2.F(Arg.Any<Vector3F>(), Arg.Any<Vector3F>()).Returns(new Spectrum(3.45f));
            
            bsdf.Add(bxdf2);
            
            Vector3F wi = new Vector3F(1, 2, 3);
            
            bxdf2.Sample_f(Arg.Any<Vector3F>(), out Arg.Any<Vector3F>(), Arg.Any<Point2F>(), out Arg.Any<float>(), out Arg.Any<BxDFType>()).Returns(info =>
            {
                info[1] = wi;
                info[3] = 0f; // Pdf = 0 !
                info[4] = bxdf2.BxdfType;
                return new Spectrum(1.23f);
            });
            
            BxDFType sampledType = BxDFType.BSDF_NONE;
            Vector3F woWorld = new Vector3F(1, 0, 1);
            Point2F u = new Point2F(0.5f, 0.5f);
            
            var spectrum = bsdf.Sample_f(woWorld, out var wiWorld, u, out var pdf, BxDFType.BSDF_REFLECTION, ref sampledType);
            
            Check.That(spectrum).IsEqualTo(new Spectrum(0f));
            Check.That(wiWorld).IsEqualTo(new Vector3F(0,0,0));
            Check.That(pdf).IsEqualTo(0f);
        }
    }
}