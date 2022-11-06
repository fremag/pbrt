using pbrt.Core;
using pbrt.Textures;

namespace Pbrt.Demos.Scenes;

public class TextureUVMapping2D : UVMapping2D
{
    public TextureUVMapping2D() : base(1, 1, 0, 0)
    {
    }

    public override Point2F Map(SurfaceInteraction si, out Vector2F dstdx, out Vector2F dstdy)
    {
        var st = base.Map(si, out dstdx, out dstdy);
        dstdx = Vector2F.Zero;
        dstdy = Vector2F.Zero;
        return st;
    }
}