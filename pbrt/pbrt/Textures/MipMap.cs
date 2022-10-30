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
    public bool DoTri { get; }
    public float MaxAniso { get; }
    public ImageWrap WrapMode { get; }

    public int Width => Resolution[0];
    public int Height => Resolution[1];
    public int Levels => Pyramid?.Count ?? 0;

    public List<RgbSpectrum> Pyramid;
    public static int WeightLUTSize => 128;
    public static float[] WeightLut { get; } = new float[WeightLUTSize];
    public RgbSpectrum[] ResampledImage { get; private set; }

    public MipMap(Point2I resolution, RgbSpectrum[] data, bool doTri = false, float maxAniso = 8f, ImageWrap wrapMode = ImageWrap.Repeat)
    {
        Resolution = resolution;
        Data = data;
        DoTri = doTri;
        MaxAniso = maxAniso;
        WrapMode = wrapMode;

        ResampleImage();
        // Initialize levels of MIPMap from image 
        // Initialize EWA filter weights if needed                 
    }

    private void ResampleImage()
    {
        if (!MathUtils.IsPowerOf2(Resolution[0]) || !MathUtils.IsPowerOf2(Resolution[1]))
        {
            // Resample image to power-of-two resolution
            var resPow2 = new Point2I(MathUtils.RoundUpPow2(Resolution[0]), MathUtils.RoundUpPow2(Resolution[1]));
            // Resample image in s direction
            ResampleWeight[] sWeights = ResampleWeights(Resolution[0], resPow2[0]);
            ResampledImage = new RgbSpectrum[resPow2[0] * resPow2[1]];
           
            // Apply sWeights to zoom in s direction 
            for(int t=0; t < Resolution[1]; t++) 
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
            for(int s = 0; s < resPow2[0]; s++)
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
                                offset = offset.Clamp(0, Resolution[1]-1);
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
}