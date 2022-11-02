using System;
using System.Collections.Generic;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Textures;

public enum ImageWrap
{
    Repeat,
    Black,
    Clamp
}

public class ResampleWeight
{
    public int FirstTexel { get; set; }
    public float[] Weight { get; } = new float[4];
    public float SumWeight => Weight[0] + Weight[1] + Weight[2] + Weight[3];
};

public class MipMap
{
    public Point2I Resolution { get; private set; }
    public RgbSpectrum[] Data { get; }
    public bool DoTrilinear { get; }
    public float MaxAniso { get; }
    public ImageWrap WrapMode { get; }

    public int Width => Resolution[0];
    public int Height => Resolution[1];
    public int Levels => Pyramid?.Count ?? 0;

    public List<BlockedArray<RgbSpectrum>> Pyramid { get; } = new List<BlockedArray<RgbSpectrum>>();
    public static int WeightLutSize => 128;
    public static float[] WeightLut { get; } = new float[WeightLutSize];
    public RgbSpectrum[] ResampledImage { get; private set; }

    public MipMap(Point2I resolution, RgbSpectrum[] data, bool doTrilinear = false, float maxAniso = 8f, ImageWrap wrapMode = ImageWrap.Repeat)
    {
        Resolution = resolution;
        Data = data;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        WrapMode = wrapMode;

        ResampleImage();
        // Initialize levels of MIPMap from image
        int nLevels = 1 + MathUtils.Log2Int(Math.Max(resolution[0], resolution[1]));
        // Initialize most detailed level of MIPMap
        Pyramid.Add(new BlockedArray<RgbSpectrum>(resolution[0], resolution[1], ResampledImage ?? data));
        for (int i = 1; i < nLevels; ++i)
        {
            // Initialize i th MIPMap level from i-1 st level
            int sRes = Math.Max(1, Pyramid[i - 1].USize() / 2);
            int tRes = Math.Max(1, Pyramid[i - 1].VSize() / 2);

            Pyramid.Add(new BlockedArray<RgbSpectrum>(sRes, tRes));
            var blockedArray = Pyramid[i];
            for (int t = 0; t < tRes; t++)
            {
                for (int s = 0; s < sRes; s++)
                {
                    var rgbSpectrum1 = Texel(i - 1, 2 * s, 2 * t);
                    var rgbSpectrum2 = Texel(i - 1, 2 * s + 1, 2 * t);
                    var rgbSpectrum3 = Texel(i - 1, 2 * s, 2 * t + 1);
                    var rgbSpectrum4 = Texel(i - 1, 2 * s + 1, 2 * t + 1);
                    var r = .25f * (rgbSpectrum1.C[0] + rgbSpectrum2.C[0] + rgbSpectrum3.C[0] + rgbSpectrum4.C[0]);
                    var g = .25f * (rgbSpectrum1.C[1] + rgbSpectrum2.C[1] + rgbSpectrum3.C[1] + rgbSpectrum4.C[1]);
                    var b = .25f * (rgbSpectrum1.C[2] + rgbSpectrum2.C[2] + rgbSpectrum3.C[2] + rgbSpectrum4.C[2]);
                    blockedArray[s, t] = new RgbSpectrum(new [] {r, g, b});
                }
            }
        }
        
        // Initialize EWA filter weights if needed                 
    }

    public RgbSpectrum Texel(int level, int s, int t)
    {
        var blockedArray = Pyramid[level];
        // Compute texel (s, t) accounting for boundary conditions 
        switch (WrapMode)
        {
            case ImageWrap.Repeat:
                s = MathUtils.Mod(s, blockedArray.USize());
                t = MathUtils.Mod(t, blockedArray.VSize());
                break;
            
            case ImageWrap.Clamp:
                s = s.Clamp(0, blockedArray.USize() - 1);
                t = t.Clamp(0, blockedArray.VSize() - 1);
                break;
            
            case ImageWrap.Black:
            {
                if (s < 0 || s >= blockedArray.USize() || t < 0 || t >= blockedArray.VSize())
                {
                    RgbSpectrum black = new(0f);
                    return black;
                }

                break;
            }
        }

        return blockedArray[s, t];
    }

    private void ResampleImage()
    {
        if (MathUtils.IsPowerOf2(Resolution[0]) && MathUtils.IsPowerOf2(Resolution[1]))
        {
            return;
        }

        // Resample image to power-of-two resolution
        var resPow2 = new Point2I(MathUtils.RoundUpPow2(Resolution[0]), MathUtils.RoundUpPow2(Resolution[1]));
        // Resample image in s direction
        ResampleWeight[] sWeights = ResampleWeights(Resolution[0], resPow2[0]);
        ResampledImage = new RgbSpectrum[resPow2[0] * resPow2[1]];

        // Apply sWeights to zoom in s direction 
        for (int t = 0; t < Resolution[1]; t++)
        {
            for (int s = 0; s < resPow2[0]; ++s)
            {
                // Compute texel (s, t) in s-zoomed image
                var index = t * resPow2[0] + s;
                ResampledImage[index] = new RgbSpectrum();
                for (int j = 0; j < 4; ++j)
                {
                    int origS = sWeights[s].FirstTexel + j;
                    switch (WrapMode)
                    {
                        case ImageWrap.Repeat:
                            origS = MathUtils.Mod(origS, Resolution[0]);
                            break;
                        case ImageWrap.Clamp:
                            origS = origS.Clamp(0, Resolution[0] - 1);
                            break;
                    }

                    if (origS >= 0 && origS < Resolution[0])
                    {
                        var f = sWeights[s].Weight[j];
                        RgbSpectrum rgbSpectrum = Data[t * Resolution[0] + origS];
                        ResampledImage[index].AddMul(f, rgbSpectrum);
                    }
                }
            }
        }


        // Resample image in t direction 
        ResampleWeight[] tWeights = ResampleWeights(Resolution[1], resPow2[1]);
        for (int s = 0; s < resPow2[0]; s++)
        {
            RgbSpectrum[] workData = new RgbSpectrum[resPow2[1]];
            for (int t = 0; t < resPow2[1]; ++t)
            {
                workData[t] = new RgbSpectrum();
                for (int j = 0; j < 4; ++j)
                {
                    int offset = tWeights[t].FirstTexel + j;
                    switch (WrapMode)
                    {
                        case ImageWrap.Repeat:
                            offset = MathUtils.Mod(offset, Resolution[1]);
                            break;
                        case ImageWrap.Clamp:
                            offset = offset.Clamp(0, Resolution[1] - 1);
                            break;
                    }

                    if (offset >= 0 && offset < (int)Resolution[1])
                    {
                        workData[t].AddMul(tWeights[t].Weight[j], ResampledImage[offset * resPow2[0] + s]);
                    }
                }
            }

            for (int t = 0; t < resPow2[1]; ++t)
            {
                workData[t].Clamp();
                ResampledImage[t * resPow2[0] + s] = workData[t];
            }
        }

        Resolution = resPow2;
    }

    public ResampleWeight[] ResampleWeights(int oldRes, int newRes)
    {
        ResampleWeight[] wt = new ResampleWeight[newRes];
        float filterwidth = 2f;
        for (int i = 0; i < newRes; ++i)
        {
            // Compute image resampling weights for ith texel
            float center = (i + .5f) * oldRes / newRes;
            wt[i] = new ResampleWeight
            {
                FirstTexel = (int)MathF.Floor(center - filterwidth + 0.5f)
            };

            for (int j = 0; j < 4; ++j)
            {
                float pos = wt[i].FirstTexel + j + .5f;
                wt[i].Weight[j] = Lanczos((pos - center) / filterwidth);
            }

            // Normalize filter weights for texel resampling
            float invSumWts = 1 / (wt[i].SumWeight);
            for (int j = 0; j < 4; ++j)
            {
                wt[i].Weight[j] *= invSumWts;
            }
        }

        return wt;
    }

    public static float Lanczos(float x)
    {
        x = MathF.Abs(x);
        if (x < 1e-5)
        {
            return 1;
        }

        return MathF.Sin(MathF.PI * x) / (MathF.PI * x);
    }
    
    public RgbSpectrum Lookup(Point2F st, float width) 
    {
        // Compute MIPMap level for trilinear filtering 
        float level = Levels - 1 + MathF.Log2(MathF.Max(width, 1e-8f));
        
        // Perform trilinear interpolation at appropriate MIPMap level 
        if (level < 0)
        {
            return Triangle(0, st);
        }

        if (level >= Levels - 1)
        {
            return Texel(Levels - 1, 0, 0);
        }

        int iLevel = (int)MathF.Floor(level);
        float delta = level - iLevel;
        var rgbSpectrum = new RgbSpectrum();
        rgbSpectrum.AddMul(1-delta, Triangle(iLevel, st));
        rgbSpectrum.AddMul(delta, Triangle(iLevel + 1, st));
        return rgbSpectrum;
    }
    
    public RgbSpectrum Triangle(int level, Point2F st) 
    {
        level = level.Clamp(0, Levels - 1);
        float s = st[0] * Pyramid[level].USize() - 0.5f;
        float t = st[1] * Pyramid[level].VSize() - 0.5f;
        int s0 = (int)MathF.Floor(s);
        int t0 = (int)MathF.Floor(t);
        
        float ds = s - s0, dt = t - t0;
        var rgbSpectrum = new RgbSpectrum();
        rgbSpectrum.AddMul((1 - ds) * (1 - dt), Texel(level, s0, t0));
        rgbSpectrum.AddMul((1 - ds) * dt, Texel(level, s0, t0 + 1));
        rgbSpectrum.AddMul(ds * (1 - dt), Texel(level, s0 + 1, t0));
        rgbSpectrum.AddMul(ds * dt      , Texel(level, s0+1, t0+1));
        return rgbSpectrum;
    }
    
    public RgbSpectrum Lookup(Point2F st, Vector2F dst0, Vector2F dst1)
    {
        if (!DoTrilinear)
        {
            throw new NotImplementedException("Elliptically Weighted Average not implemented yet !");
        }

        var maxDst0 = MathF.Max(MathF.Abs(dst0[0]), MathF.Abs(dst0[1]));
        var maxDst1 = MathF.Max(MathF.Abs(dst1[0]), MathF.Abs(dst1[1]));
        float width = MathF.Max(maxDst0, maxDst1);
        return Lookup(st, 2 * width);
    }    
}