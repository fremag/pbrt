using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Lights
{
    public abstract class AreaLight : Light
    {
        public abstract Spectrum L(Interaction intr, Vector3F w);

        protected AreaLight(LightFlags flags, Transform lightToWorld, MediumInterface mediumInterface, int nSamples = 1) : base(flags, lightToWorld, mediumInterface, nSamples)
        {
        }
    }
}