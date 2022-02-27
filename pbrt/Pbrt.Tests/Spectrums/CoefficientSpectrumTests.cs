using NFluent;
using NUnit.Framework;
using pbrt.Spectrums;
// ReSharper disable EqualExpressionComparison
// ReSharper disable SuspiciousTypeConversion.Global
#pragma warning disable CS1718 

namespace Pbrt.Tests.Spectrums
{
    [TestFixture]
    public class CoefficientSpectrumTests
    {
        [Test]
        public void IsBlackTest()
        {
            var cs = new CoefficientSpectrum(10, 0f);
            Check.That(cs.C).CountIs(10);
            Check.That(cs.IsBlack()).IsTrue();

            var csCopy = new CoefficientSpectrum(cs);
            Check.That(csCopy.C).CountIs(10);
            Check.That(csCopy.IsBlack()).IsTrue();

            var csNotBlack = new CoefficientSpectrum(new[] { 1f, 2, 3 });
            Check.That(csNotBlack.IsBlack()).IsFalse();
        }

        [Test]
        public void IndexTest()
        {
            var values = new[] { 1f, 2, 3 };
            var cs = new CoefficientSpectrum(values);
            Check.That(cs.NSpectrumSamples).IsEqualTo(3);
            Check.That(cs[0]).IsEqualTo(1f);
            Check.That(cs[1]).IsEqualTo(2f);
            Check.That(cs[2]).IsEqualTo(3f);
        }
        
        [Test]
        public void EqualsTest()
        {
            var values = new[] { 1f, 2, 3 };
            
            var cs1 = new CoefficientSpectrum(values);
            var cs2 = new CoefficientSpectrum(values);
            var cs3 = new CoefficientSpectrum(1, 0f);

            Check.That(cs1.C).Not.IsSameReferenceAs(values);
            Check.That(cs2.C).Not.IsSameReferenceAs(values);
            Check.That(cs1.Equals(cs1)).IsTrue();
            Check.That(cs1.Equals(cs2)).IsTrue();
            Check.That(cs1.Equals(null)).IsFalse();
            Check.That(cs1.Equals(1)).IsFalse();
            Check.That(cs1.Equals(cs3)).IsFalse();
            Check.That(cs1.Equals(new CoefficientSpectrum(values.Length, 4f))).IsFalse();
        }

        [Test]
        public void GetHashCodeTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs2 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs3 = new CoefficientSpectrum(new [] {2f, 3f});
            
            Check.That(cs1.GetHashCode()).IsEqualTo(cs2.GetHashCode());
            Check.That(cs2.GetHashCode()).Not.IsEqualTo(cs3.GetHashCode());
        } 

        [Test]
        public void AddTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs2 = new CoefficientSpectrum(new [] {2f, 3f});
            cs1.Add(cs2);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 3.23f, 5.34f }));
        } 

        [Test]
        public void SubTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs2 = new CoefficientSpectrum(new [] {2f, 3f});
            cs2.Sub(cs1);
            Check.That(cs2).IsEqualTo(new CoefficientSpectrum(new[] { 0.77f, 0.6600001f }));
        } 

        [Test]
        public void DivTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs2 = new CoefficientSpectrum(new [] {2f, 3f});
            cs1.Div(cs2);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 0.615f, 0.78f }));
        } 
        
        [Test]
        public void MulTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            var cs2 = new CoefficientSpectrum(new [] {2f, 3f});
            cs1.Mul(cs2);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 2.46f, 7.0199995f }));
        } 
        
        [Test]
        public void MulFloatTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            cs1.Mul(2);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 2.46f, 4.68f }));
        } 
        
        [Test]
        public void DivFloatTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            cs1.Div(2);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 0.615f, 1.17f }));
        } 
        
        [Test]
        public void NegTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            cs1.Neg();
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { -1.23f, -2.34f }));
        } 

        [Test]
        public void SqrtTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {4f, 9f});
            cs1.Sqrt();
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 2f, 3f }));
        }
        
        
        [Test]
        public void ClampTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {0.1f, 1.23f, 2.34f});
            cs1.Clamp(0.25f, 2f);
            Check.That(cs1).IsEqualTo(new CoefficientSpectrum(new[] { 0.25f, 1.23f, 2f }));
        } 
         
        [Test]
        public void HasNanTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            Check.That(cs1.HasNaNs()).IsFalse();
            cs1.Mul(float.NaN);
            Check.That(cs1.HasNaNs()).IsTrue();
        }

        [Test]
        public void ToStringTest()
        {
            var cs1 = new CoefficientSpectrum(new [] {1.23f, 2.34f});
            Check.That(cs1.ToString()).IsEqualTo("[1.23, 2.34]");
        }
    }
}