using pbrt.Spectrums;

namespace pbrt.Reflections
{
    public interface Fresnel
    {
        Spectrum Evaluate(float cosI);
    }
}