using pbrt.Core;

namespace pbrt.Reflections
{
    public abstract class MicrofacetDistribution
    {
        public bool SampleVisibleArea { get; }

        protected MicrofacetDistribution(bool sampleVisibleArea)
        {
            SampleVisibleArea = sampleVisibleArea;
        }

        public abstract float D(Vector3F wh);
        public abstract float Lambda(Vector3F w);
        
        public float G1(Vector3F w) => 1 / (1 + Lambda(w));
        public float G(Vector3F wo, Vector3F wi) => 1 / (1 + Lambda(wo) + Lambda(wi));
    }
}