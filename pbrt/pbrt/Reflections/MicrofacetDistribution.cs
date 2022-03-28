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
        public abstract Vector3F Sample_wh(Vector3F wo, Point2F u);
        
        public float G1(Vector3F w) => 1 / (1 + Lambda(w));
        public float G(Vector3F wo, Vector3F wi) => 1 / (1 + Lambda(wo) + Lambda(wi));
        
        public float Pdf(Vector3F wo, Vector3F wh)
        {
            if (SampleVisibleArea)
            {
                return D(wh) * G1(wo) * wo.AbsDot(wh) / BxDF.AbsCosTheta(wo);
            }

            return D(wh) * BxDF.AbsCosTheta(wh);
        }        
    }
}