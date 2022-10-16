using System;
using pbrt.Core;

namespace pbrt.Textures
{
    public class Checkerboard3DTexture<T> : Texture<T>
    {
        public TextureMapping3D Mapping { get; }
        public Texture<T> Tex1 { get; }
        public Texture<T> Tex2 { get; }

        public Checkerboard3DTexture(TextureMapping3D mapping, Texture<T> tex1, Texture<T> tex2)
        {
            Mapping = mapping;
            Tex1 = tex1;
            Tex2 = tex2;
        }

        public override T Evaluate(SurfaceInteraction si)
        {
            var p = Mapping.Map(si, out _, out _);
            if (((int)MathF.Floor(p.X) + (int)MathF.Floor(p.Y) + (int)MathF.Floor(p.Z)) % 2 == 0)
            {
                return Tex1.Evaluate(si);
            }

            return Tex2.Evaluate(si);            
        }
    }
}