using System;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using Pbrt.Tests.Core;

namespace Pbrt.Tests
{
    public class MathUtilsTests
    {
        [SetUp]
        public void Setup()
        {
            Check.That(MathUtils.InvPI).IsCloseTo(1/MathF.PI, 1e-9f);
            Check.That(MathUtils.Inv2PI).IsCloseTo(0.5f/MathF.PI, 1e-9f);
            Check.That(MathUtils.Inv4PI).IsCloseTo(0.25f/MathF.PI, 1e-9f);
        }

        [Test]
        public void FloatToBitsToFloatTest()
        {
            float f = 1.23f;
            var ui = MathUtils.FloatToBits(f);
            var ff = MathUtils.BitsToFloat(ui);
            Check.That(ff).IsEqualTo(f);
        }
        
        [Test]
        public void DoubleToBitsToDoubleTest()
        {
            double d = 1.23;
            ulong ul = MathUtils.DoubleToBits(d);
            double dd = MathUtils.BitsToDouble(ul);
            Check.That(dd).IsEqualTo(d);
        }

        [Test]
        public void NextFloatUpTest()
        {
            var f = 1.23f;
            var nextF = MathUtils.NextFloatUp(f);
            var delta = nextF-f;
            Check.That(delta).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyPositive();

            var nextFloatUp = MathUtils.NextFloatUp(float.PositiveInfinity);
            Check.That(float.IsPositiveInfinity(nextFloatUp)).IsTrue();
            Check.That(MathUtils.NextFloatUp(0f)).IsEqualTo(float.Epsilon);

            Check.That(MathUtils.NextFloatUp(-float.Epsilon)).IsZero();
        }
        
        [Test]
        public void NextFloatDownTest()
        {
            var f = 1.23f;
            var nextF = MathUtils.NextFloatDown(f);
            var delta = nextF-f;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyNegative();

            var nextFloatDown = MathUtils.NextFloatDown(float.NegativeInfinity);
            Check.That(float.IsNegativeInfinity(nextFloatDown)).IsTrue();
            Check.That(MathUtils.NextFloatDown(0f)).IsEqualTo(-float.Epsilon);
        }
        [Test]
        public void _NextFloatUpTest()
        {
            var f = 1.23f;
            var nextF = MathUtils._NextFloatUp(f);
            var delta = nextF-f;
            Check.That(delta).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyPositive();

            var nextFloatUp = MathUtils._NextFloatUp(float.PositiveInfinity);
            Check.That(float.IsPositiveInfinity(nextFloatUp)).IsTrue();
            Check.That(MathUtils._NextFloatUp(0f)).IsEqualTo(float.Epsilon);

            Check.That(MathUtils._NextFloatUp(-float.Epsilon)).IsZero();
        }
        
        [Test]
        public void _NextFloatDownTest()
        {
            var f = 1.23f;
            var nextF = MathUtils._NextFloatDown(f);
            var delta = nextF-f;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6f);
            Check.That(delta).IsStrictlyNegative();

            var nextFloatDown = MathUtils._NextFloatDown(float.NegativeInfinity);
            Check.That(float.IsNegativeInfinity(nextFloatDown)).IsTrue();
            Check.That(MathUtils._NextFloatDown(0f)).IsEqualTo(-float.Epsilon);
        }
        [Test]
        public void NextDoubletUpTest()
        {
            var d = 1.23d;
            var nextF = MathUtils.NextDoubleUp(d);
            var delta = nextF-d;
            Check.That(delta).IsLessOrEqualThan(1e-6d);
            Check.That(delta).IsStrictlyPositive();

            var nextDoubleDown = MathUtils.NextDoubleUp(double.PositiveInfinity);
            Check.That(double.IsPositiveInfinity(nextDoubleDown)).IsTrue();
            Check.That(MathUtils.NextDoubleUp(0f)).IsEqualTo(double.Epsilon);

            Check.That(MathUtils.NextDoubleUp(-double.Epsilon)).IsZero();
        }
        
        [Test]
        public void NextDoubleDownTest()
        {
            var d = 1.23f;
            var nextF = MathUtils.NextDoubleDown(d);
            var delta = nextF-d;
            Check.That(Math.Abs(delta)).IsLessOrEqualThan(1e-6d);
            Check.That(delta).IsStrictlyNegative();

            var nextDoubleDown = MathUtils.NextDoubleDown(double.NegativeInfinity);
            Check.That(Double.IsNegativeInfinity(nextDoubleDown)).IsTrue();
            Check.That(MathUtils.NextDoubleDown(0f)).IsEqualTo(-double.Epsilon);
        }

        [Test]
        public void GammaTest()
        {
            Check.That(MathUtils.Gamma(0)).IsEqualTo(0);
            Check.That(MathUtils.Gamma(1)).IsCloseTo(MathUtils.MachineEpsilon, 1e-12);
            Check.That(MathUtils.Gamma(3)).IsCloseTo(3.576271E-07f, 1e-12);
        }

        [Test]
        public void ClampTest()
        {
            Check.That(0f.Clamp( -1, 1)).IsEqualTo(0);
            Check.That((-5f).Clamp( -1, 1)).IsEqualTo(-1);
            Check.That(5f.Clamp(-1, 1)).IsEqualTo(1);
        }

        [Test]
        public void DegTest()
        {
            Check.That(MathF.PI.Degrees()).IsEqualTo(180);
            Check.That((2*MathF.PI).Degrees()).IsEqualTo(360);
            Check.That(0f.Degrees()).IsEqualTo(0);
        }

        [Test]
        public void RadTest()
        {
            Check.That(180f.Radians()).IsEqualTo(MathF.PI);
            Check.That((360f).Radians()).IsEqualTo(MathF.PI*2);
            Check.That(0f.Radians()).IsEqualTo(0);
        }

        [Test]
        [TestCase(1, 1, 1, 0, 0, false)]
        [TestCase(1, 0, 1, 0, 0, false)]
        [TestCase(1, -4, 3, 1, 3, true)]
        [TestCase(-1, 4, -3, 1, 3, true)]
        public void QuadraticTest(float a, float b, float c, float expectedT0, float expectedT1, bool expectedResult)
        {
            var result = MathUtils.Quadratic(a, b, c, out var t0, out var t1);
            Check.That(result).IsEqualTo(expectedResult);
            Check.That(t0).IsEqualTo(expectedT0);
            Check.That(t1).IsEqualTo(expectedT1);
        }

        [Test]
        public void SphericalDirectionTest()
        {
            var v = MathUtils.SphericalDirection(MathF.Sqrt(2)/2, MathF.Sqrt(2)/2, MathF.PI/4);
            Check.That(v.X).IsCloseTo(0.5f, 1e-4);
            Check.That(v.Y).IsCloseTo(0.5f, 1e-4);
            Check.That(v.Z).IsCloseTo(MathF.Sqrt(2)/2, 1e-4);

            v = MathUtils.SphericalDirection(0, 1, 0);
            Check.That(v.X).IsCloseTo(0f, 1e-4);
            Check.That(v.Y).IsCloseTo(0f, 1e-4);
            Check.That(v.Z).IsCloseTo(1, 1e-4);

            v = MathUtils.SphericalDirection(1, 0, 0);
            Check.That(v.X).IsCloseTo(1f, 1e-4);
            Check.That(v.Y).IsCloseTo(0f, 1e-4);
            Check.That(v.Z).IsCloseTo(0, 1e-4);
        }
        
        [Test]
        public void SphericalDirectionVectorTest()
        {
            var x = new Vector3F(1f, 0, 0);
            var y = new Vector3F(0, 1, 0);
            var z = new Vector3F(0, 0, 1);
            
            var v = MathUtils.SphericalDirection(0, 1, 0, x, y, z);
            Check.That(v.X).IsCloseTo(0f, 1e-4);
            Check.That(v.Y).IsCloseTo(0f, 1e-4);
            Check.That(v.Z).IsCloseTo(1, 1e-4);

            v = MathUtils.SphericalDirection(1, 0, 0, x, y, z);
            Check.That(v.X).IsCloseTo(1f, 1e-4);
            Check.That(v.Y).IsCloseTo(0f, 1e-4);
            Check.That(v.Z).IsCloseTo(0, 1e-4);
        }

        [Test]
        public void SphericalThetaTest()
        {
            Check.That(MathUtils.SphericalTheta(new Vector3F(5, 4, 0))).IsEqualTo(MathF.PI/2);
            Check.That(MathUtils.SphericalTheta(new Vector3F(1, 1, 1))).IsEqualTo(0);
            Check.That(MathUtils.SphericalTheta(new Vector3F(1, 1, MathF.Sqrt(2)/2))).IsCloseTo(Math.PI/4, 1e-4);
        }
        [Test]
        public void SphericalPhiTest()
        {
            Check.That(MathUtils.SphericalPhi(new Vector3F(0, 1, 0))).IsEqualTo(MathF.PI/2);
            Check.That(MathUtils.SphericalPhi(new Vector3F(1, 0, 5))).IsEqualTo(0);
            Check.That(MathUtils.SphericalPhi(new Vector3F(1, 1, 1))).IsCloseTo(Math.PI/4, 1e-4);
            Check.That(MathUtils.SphericalPhi(new Vector3F(-1, 1, 1))).IsCloseTo(3*Math.PI/4, 1e-4);
            Check.That(MathUtils.SphericalPhi(new Vector3F(-1, -1, 1))).IsCloseTo(5*Math.PI/4, 1e-4);
        }

        [Test]
        [TestCase(0F, 0F, -0.707F, -0.707F)]
        [TestCase(1F, 1F, 0.707F, 0.707F)]
        [TestCase(0F, 1F, -0.707F, 0.707F)]
        [TestCase(1F, 0F, 0.707F, -0.707F)]
        [TestCase(0.5F, 0.5F, 0F, 0F)]
        [TestCase(-0.5F, 0.5F, -2F, 0F)]
        public void ConcentricSampleDiskTest(float u, float v, float pX, float pY)
        {
            Point2F uv = new Point2F(u, v);
            var p = MathUtils.ConcentricSampleDisk(uv);
            Check.That(p.X).IsCloseTo(pX, 1e-3);
            Check.That(p.Y).IsCloseTo(pY, 1e-3);
        }

        [Test]
        public void SolveLinearSystem2X2Test()
        {
            float[][] a = new float[][] { new []{0f, 1f}, new []{0f, 1f}};
            float[] b = new []{0f, 1f};
            var x = MathUtils.SolveLinearSystem2X2(a, b, out var x0, out var x1);
            Check.That(x).IsFalse();
            Check.That(x0).IsZero();
            Check.That(x1).IsZero();

            a = new float[][] { new []{2f, 1f}, new []{1f, 1f}};
            b = new []{1f, 2f};
            x = MathUtils.SolveLinearSystem2X2(a, b, out x0, out x1);
            Check.That(x).IsTrue();
            Check.That(x0).IsEqualTo(-1);
            Check.That(x1).IsEqualTo(3);
        }

        [Test]
        public void RejectionSampleDiskTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                var p = MathUtils.RejectionSampleDisk(r);
                sumX += p.X;
                sumY += p.Y;
                
                Check.That(p.X*p.X+p.Y*p.Y).IsLessOrEqualThan(1);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
        }

        [Test]
        public void UniformSampleHemisphereTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var v = MathUtils.UniformSampleHemisphere(uv);
                sumX += v.X;
                sumY += v.Y;
                
                Check.That(v.X*v.X+v.Y*v.Y+v.Z*v.Z).IsCloseTo(1, 1e-5);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
        }

        [Test]
        public void UniformSampleSphereTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var sumZ = 0f;
            
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var v = MathUtils.UniformSampleSphere(uv);
                sumX += v.X;
                sumY += v.Y;
                sumZ += v.Y;
                
                Check.That(v.X*v.X+v.Y*v.Y+v.Z*v.Z).IsLessOrEqualThan(1+1e-6f);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
            Check.That(sumZ/n).IsCloseTo(0, 1e-2);
        }
        
        [Test]
        public void BasicTest()
        {
            Check.That(1/MathUtils.UniformHemispherePdf).IsCloseTo(2*MathF.PI, 1e-6);
            Check.That(1/MathUtils.UniformSpherePdf).IsCloseTo(4*MathF.PI, 1e-6);
        }

        [Test]
        public void CosineHemispherePdfTest()
        {
            Check.That(MathUtils.CosineHemispherePdf(1f)).IsCloseTo(1f/MathF.PI, 1e-6);
            Check.That(MathUtils.CosineHemispherePdf(0f)).IsCloseTo(0f/MathF.PI, 1e-6);
            Check.That(MathUtils.CosineHemispherePdf(0.5f)).IsCloseTo(0.5f/MathF.PI, 1e-6);
        }
 
        [Test]
        public void UniformSampleDiskTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var v = MathUtils.UniformSampleDisk(uv);
                sumX += v.X;
                sumY += v.Y;
                
                Check.That(v.X*v.X+v.Y*v.Y).IsLessOrEqualThan(1+1e-5f);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
        }
 
        [Test]
        public void CosineSampleHemisphereTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var sumZ = 0f;
            
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var v = MathUtils.CosineSampleHemisphere(uv);
                sumX += v.X;
                sumY += v.Y;
                sumZ += v.Y;
                
                Check.That(v.X*v.X+v.Y*v.Y+v.Z*v.Z).IsCloseTo(1, 1e-6f);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
            Check.That(sumZ/n).IsCloseTo(0, 1e-2);
        }

        [Test]
        public void UniformConePdfTest()
        {
            Check.That(MathUtils.UniformConePdf(1f)).Not.IsFinite();
            Check.That(MathUtils.UniformConePdf(0f)).IsCloseTo(0.5f/MathF.PI, 1e-6);
            Check.That(MathUtils.UniformConePdf(0.5f)).IsCloseTo(1f/MathF.PI, 1e-6);
        }
        
        [Test]
        public void UniformSampleConeTest()
        {
            Random r = new Random(1337);
            var sumX = 0f;
            var sumY = 0f;
            var sumZ = 0f;
            
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var v = MathUtils.UniformSampleCone(uv, 0.5f);
                sumX += v.X;
                sumY += v.Y;
                sumZ += v.Y;
                
                Check.That(v.X*v.X+v.Y*v.Y+v.Z*v.Z).IsCloseTo(1, 1e-6f);
            }

            Check.That(sumX/n).IsCloseTo(0, 1e-2);
            Check.That(sumY/n).IsCloseTo(0, 1e-2);
            Check.That(sumZ/n).IsCloseTo(0, 1e-2);
        }

        [Test]
        public void UniformSampleTriangleTest()
        {
            Random r = new Random(1337);
            
            var n = 100_000;
            for (int i = 0; i < n; i++)
            {
                Point2F uv = new Point2F((float)r.NextDouble(), (float)r.NextDouble()) ;
                var p = MathUtils.UniformSampleTriangle(uv);
                // check all points are below the hypotenuse of the a rect isosceles triangle side = 1 
                Check.That(p.X+p.Y).IsLessOrEqualThan(1f);
            }
        }

        [Test]
        public void PowerHeuristicTest()
        {
            Check.That(MathUtils.PowerHeuristic(1, 2, 3, 4)).IsEqualTo( (2f*1f)*(2*1) / ( (2*1f)*(2*1)+3f*4*3*4));
        }

        [Test]
        public void BalanceHeuristicTest()
        {
            Check.That(MathUtils.BalanceHeuristic(1, 2, 3, 4)).IsEqualTo( (2f*1f) / ( (2*1f)+3f*4));
        }

        [Test]
        public void ErfTest()
        {
            Check.That(MathUtils.Erf(1)).IsEqualTo(0.8427007f);
            Check.That(MathUtils.Erf(-1)).IsEqualTo(-0.8427007f);
            Check.That(MathUtils.Erf(0)).IsEqualTo(0f);
            Check.That(MathUtils.Erf(5)).IsEqualTo(1);
        }

        [Test]
        public void ErfInvTest()
        {
            Check.That(MathUtils.ErfInv(1)).IsEqualTo(3.1232057f);
            Check.That(MathUtils.ErfInv(-1)).IsEqualTo(-3.1232057f);
            Check.That(MathUtils.ErfInv(0)).IsEqualTo(0f);
            Check.That(MathUtils.ErfInv(5)).IsEqualTo(3.1232057f);
        }

        [Test]
        public void IsPowerOfTwoTest()
        {
            Check.That(MathUtils.IsPowerOf2(123)).IsFalse();
            Check.That(MathUtils.IsPowerOf2(0)).IsFalse();
            Check.That(MathUtils.IsPowerOf2(2)).IsTrue();
            Check.That(MathUtils.IsPowerOf2(4)).IsTrue();
            Check.That(MathUtils.IsPowerOf2(3)).IsFalse();
        }

        [Test]
        public void RoundUpPow2Test()
        {
            Check.That(MathUtils.RoundUpPow2(0)).IsEqualTo(0);
            Check.That(MathUtils.RoundUpPow2(1)).IsEqualTo(1);
            Check.That(MathUtils.RoundUpPow2(2)).IsEqualTo(2);
            Check.That(MathUtils.RoundUpPow2(3)).IsEqualTo(4);
            Check.That(MathUtils.RoundUpPow2(4)).IsEqualTo(4);
            Check.That(MathUtils.RoundUpPow2(5)).IsEqualTo(8);
            Check.That(MathUtils.RoundUpPow2(6)).IsEqualTo(8);
            Check.That(MathUtils.RoundUpPow2(7)).IsEqualTo(8);
            Check.That(MathUtils.RoundUpPow2(8)).IsEqualTo(8);
            Check.That(MathUtils.RoundUpPow2(9)).IsEqualTo(16);
            Check.That(MathUtils.RoundUpPow2(10)).IsEqualTo(16);
            Check.That(MathUtils.RoundUpPow2(11)).IsEqualTo(16);
            Check.That(MathUtils.RoundUpPow2(12)).IsEqualTo(16);
        }

        [Test]
        public void Log2IntTest()
        {
            Check.That(MathUtils.Log2Int(0)).IsEqualTo(0);
            Check.That(MathUtils.Log2Int(1)).IsEqualTo(1);
            Check.That(MathUtils.Log2Int(2)).IsEqualTo(2);
            Check.That(MathUtils.Log2Int(3)).IsEqualTo(2);
            Check.That(MathUtils.Log2Int(4)).IsEqualTo(3);
            Check.That(MathUtils.Log2Int(5)).IsEqualTo(3);
            Check.That(MathUtils.Log2Int(6)).IsEqualTo(3);
            Check.That(MathUtils.Log2Int(7)).IsEqualTo(3);
            Check.That(MathUtils.Log2Int(8)).IsEqualTo(4);
            Check.That(MathUtils.Log2Int(9)).IsEqualTo(4);
            Check.That(MathUtils.Log2Int(10)).IsEqualTo(4);
            Check.That(MathUtils.Log2Int(11)).IsEqualTo(4);
            Check.That(MathUtils.Log2Int(12)).IsEqualTo(4);
        }
    }
}