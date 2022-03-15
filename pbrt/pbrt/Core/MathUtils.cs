using System;

namespace pbrt.Core
{
    public static class MathUtils
    {
        public static float Lerp(float t, float x1, float x2) => (1 - t) * x1 + t * x2;

        public static float Gamma(int n)
        {
            return (n * float.Epsilon) / (1 - n * float.Epsilon);
        }

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
        
        public static float NextFloatUp(float v) {
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

        public static float NextFloatDown(float v) {
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
        
        public static double NextDoubleUp(double v, ulong delta = 1) {
            if (double.IsInfinity(v) && v > 0d) return v;
            if (v == -0d) v = 0d;
            ulong ul = DoubleToBits(v);
            if (v >= 0d)
                ul += delta;
            else
                ul -= delta;
            return BitsToDouble(ul);
        }

        public static  double NextDoubleDown(double v, ulong delta = 1) {
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
    }
}