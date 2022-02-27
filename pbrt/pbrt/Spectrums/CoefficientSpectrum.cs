using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using pbrt.Core;

namespace pbrt.Spectrums
{
    public class CoefficientSpectrum
    {
        public int NSpectrumSamples { get; }
        public float[] C { get; }

        public CoefficientSpectrum(float[] values)
        {
            NSpectrumSamples = values.Length;
            C = new float[NSpectrumSamples];
            Array.Copy(values, C, NSpectrumSamples);
        }
        
        public CoefficientSpectrum(int nSpectrumSamples, float v)
        {
            NSpectrumSamples = nSpectrumSamples;
            C = Enumerable.Range(0, nSpectrumSamples).Select(_ => v).ToArray();
        }

        public float this[int i] => C[i];
        public bool IsBlack() => C.All(c => c == 0);

        public CoefficientSpectrum(CoefficientSpectrum cs) : this(cs.NSpectrumSamples, 0)
        {
            Add(cs);
        }

        public void Add(CoefficientSpectrum s2)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] += s2.C[i];
            }
        }

        public void Sub(CoefficientSpectrum s2)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] -= s2.C[i];
            }
        }

        public void Mul(CoefficientSpectrum s2)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] *= s2.C[i];
            }
        }

        public void Div(CoefficientSpectrum s2)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] /= s2.C[i];
            }
        }

        public void Mul(float a)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] *= a;
            }
        }

        public void Div(float a)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] /= a;
            }
        }

        public void Neg()
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] = -C[i];
            }
        }

        public void Sqrt()
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] = MathF.Sqrt(C[i]);
            }
        }

        public void Clamp(float low = 0, float high = float.PositiveInfinity)
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                C[i] = C[i].Clamp(low, high);
            }
        }

        public bool HasNaNs()
        {
            for (var i = 0; i < NSpectrumSamples; ++i)
            {
                if( float.IsNaN(C[i]))
                {
                    return true;
                }
            }

            return false;
        }

        protected bool Equals(CoefficientSpectrum other)
        {
            if (NSpectrumSamples != other.NSpectrumSamples)
            {
                return false;
            }

            for (var i = 0; i < NSpectrumSamples; i++)
            {
                if (Math.Abs(C[i] - other.C[i]) > float.Epsilon)
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CoefficientSpectrum)obj);
        }

        public override int GetHashCode() => HashCode.Combine(NSpectrumSamples, ((IStructuralEquatable)C).GetHashCode(EqualityComparer<float>.Default));

        public override string ToString() => $"[{string.Join(", ", C)}]";
    }
}