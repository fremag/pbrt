using System.Collections.Generic;
using System.Drawing;
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
    
    public static Dictionary<TextureInfo, MipMap> MipMapCache { get; }= new();    
    public static void ClearCache() 
    {
        MipMapCache.Clear();
    }    

    public ImageTexture(TextureMapping2D mapping, Stream stream, string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        Mapping = mapping;
        Filename = filename;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        ImageWrap = imageWrap;
        Scale = scale;
        Gamma = gamma;
        
        MipMap = GetTexture(filename, stream, doTrilinear, maxAniso, imageWrap, scale, gamma);        
    }

    private static MipMap GetTexture(string filename, Stream stream, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        TextureInfo textureInfo = new TextureInfo(filename, doTrilinear, maxAniso, imageWrap, scale, gamma);
        if (!MipMapCache.TryGetValue(textureInfo, out var mipMap))
        {
            // Read Image + Init MipMap
            var image = Image.FromStream(stream);
            var bmp = new Bitmap(image);
            RgbSpectrum[] data = new RgbSpectrum[bmp.Height*bmp.Width];
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    var idx = i * image.Width + j;
                    var color = bmp.GetPixel(j, i);
                    var colorR = color.R/255f;
                    var colorG = color.G/255f;
                    var colorB = color.B/255f;
                    var floats = new []{colorR, colorG, colorB};
                    data[idx] = RgbSpectrum.FromRGB(floats);
                }
            }
            mipMap = new MipMap(new Point2I(bmp.Width, bmp.Height), data, doTrilinear, maxAniso, imageWrap);
            MipMapCache[textureInfo] = mipMap; 
        }

        return mipMap;
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