using pbrt.Core;

namespace pbrt.Materials
{
    public abstract class Texture<T>
    {
        public abstract T Evaluate(SurfaceInteraction si);
    }
}