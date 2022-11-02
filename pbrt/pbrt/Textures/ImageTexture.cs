using System;
using System.Collections.Generic;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Textures;

public class ImageTexture : Texture<Spectrum>
{
    public TextureMapping2D Mapping { get; private set; }
    public string Filename { get; private set; }
    public bool DoTrilinear { get; private set; }
    public float MaxAniso { get; private set; }
    public ImageWrap ImageWrap { get; private set; }
    public float Scale { get; private set; }
    public bool Gamma { get; private set; }

    public MipMap MipMap { get; private set; }
    
    static Dictionary<TexInfo, MipMap> MipMapCache;    
    public static void ClearCache() 
    {
        MipMapCache.Clear();
    }    

    public ImageTexture(TextureMapping2D mapping, string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        Mapping = mapping;
        Filename = filename;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        ImageWrap = imageWrap;
        Scale = scale;
        Gamma = gamma;
        
        MipMap = GetTexture(filename, doTrilinear, maxAniso, imageWrap, scale, gamma);        
    }

    private MipMap GetTexture(string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        TexInfo texInfo = new TexInfo(filename, doTrilinear, maxAniso, imageWrap, scale, gamma);
        if (!MipMapCache.TryGetValue(texInfo, out var mipMap))
        {
            // Read Image + Init MipMap
            
        }

        return mipMap;
    }

    public override Spectrum Evaluate(SurfaceInteraction si)
    {
        Point2F st = Mapping.Map(si, out var dstDx, out var dstDy);
        var rgbSpectrum = MipMap.Lookup(st, dstDx, dstDy);
        ConvertOut(rgbSpectrum, out var ret);
        return ret;        
    }

    private void ConvertOut(RgbSpectrum rgbSpectrum, out Spectrum spectrum)
    {
        var floats = rgbSpectrum.ToRgb();
        var sampledSpectrum = SampledSpectrum.FromRgb(floats);
        spectrum = new Spectrum(sampledSpectrum);
    }
}

internal class TexInfo
{
    private string Filename { get; }
    private bool DoTrilinear { get; }
    private float MaxAniso { get; }
    private ImageWrap ImageWrap { get; }
    private float Scale { get; }
    private bool Gamma { get; }

    public TexInfo(string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        Filename = filename;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        ImageWrap = imageWrap;
        Scale = scale;
        Gamma = gamma;
    }

    public bool Equals(TexInfo other)
    {
        return Filename == other.Filename && DoTrilinear == other.DoTrilinear && MaxAniso.Equals(other.MaxAniso) && ImageWrap == other.ImageWrap && Scale.Equals(other.Scale) && Gamma == other.Gamma;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TexInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Filename, DoTrilinear, MaxAniso, (int)ImageWrap, Scale, Gamma);
    }
}