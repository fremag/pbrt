using System;
using System.Collections.Generic;
using pbrt.Core;

namespace pbrt.Spectrums
{
    public enum SpectrumType
    {
        Reflectance,
        Illuminant
    };

    public class SampledSpectrum : CoefficientSpectrum
    {
        public static readonly int SampledLambdaStart = 400; // nano meter
        public static readonly int SampledLambdaEnd = 700; // nano meter
        public const int NSpectralSamples = 60;

        public SampledSpectrum(int nSpectrumSamples = NSpectralSamples, float v = 0) : base(nSpectrumSamples, v)
        {
        }

        public SampledSpectrum(CoefficientSpectrum cs) : base(cs)
        {
        }

        public SampledSpectrum(float[] values) : base(values)
        {
        }

        public SampledSpectrum(RgbSpectrum r, SpectrumType t) : this(FromRgb(r.ToRgb(), t)) 
        {
        }
        
        public static bool SpectrumSamplesSorted(float[] lambdas, int n)
        {
            for (int i = n - 1 - 1; i >= 0; --i)
            {
                if (lambdas[i] > lambdas[i + 1])
                {
                    return false;
                }
            }

            return true;
        }

        public static void SortSpectrumSamples(float[] lambdas, float[] values, int n, out float[] sortedLambdas, out float[] sortedValues)
        {
            SortedList<float, float> sorted = new SortedList<float, float>();
            for (int i = 0; i < n; i++)
            {
                var lambda = lambdas[i];
                var value = values[i];
                sorted[lambda] = value;
            }

            sortedLambdas = new float[n];
            sortedValues = new float[n];

            for (int i = 0; i < n; i++)
            {
                sortedLambdas[i] = sorted.Keys[i];
                sortedValues[i] = sorted.Values[i];
            }
        }

        public static SampledSpectrum FromSampled(float[] lambdas, float[] values, int n)
        {
            if (!SpectrumSamplesSorted(lambdas, n))
            {
                SortSpectrumSamples(lambdas, values, n, out var sortedLambdas, out var sortedValues);
                return FromSampled(sortedLambdas, sortedValues, n);
            }

            SampledSpectrum r = new SampledSpectrum();
            for (int i = 0; i < NSpectralSamples; ++i)
            {
                // Compute average value of given SPD over ith sample’s range
                float lambda0 = MathUtils.Lerp(((float)i) / NSpectralSamples, SampledLambdaStart, SampledLambdaEnd);
                float lambda1 = MathUtils.Lerp((float)(i + 1) / NSpectralSamples, SampledLambdaStart, SampledLambdaEnd);
                r.C[i] = AverageSpectrumSamples(lambdas, values, n, lambda0, lambda1);
            }

            return r;
        }

        public static float AverageSpectrumSamples(float[] lambdas, float[] values, int n, float lambdaStart, float lambdaEnd)
        {
            // Handle cases with out-of-bounds range or single sample only
            if (lambdaEnd <= lambdas[0])
            {
                return values[0];
            }

            if (lambdaStart >= lambdas[n - 1])
            {
                return values[n - 1];
            }

            if (n == 1)
            {
                return values[0];
            }

            float sum = 0;
            // Add contributions of constant segments before/after samples
            if (lambdaStart < lambdas[0])
            {
                sum += values[0] * (lambdas[0] - lambdaStart);
            }

            if (lambdaEnd > lambdas[n - 1])
            {
                sum += values[n - 1] * (lambdaEnd - lambdas[n - 1]);
            }

            // Advance to first relevant wavelength segment 
            int i = 0;
            while (lambdaStart > lambdas[i + 1])
            {
                ++i;
            }

            // Loop over wavelength sample segments and add contributions
            float Interp(float w, int j)
            {
                return MathUtils.Lerp((w - lambdas[j]) / (lambdas[j + 1] - lambdas[j]), values[j], values[j + 1]);
            }

            for (; i + 1 < n && lambdaEnd >= lambdas[i]; i++)
            {
                float segLambdaStart = MathF.Max(lambdaStart, lambdas[i]);
                float segLambdaEnd = MathF.Min(lambdaEnd, lambdas[i + 1]);
                var interpValueStart = Interp(segLambdaStart, i);
                var interpValueEnd = Interp(segLambdaEnd, i);
                sum += 0.5f * (interpValueStart + interpValueEnd) * (segLambdaEnd - segLambdaStart);
            }

            return sum / (lambdaEnd - lambdaStart);
        }

        public float[] ToXYZ()
        {
            float[] xyz = new float[3];
            xyz[0] = 0;
            xyz[1] = 0;
            xyz[2] = 0;

            for (int i = 0; i < NSpectralSamples; ++i)
            {
                xyz[0] += XyzUtils.X.C[i] * C[i];
                xyz[1] += XyzUtils.Y.C[i] * C[i];
                xyz[2] += XyzUtils.Z.C[i] * C[i];
            }

            float scale = (SampledLambdaEnd - SampledLambdaStart) / (XyzUtils.CIE_Y_integral * NSpectralSamples);
            xyz[0] *= scale;
            xyz[1] *= scale;
            xyz[2] *= scale;
            return xyz;
        }

        public float Y()
        {
            float yy = 0f;
            for (int i = 0; i < NSpectralSamples; ++i)
            {
                yy += XyzUtils.Y.C[i] * C[i];
            }

            return yy * (SampledLambdaEnd - SampledLambdaStart) / NSpectralSamples;
        }

        public float[] ToRgb()
        {
            var xyz = ToXYZ();
            var rgb = SpectrumUtils.XYZToRGB(xyz);
            return rgb;
        }

        public RgbSpectrum ToRgbSpectrum()
        {
            var rgb = ToRgb();
            return RgbSpectrum.FromRGB(rgb);        }

        public static SampledSpectrum operator +(SampledSpectrum spec1, SampledSpectrum spec2)
        {
            var result = new SampledSpectrum(spec1);
            result.Add(spec2);
            return result;
        }

        public static SampledSpectrum operator *(float factor, SampledSpectrum spec)
        {
            var result = new SampledSpectrum(spec);
            result.Mul(factor);
            return result;
        }

        // https://github.com/mmp/pbrt-v3/blob/aaa552a4b9cbf9dccb71450f47b268e0ed6370e2/src/core/spectrum.cpp#L98
        public static SampledSpectrum FromRgb(float[] rgb, SpectrumType type)
        {
            SampledSpectrum r = new SampledSpectrum();
            switch (type)
            {
                case SpectrumType.Reflectance:
                {
                    // Convert reflectance spectrum to RGB 
                    if (rgb[0] <= rgb[1] && rgb[0] <= rgb[2])
                    {
                        // Compute reflectance SampledSpectrum with rgb[0] as minimum
                        r += rgb[0] * RgbUtils.rgbRefl2SpectWhite;
                        if (rgb[1] <= rgb[2])
                        {
                            r += (rgb[1] - rgb[0]) * RgbUtils.rgbRefl2SpectCyan;
                            r += (rgb[2] - rgb[1]) * RgbUtils.rgbRefl2SpectBlue;
                        }
                        else
                        {
                            r += (rgb[2] - rgb[0]) * RgbUtils.rgbRefl2SpectCyan;
                            r += (rgb[1] - rgb[2]) * RgbUtils.rgbRefl2SpectGreen;
                        }
                    }
                    else if (rgb[1] <= rgb[0] && rgb[1] <= rgb[2])
                    {
                        // Compute reflectance _SampledSpectrum_ with _rgb[1]_ as minimum
                        r += rgb[1] * RgbUtils.rgbRefl2SpectWhite;
                        if (rgb[0] <= rgb[2])
                        {
                            r += (rgb[0] - rgb[1]) * RgbUtils.rgbRefl2SpectMagenta;
                            r += (rgb[2] - rgb[0]) * RgbUtils.rgbRefl2SpectBlue;
                        }
                        else
                        {
                            r += (rgb[2] - rgb[1]) * RgbUtils.rgbRefl2SpectMagenta;
                            r += (rgb[0] - rgb[2]) * RgbUtils.rgbRefl2SpectRed;
                        }
                    }
                    else
                    {
                        // Compute reflectance _SampledSpectrum_ with _rgb[2]_ as minimum
                        r += rgb[2] * RgbUtils.rgbRefl2SpectWhite;
                        if (rgb[0] <= rgb[1])
                        {
                            r += (rgb[0] - rgb[2]) * RgbUtils.rgbRefl2SpectYellow;
                            r += (rgb[1] - rgb[0]) * RgbUtils.rgbRefl2SpectGreen;
                        }
                        else
                        {
                            r += (rgb[1] - rgb[2]) * RgbUtils.rgbRefl2SpectYellow;
                            r += (rgb[0] - rgb[1]) * RgbUtils.rgbRefl2SpectRed;
                        }
                    }

                    r.Mul(0.94f);
                    break;
                }
                case SpectrumType.Illuminant:
                    // Convert illuminant spectrum to RGB
                    if (rgb[0] <= rgb[1] && rgb[0] <= rgb[2])
                    {
                        // Compute illuminant _SampledSpectrum_ with _rgb[0]_ as minimum
                        r += rgb[0] * RgbUtils.rgbIllum2SpectWhite;
                        if (rgb[1] <= rgb[2])
                        {
                            r += (rgb[1] - rgb[0]) * RgbUtils.rgbIllum2SpectCyan;
                            r += (rgb[2] - rgb[1]) * RgbUtils.rgbIllum2SpectBlue;
                        }
                        else
                        {
                            r += (rgb[2] - rgb[0]) * RgbUtils.rgbIllum2SpectCyan;
                            r += (rgb[1] - rgb[2]) * RgbUtils.rgbIllum2SpectGreen;
                        }
                    }
                    else if (rgb[1] <= rgb[0] && rgb[1] <= rgb[2])
                    {
                        // Compute illuminant _SampledSpectrum_ with _rgb[1]_ as minimum
                        r += rgb[1] * RgbUtils.rgbIllum2SpectWhite;
                        if (rgb[0] <= rgb[2])
                        {
                            r += (rgb[0] - rgb[1]) * RgbUtils.rgbIllum2SpectMagenta;
                            r += (rgb[2] - rgb[0]) * RgbUtils.rgbIllum2SpectBlue;
                        }
                        else
                        {
                            r += (rgb[2] - rgb[1]) * RgbUtils.rgbIllum2SpectMagenta;
                            r += (rgb[0] - rgb[2]) * RgbUtils.rgbIllum2SpectRed;
                        }
                    }
                    else
                    {
                        // Compute illuminant _SampledSpectrum_ with _rgb[2]_ as minimum
                        r += rgb[2] * RgbUtils.rgbIllum2SpectWhite;
                        if (rgb[0] <= rgb[1])
                        {
                            r += (rgb[0] - rgb[2]) * RgbUtils.rgbIllum2SpectYellow;
                            r += (rgb[1] - rgb[0]) * RgbUtils.rgbIllum2SpectGreen;
                        }
                        else
                        {
                            r += (rgb[1] - rgb[2]) * RgbUtils.rgbIllum2SpectYellow;
                            r += (rgb[0] - rgb[1]) * RgbUtils.rgbIllum2SpectRed;
                        }
                    }

                    r.Mul(0.86445f);
                    break;
            }

            r.Clamp();
            return r;
        }
        
        public static SampledSpectrum FromXYZ(float[] xyz, SpectrumType type = SpectrumType.Reflectance)
        {
            var rgb = SpectrumUtils.XYZToRGB(xyz);
            var spec = FromRgb(rgb, type);
            return spec;
        }        
    }
}