using System;

namespace pbrt.Core
{
    public class EFloat
    {
        public float V { get; }
        public float Low { get; }
        public float High { get; }

        private EFloat(float v, float high, float low)
        {
            V = v;
            High = high;
            Low = low;
        }

        public EFloat()
        {
        }

        public EFloat(EFloat ef)
        {
            ef.Check();
            V = ef.V;
            Low = ef.Low;
            High = ef.High;
        }
        
        public EFloat(float v, float err = 0f)
        {
            this.V = v;
            if (err == 0f)
                Low = High = v;
            else
            {
                // Compute conservative bounds by rounding the endpoints away
                // from the middle. Note that this will be over-conservative in
                // cases where v-err or v+err are exactly representable in
                // floating-point, but it's probably not worth the trouble of
                // checking this case.
                Low = MathUtils.NextFloatDown(v - err);
                High = MathUtils.NextFloatUp(v + err);
            }
        }

        public float UpperBound() => High;
        public float LowerBound() => Low;
        public float GetAbsoluteError() => MathUtils.NextFloatUp(MathF.Max(MathF.Abs(High - V), MathF.Abs(V - Low)));

        public static explicit operator float(EFloat ef) => ef.V;
        public static explicit operator double(EFloat ef) => ef.V;
        
        public static EFloat operator +(EFloat ef1, EFloat ef2)
        {
            var v = ef1.V + ef2.V;
            // Interval arithmetic addition, with the result rounded away from
            // the value r.v in order to be conservative.
            var low = MathUtils.NextFloatDown(ef1.LowerBound() + ef2.LowerBound());
            var high = MathUtils.NextFloatUp(ef1.UpperBound() + ef2.UpperBound());
            EFloat r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static EFloat operator -(EFloat ef1, EFloat ef2)
        {
            var v = ef1.V - ef2.V;
            var low = MathUtils.NextFloatDown(ef1.LowerBound() - ef2.UpperBound());
            var high = MathUtils.NextFloatUp(ef1.UpperBound() - ef2.LowerBound());
            EFloat r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static EFloat operator *(EFloat ef1, EFloat ef2)
        {
            var v = ef1.V * ef2.V;
            float[] prod =
            {
                ef1.LowerBound() * ef2.LowerBound(), ef1.UpperBound() * ef2.LowerBound(),
                ef1.LowerBound() * ef2.UpperBound(), ef1.UpperBound() * ef2.UpperBound()
            };

            var low = MathUtils.NextFloatDown(
                MathF.Min(MathF.Min(prod[0], prod[1]), MathF.Min(prod[2], prod[3])));
            var high = MathUtils.NextFloatUp(
                MathF.Max(MathF.Max(prod[0], prod[1]), MathF.Max(prod[2], prod[3])));
            var r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static EFloat operator /(EFloat ef1, EFloat ef2)
        {
            var v = ef1.V / ef2.V;
            float high, low;

            if (ef2.Low < 0 && ef2.High > 0)
            {
                // Bah. The interval we're dividing by straddles zero, so just
                // return an interval of everything.
                low = float.NegativeInfinity;
                high = float.PositiveInfinity;
            }
            else
            {
                float[] div =
                {
                    ef1.LowerBound() / ef2.LowerBound(), ef1.UpperBound() / ef2.LowerBound(),
                    ef1.LowerBound() / ef2.UpperBound(), ef1.UpperBound() / ef2.UpperBound()
                };
                low = MathUtils.NextFloatDown(
                    MathF.Min(MathF.Min(div[0], div[1]), MathF.Min(div[2], div[3])));
                high = MathUtils.NextFloatUp(
                    MathF.Max(MathF.Max(div[0], div[1]), MathF.Max(div[2], div[3])));
            }

            var r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static EFloat operator -(EFloat ef)
        {
            EFloat r = new EFloat(-ef.V, -ef.High, -ef.Low);
            r.Check();
            return r;
        }

        public static bool operator !=(EFloat ef1, EFloat ef2) => !(ef1 == ef2);
        public static bool operator ==(EFloat ef1, EFloat ef2)
        {
            if (ef1 == null && ef2 != null) return false;
            if (ef1 != null && ef2 == null) return false;
            if (ef1 == ef2) return true;
            return Math.Abs(ef1.V - ef2.V) < float.Epsilon;
        }

        public void Check()
        {
            if (!float.IsInfinity(Low) && !float.IsNaN(Low) && !float.IsInfinity(High) && !float.IsNaN(High))
            {
                if (Low > High)
                {
                    throw new Exception();
                }
            }
        }

        public static EFloat operator *(float f, EFloat fe) => new EFloat(f) * fe;
        public static EFloat operator /(float f, EFloat fe) => new EFloat(f) / fe;
        public static EFloat operator +(float f, EFloat fe) => new EFloat(f) + fe;
        public static EFloat operator -(float f, EFloat fe) => new EFloat(f) - fe;
        public static EFloat operator *(EFloat fe, float f) => new EFloat(f) * fe;
        public static EFloat operator /(EFloat fe, float f) => new EFloat(f) / fe;
        public static EFloat operator +(EFloat fe, float f) => new EFloat(f) + fe;
        public static EFloat operator -(EFloat fe, float f) => new EFloat(f) - fe;

        public static EFloat Sqrt(EFloat fe)
        {
            var v = MathF.Sqrt(fe.V);
            var low = MathUtils.NextFloatDown(MathF.Sqrt(fe.Low));
            var high = MathUtils.NextFloatUp(MathF.Sqrt(fe.High));
            EFloat r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static EFloat Abs(EFloat fe)
        {
            if (fe.Low >= 0)
                // The entire interval is greater than zero, so we're all set.
                return fe;
            float v, high, low;
            if (fe.High <= 0)
            {
                // The entire interval is less than zero.
                v = -fe.V;
                low = -fe.High;
                high = -fe.Low;
            }
            else
            {
                // The interval straddles zero.
                v = MathF.Abs(fe.V);
                low = 0;
                high = MathF.Max(-fe.Low, fe.High);
            }

            EFloat r = new EFloat(v, high, low);
            r.Check();
            return r;
        }

        public static bool Quadratic(EFloat efA, EFloat efB, EFloat efC, out EFloat t0, out EFloat t1)
        {
            // Find quadratic discriminant
            double discrim = (double)efB.V * (double)efB.V - 4 * (double)efA.V * (double)efC.V;
            if (discrim < 0)
            {
                t0 = null;
                t1 = null;
                return false;
            }

            double rootDiscrim = Math.Sqrt(discrim);

            EFloat floatRootDiscrim = new EFloat((float)rootDiscrim, (float)(float.Epsilon * rootDiscrim));

            // Compute quadratic _t_ values
            EFloat q;
            if ((float)efB < 0)
                q = new EFloat(-.5f * (efB - floatRootDiscrim));
            else
                q = new EFloat(-.5f * (efB + floatRootDiscrim));
            t0 = q / efA;
            t1 = efC / q;
            if ((float)t0 > (float)t1)
            {
                (t0, t1) = (t1, t0);
            }

            return true;
        }
        
        protected bool Equals(EFloat other)
        {
            return V.Equals(other.V) && Low.Equals(other.Low) && High.Equals(other.High);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EFloat)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(V, Low, High);
        }
   }
}