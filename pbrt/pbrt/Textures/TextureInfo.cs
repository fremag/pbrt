using System;

namespace pbrt.Textures;

public class TextureInfo
{
    public string Filename { get; }
    public bool DoTrilinear { get; }
    public float MaxAniso { get; }
    public ImageWrap ImageWrap { get; }
    public float Scale { get; }
    public bool Gamma { get; }

    public TextureInfo(string filename, bool doTrilinear, float maxAniso, ImageWrap imageWrap, float scale, bool gamma)
    {
        Filename = filename;
        DoTrilinear = doTrilinear;
        MaxAniso = maxAniso;
        ImageWrap = imageWrap;
        Scale = scale;
        Gamma = gamma;
    }

    public bool Equals(TextureInfo other)
    {
        return Filename == other.Filename && DoTrilinear == other.DoTrilinear && MaxAniso.Equals(other.MaxAniso) && ImageWrap == other.ImageWrap && Scale.Equals(other.Scale) && Gamma == other.Gamma;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((TextureInfo)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Filename, DoTrilinear, MaxAniso, (int)ImageWrap, Scale, Gamma);
    }
}