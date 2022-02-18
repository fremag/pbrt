using System;

namespace pbrt.Core
{
    public static class MathUtils
    {
        public static float Gamma(int n)
        {
            return (n * float.Epsilon) / (1 - n * float.Epsilon);
        }

        public static float Clamp(this float val, float low, float high)
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
    }
}