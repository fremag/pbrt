using pbrt.Core;

namespace pbrt.Textures
{
    public class ConstantTexture<T> : Texture<T>
    {
        public T Value { get; }
        
        public ConstantTexture(T value)
        {
            Value = value;
        }

        public override T Evaluate(SurfaceInteraction si) => Value;
    }
}