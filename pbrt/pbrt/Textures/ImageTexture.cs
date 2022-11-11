using System.Collections.Generic;
using System.IO;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Textures;

public class ImageTexture : Texture<Spectrum>
{
    public TextureMapping2D Mapping { get; }
    public string Filename { get; }
    public bool DoTrilinear { get; }
    public float MaxAniso { get; }
    public ImageWrap ImageWrap { get; }
    public float Scale { get; }
    public bool Gamma { get; }

    public MipMap MipMap { get; }

    public ImageTexture(TextureMapping2D mapping, Stream stream, string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        Mapping = mapping;
        Filename = filename;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        ImageWrap = imageWrap;
        Scale = scale;
        Gamma = gamma;
        
        MipMap = MipMap.GetMipMap(filename, stream, doTrilinear, maxAniso, imageWrap, scale, gamma);        
    }

    public override Spectrum Evaluate(SurfaceInteraction si)
    {
        var st = Mapping.Map(si, out var dstDx, out var dstDy);
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