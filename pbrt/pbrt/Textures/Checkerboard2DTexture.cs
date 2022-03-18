using System;
using pbrt.Core;

namespace pbrt.Textures
{
    public class Checkerboard2DTexture<T> : Texture<T>
    {
        public TextureMapping2D Mapping { get; }
        public Texture<T> Tex1 { get; }
        public Texture<T> Tex2 { get; }

        public Checkerboard2DTexture(TextureMapping2D mapping, Texture<T> tex1, Texture<T> tex2)
        {
            Mapping = mapping;
            Tex1 = tex1;
            Tex2 = tex2;
        }

        public override T Evaluate(SurfaceInteraction si)
        {
            Point2F st = Mapping.Map(si, out _, out _);
            // Point sample Checkerboard2DTexture 
            var x = (int)MathF.Floor(st.X);
            var y = (int)MathF.Floor(st.Y);
            
            if ((x + y) % 2 == 0)
            {
                return Tex1.Evaluate(si);
            }

            return Tex2.Evaluate(si);
        }
    }
}