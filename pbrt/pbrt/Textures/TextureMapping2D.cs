using pbrt.Core;

namespace pbrt.Textures
{
    public abstract class TextureMapping2D
    {
        public abstract Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy);
    }
}