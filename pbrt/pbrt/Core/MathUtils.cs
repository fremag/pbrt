using System;

namespace pbrt.Core
{
    public static class MathUtils
    {
        public static float UniformHemispherePdf => 1/(2*MathF.PI);
        public static float UniformSpherePdf => 1/(4*MathF.PI);
        public static float Inv4PI => 1/(4*MathF.PI);

        public static float Lerp(float t, float x1, float x2) => (1 - t) * x1 + t * x2;
        public static readonly float MachineEpsilon = 1.19209e-07f; // https://en.cppreference.com/w/cpp/types/climits

        public static float Gamma(int n) => n * MachineEpsilon / (1 - n * MachineEpsilon);

        public static float Clamp(this float val, float low, float high)
        {
            if (val < low) return low;
            return val > high ? high : val;
        }

        public static int Clamp(this int val, int low, int high)
        {
            if (val < low) return low;
            return val > high ? high : val;
        }

        public static float Radians(this float deg)
        {
            return (MathF.PI / 180) * deg;
        }

        public static float Degrees(this float rad)
        {
            return (180 / MathF.PI) * rad;
        }

        public static bool Quadratic(float a, float b, float c, out float t0, out float t1)
        {
            double discrim = (double)b * (double)b - 4 * (double)a * (double)c;
            if (discrim < 0)
            {
                t0 = 0;
                t1 = 0;
                return false;
            }

            double rootDiscrim = Math.Sqrt(discrim);
            double q;
            if (b < 0)
            {
                q = -.5 * (b - rootDiscrim);
            }
            else
            {
                q = -.5 * (b + rootDiscrim);
            }

            t0 = (float)(q / a);
            t1 = (float)(c / q);

            if (t0 > t1)
            {
                (t0, t1) = (t1, t0);
            }

            return true;
        }
        
        public static unsafe uint FloatToBits(float f)
        {
            float* refFloat = &f;
            void* refVoid = refFloat;
            uint* ui = (uint*)refVoid;
            return *ui;
        }
        
        public static unsafe float BitsToFloat(uint ui)
        {
            uint* refUint = &ui;
            void* refVoid = refUint;
            float* refFloat = (float*)refVoid;
            return *refFloat;
        }
        
        public static unsafe ulong DoubleToBits(double d)
        {
            double* refDouble = &d;
            void* refVoid = refDouble;
            ulong* ul = (ulong*)refVoid;
            return *ul;
        }
        
        public static unsafe double BitsToDouble(ulong ul)
        {
            ulong* refUlong = &ul;
            void* refVoid = refUlong;
            double* refDouble = (double*)refVoid;
            return *refDouble;
        }

        public static float NextFloatUp(float v) => MathF.BitIncrement(v);
        public static float NextFloatDown(float v) => MathF.BitDecrement(v); 

        public static float _NextFloatUp(float v) 
        {
            // Handle infinity and negative zero for _NextFloatUp()_
            if (float.IsInfinity(v) && v > 0f) return v;
            if (v == -0f) v = 0f;

            // Advance _v_ to next higher float
            uint ui = FloatToBits(v);
            if (v >= 0)
                ++ui;
            else
                --ui;
            return BitsToFloat(ui);
        }

        public static float _NextFloatDown(float v) 
        {
            // Handle infinity and positive zero for _NextFloatDown()_
            if (float.IsInfinity(v) && v < 0f) return v;
            if (v == 0f) v = -0f;
            uint ui = FloatToBits(v);
            if (v > 0)
                --ui;
            else
                ++ui;
            return BitsToFloat(ui);
        }
        
        public static double NextDoubleUp(double v, ulong delta = 1) 
        {
            if (double.IsInfinity(v) && v > 0d) return v;
            if (v == -0d) v = 0d;
            ulong ul = DoubleToBits(v);
            if (v >= 0d)
                ul += delta;
            else
                ul -= delta;
            return BitsToDouble(ul);
        }

        public static  double NextDoubleDown(double v, ulong delta = 1) 
        {
            if (double.IsInfinity(v) && v < 0) return v;
            if (v == 0) v = -0d;
            ulong ul = DoubleToBits(v);
            if (v > 0)
                ul -= delta;
            else
                ul += delta;
            return BitsToDouble(ul);
        }
        
        public static Vector3F SphericalDirection(float sinTheta, float cosTheta, float phi)
        {
            var vx = sinTheta * MathF.Cos(phi);
            var vy = sinTheta * MathF.Sin(phi);
            var vz = cosTheta;
            return new Vector3F(vx, vy, vz);
        }
        
        public static Vector3F SphericalDirection(float sinTheta, float cosTheta, float phi, Vector3F x, Vector3F y, Vector3F z) 
        {
            return sinTheta * MathF.Cos(phi) * x + sinTheta * MathF.Sin(phi) * y + cosTheta * z;
        }  
        
        public static float SphericalTheta(Vector3F v) => MathF.Acos(v.Z.Clamp(-1, 1));
        
        public static float SphericalPhi(Vector3F v) 
        {
            float p = MathF.Atan2(v.Y, v.X);
            return (p < 0) ? (p + 2 * MathF.PI) : p;
        }
        
        public static Point2F ConcentricSampleDisk(Point2F u)
        {
            // Map uniform random numbers to [-1, 1] x [-1, 1]
            Point2F uOffset = new Point2F(2f * u.X - 1, 2f * u.Y - 1);

            // Handle degeneracy at the origin 
            if (uOffset.X == 0 && uOffset.Y == 0)
            {
                return new Point2F(0, 0);
            }

            // Apply concentric mapping to point 
            float theta, r;
            if (MathF.Abs(uOffset.X) > MathF.Abs(uOffset.Y))
            {
                r = uOffset.X;
                theta = MathF.PI / 4 * (uOffset.Y / uOffset.X);
            }
            else
            {
                r = uOffset.Y;
                theta = MathF.PI / 2 - MathF.PI / 4 * (uOffset.X / uOffset.Y);
            }

            return r * new Point2F(MathF.Cos(theta), MathF.Sin(theta));
        }
                           
        public static bool SolveLinearSystem2X2(float[][] a, float[] b, out float x0, out float x1) 
        {
            float det = a[0][0] * a[1][1] - a[0][1] * a[1][0];
            if (MathF.Abs(det) < 1e-10f)
            {
                x0 = 0;
                x1 = 0;
                return false;
            }

            x0 = (a[1][1] * b[0] - a[0][1] * b[1]) / det;
            x1 = (a[0][0] * b[1] - a[1][0] * b[0]) / det;
            return !float.IsNaN(x0) && !float.IsNaN(x1);
        }
        
        public static Point2F RejectionSampleDisk(Random rng)
        {
            float x;
            float y;
            do {
                x = 1 - 2 * (float)rng.NextDouble();
                y = 1 - 2 * (float)rng.NextDouble();
            } while (x * x + y * y > 1);
            return new Point2F(x, y);
        }
        
        public static Vector3F UniformSampleHemisphere(Point2F u) {
            float z = u[0];
            float r = MathF.Sqrt(MathF.Max(0f, 1 - z * z));
            float phi = 2 * MathF.PI * u[1];
            var x = r * MathF.Cos(phi);
            var y = r * MathF.Sin(phi);
            return new Vector3F(x, y, z);
        }
        
        public static Vector3F UniformSampleSphere(Point2F u) 
        {
            float z = 1 - 2 * u[0];
            float r = MathF.Sqrt(MathF.Max(0f, 1 - z * z));
            float phi = 2 * MathF.PI * u[1];
            var x = r * MathF.Cos(phi);
            var y = r * MathF.Sin(phi);
            return new Vector3F(x, y, z);
        }
        
        public static Point2F UniformSampleDisk(Point2F u) 
        {
            float r = MathF.Sqrt(u[0]);
            float theta = 2 * MathF.PI * u[1];
            var x = r * MathF.Cos(theta);
            var y = r * MathF.Sin(theta);
            return new Point2F(x, y);
        }        
        
        public static Vector3F CosineSampleHemisphere(Point2F u) 
        {
            Point2F d = ConcentricSampleDisk(u);
            float z = MathF.Sqrt(MathF.Max(0f, 1 - d.X * d.X - d.Y * d.Y));
            return new Vector3F(d.X, d.Y, z);
        }
        
        public static float CosineHemispherePdf(float cosTheta) => cosTheta / MathF.PI;
        public static float UniformConePdf(float cosThetaMax) => 1 / (2 * MathF.PI * (1 - cosThetaMax));
        
        public static Vector3F UniformSampleCone(Point2F u, float cosThetaMax) 
        {
            float cosTheta = (1f - u[0]) + u[0] * cosThetaMax;
            float sinTheta = MathF.Sqrt(1f - cosTheta * cosTheta);
            float phi = u[1] * 2 * MathF.PI;
            var x = MathF.Cos(phi) * sinTheta;
            var y = MathF.Sin(phi) * sinTheta;
            return new Vector3F(x, y, cosTheta);
        }
        
        public static Point2F UniformSampleTriangle(Point2F u) 
        {
            float su0 = MathF.Sqrt(u[0]);
            return new Point2F(1 - su0, u[1] * su0);
        }
        
        public static float PowerHeuristic(int nf, float fPdf, int ng, float gPdf) 
        {
            float f = nf * fPdf;
            float g = ng * gPdf;
            var ff = f * f;
            return ff / (ff + g * g);
        }   
        
        public static float BalanceHeuristic(int nf, float fPdf, int ng, float gPdf) 
        {
            return (nf * fPdf) / (nf * fPdf + ng * gPdf);
        }
        

        public static float ErfInv(float x) {
            float w, p;
            x = Clamp(x, -.99999f, .99999f);
            w = -MathF.Log((1 - x) * (1 + x));
            if (w < 5) {
                w = w - 2.5f;
                p = 2.81022636e-08f;
                p = 3.43273939e-07f + p * w;
                p = -3.5233877e-06f + p * w;
                p = -4.39150654e-06f + p * w;
                p = 0.00021858087f + p * w;
                p = -0.00125372503f + p * w;
                p = -0.00417768164f + p * w;
                p = 0.246640727f + p * w;
                p = 1.50140941f + p * w;
            }
            else 
            {
                w = MathF.Sqrt(w) - 3;
                p = -0.000200214257f;
                p = 0.000100950558f + p * w;
                p = 0.00134934322f + p * w;
                p = -0.00367342844f + p * w;
                p = 0.00573950773f + p * w;
                p = -0.0076224613f + p * w;
                p = 0.00943887047f + p * w;
                p = 1.00167406f + p * w;
                p = 2.83297682f + p * w;
            }
            return p * x;
        }

        public static float Erf(float x) 
        {
            // constants
            float a1 = 0.254829592f;
            float a2 = -0.284496736f;
            float a3 = 1.421413741f;
            float a4 = -1.453152027f;
            float a5 = 1.061405429f;
            float p = 0.3275911f;

            // Save the sign of x
            int sign = 1;
            if (x < 0)
            {
                sign = -1;
            }

            x = MathF.Abs(x);

            // A&S formula 7.1.26
            float t = 1 / (1 + p * x);
            float y = 1 - (((((a5 * t + a4) * t) + a3) * t + a2) * t + a1) * t * MathF.Exp(-x * x);

            return sign * y;
        }
        
        public static void Shuffle<T>(T[] samp, int start, int count, int nDimensions, Random rng) 
        {
            for (int i = start; i < count; ++i) 
            {
                int other = i + rng.Next(count - i);
                for (int j = 0; j < nDimensions; ++j)
                {
                    var item1 = samp[nDimensions * i + j];
                    var item2 = samp[nDimensions * other + j];
                    samp[nDimensions * i + j] = item2;
                    samp[nDimensions * other + j] = item1;
                }
            }
        }

        public static bool IsPowerOf2(long x) => (x != 0) && ((x & (x - 1)) == 0);
        
        public static int RoundUpPow2(int v) {
            v--;
            v |= v >> 1;    v |= v >> 2;
            v |= v >> 4;    v |= v >> 8;
            v |= v >> 16;
            return v+1;
        }

        public static int Mod(int a, int b)
        {
            var result = a - (a/b) * b;
            return result < 0 ? result + b : result;
        }
    }
}