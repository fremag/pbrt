using pbrt.Core;

namespace pbrt.Textures
{
    public abstract class Texture<T>
    {
        public abstract T Evaluate(SurfaceInteraction si);
    }
}