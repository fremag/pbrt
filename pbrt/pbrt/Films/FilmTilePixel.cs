using pbrt.Spectrums;

namespace pbrt.Films
{
    public class FilmTilePixel 
    {
        public Spectrum contribSum = new Spectrum(SampledSpectrum.NSpectralSamples, 0f);
        public float filterWeightSum = 0f;
    }
}