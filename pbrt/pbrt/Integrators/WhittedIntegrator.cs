using System;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Reflections;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators
{
    public class WhittedIntegrator : SamplerIntegrator
    {
        public TransportMode TransportMode { get; }
        public int MaxDepth { get; }

        public WhittedIntegrator(int maxDepth, AbstractSampler sampler, AbstractCamera camera) : base(sampler, camera)
        {
            MaxDepth = maxDepth;
            TransportMode = new TransportMode();
        }

        public override Spectrum Li(RayDifferential ray, Scene scene, AbstractSampler sampler, int depth = 0)
        {
            Spectrum l = new Spectrum(0f);
            
            // Find closest ray intersection or return background radiance
            if (!scene.Intersect(ray, out var isect)) 
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
            isect.ComputeScatteringFunctions(ray, null, true, TransportMode);
            
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
                if (!f.IsBlack() && visibility.Unoccluded(scene))
                    l += f * li * wi.AbsDot(n) / pdf;
            }
            
            if (depth + 1 < MaxDepth) {
                // Trace rays for specular reflection and refraction 
                l += SpecularReflect(ray, isect, scene, sampler, depth);
                l += SpecularTransmit(ray, isect, scene, sampler, depth);
            }            
            return l;
        }
        
        public Spectrum SpecularReflect(RayDifferential ray, SurfaceInteraction isect, Scene scene, AbstractSampler sampler, int depth) 
        {
            // Compute specular reflection direction wi and BSDF value 
            Vector3F wo = isect.Wo;
            Vector3F wi;
            
            float pdf;
            BxDFType type = BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR;
            Spectrum f = isect.Bsdf.Sample_f(wo, out wi, sampler.Get2D(), out pdf, type);            
            // Return contribution of specular reflection
            throw new NotImplementedException();
        }

        public Spectrum SpecularTransmit(RayDifferential ray, SurfaceInteraction isect, Scene scene, AbstractSampler sampler, int depth)
        {
            throw new NotImplementedException();
        }
    }
}