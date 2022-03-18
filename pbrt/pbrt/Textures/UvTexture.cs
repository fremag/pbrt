using System;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Textures
{
    public class UvTexture : Texture<Spectrum>
    {
        public TextureMapping2D Mapping { get; }

        public UvTexture(TextureMapping2D mapping)
        {
            Mapping = mapping;
        }

        public override Spectrum Evaluate(SurfaceInteraction si)
        {
            Point2F st = Mapping.Map(si, out _, out _);
            float[] rgb = { st[0] - MathF.Floor(st.X), st.Y - MathF.Floor(st.Y), 0 };
            var result = new Spectrum(SampledSpectrum.FromRgb(rgb));
            return result;
        }
    }
}