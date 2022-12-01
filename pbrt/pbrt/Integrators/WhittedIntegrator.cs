using System.Diagnostics.CodeAnalysis;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators
{
    [ExcludeFromCodeCoverage]
    public class WhittedIntegrator : SamplerIntegrator
    {
        public int MaxDepth { get; }

        public WhittedIntegrator(int maxDepth, AbstractSampler sampler, AbstractCamera camera, int nbThreads = 1, int tileSize = 16) : base(sampler, camera, nbThreads, tileSize)
        {
            MaxDepth = maxDepth;
        }

        public override Spectrum Li(RayDifferential ray, IScene scene, ISampler sampler, int depth = 0)
        {
            Spectrum l = new Spectrum(0f);
            // Find closest ray intersection or return background radiance
            var intersect = scene.Intersect(ray, out var isect);

            if (!intersect) 
            {
                foreach (var light in scene.Lights)
                {
                    l += light.Le(ray);
                }

                return l;
            }
            
            // Compute emitted and reflected light at ray intersection point
            //Initialize common variables for Whitted integrator 
            Normal3F n = isect.Shading.N;
            Vector3F wo = isect.Wo;
            
            // Compute scattering functions for surface interaction 
            isect.ComputeScatteringFunctions(ray, null, false, TransportMode.Radiance);
            if (isect.Bsdf == null)
            {
                var spawnRay = isect.SpawnRay(ray.D);
                RayDifferential spawnRayDiff = new RayDifferential(spawnRay);
                return Li(spawnRayDiff, scene, sampler, depth);
            }

            // Compute emitted light if ray hit an area light source 
            l += isect.Le(wo);
            
            // Add contribution of each light source 
            foreach (var light in scene.Lights) 
            {
                Spectrum li = light.Sample_Li(isect, sampler.Get2D(), out var wi, out var pdf, out var visibility);
                if (li.IsBlack() || pdf == 0)
                {
                    continue;
                }

                Spectrum f = isect.Bsdf.F(wo, wi);

                var isBlack = f.IsBlack();
                if (isBlack)
                {
                    continue;
                }
                
                var unOccluded = visibility.Unoccluded(scene);
                if (unOccluded)
                {
                    l += f * li * wi.AbsDot(n) / pdf;
                }
            }
            
            if (depth + 1 < MaxDepth)
            {
                // Trace rays for specular reflection and refraction 
                l += SpecularReflect(ray, isect, scene, sampler, depth);
                l += SpecularTransmit(ray, isect, scene, sampler, depth);
            }            
            return l;
        }

        public override void Preprocess(IScene scene, AbstractSampler sampler)
        {
            // nothing to do
        }
    }
}