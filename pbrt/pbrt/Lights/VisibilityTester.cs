using pbrt.Core;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Lights
{
    public class VisibilityTester
    {
        public Interaction P0 { get; }
        public Interaction P1 { get; }

        public VisibilityTester(Interaction p0, Interaction p1)
        {
            P0 = p0;
            P1 = p1;
        }

        public bool Unoccluded(IScene scene)
        {
            var rayTo = P0.SpawnRayTo(P1);
            return !scene.IntersectP(rayTo);
        }

        public Spectrum Tr(IScene scene, ISampler sampler)
        {
            Ray ray = P0.SpawnRayTo(P1);
            Spectrum tr = new Spectrum(1f);
            while (true)
            {
                bool hitSurface = scene.Intersect(ray, out var isect);
                // Handle opaque surface along ray’s path
                if (hitSurface && isect.Primitive.GetMaterial() != null)
                {
                    return new Spectrum(0.0f);
                }

                // Update transmittance for current ray segment
                if (ray.Medium != null)
                {
                    tr *= ray.Medium.Tr(ray, sampler);
                }

                // Generate next ray segment or return final transmittance 
                if (!hitSurface)
                {
                    break;
                }

                ray = isect.SpawnRayTo(P1);
            }

            return tr;
        }
    }
}