using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Lights;
using pbrt.Media;
using pbrt.Reflections;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators 
{
    public enum LightStrategy
    {
        UniformSampleAll,
        UniformSampleOne
    };

    [ExcludeFromCodeCoverage]
    public class DirectLightingIntegrator : SamplerIntegrator
    {
        public LightStrategy Strategy { get; set; }
        public int MaxDepth { get; set; }
        public List<int> NLightSamples { get; }

        public DirectLightingIntegrator(AbstractSampler sampler, AbstractCamera camera, LightStrategy strategy, int maxDepth, int nbThreads = 1, int tileSize = 16) : base(sampler, camera, nbThreads, tileSize)
        {
            Strategy = strategy;
            MaxDepth = maxDepth;
            NLightSamples = new List<int>();
        }

        public override void Preprocess(IScene scene, AbstractSampler sampler)
        {
            if (Strategy == LightStrategy.UniformSampleAll)
            {
                // Compute number of samples to use for each light
                foreach (var light in scene.Lights)
                {
                    var roundCount = sampler.RoundCount(light.NSamples);
                    NLightSamples.Add(roundCount);
                }

                // Request samples for sampling all lights
                for (int i = 0; i < MaxDepth; ++i)
                {
                    for (int j = 0; j < scene.Lights.Length; ++j)
                    {
                        sampler.Request2DArray(NLightSamples[j]); // one for a position on the light shape
                        sampler.Request2DArray(NLightSamples[j]); // another one for a scattering direction from the BSDF
                    }
                }
            }
        }

        public override Spectrum Li(RayDifferential ray, IScene scene, AbstractSampler sampler, int depth = 0)
        {
            Spectrum L = new Spectrum(0f);
            // Find closest ray intersection or return background radiance
            if (!scene.Intersect(ray, out var isect))
            {
                foreach (var light in scene.Lights)
                {
                    L += light.Le(ray);
                }

                return L;
            }

            // Compute scattering functions for surface interaction
            isect.ComputeScatteringFunctions(ray, null, false, TransportMode.Radiance);

            if (isect.Bsdf == null)
            {
                Ray spawnRay = isect.SpawnRay(ray.D);
                var spawnRayDiff = new RayDifferential(spawnRay);
                return Li(spawnRayDiff, scene, sampler, depth);
            }

            Vector3F wo = isect.Wo;
            // Compute emitted light if ray hit an area light source
            L += isect.Le(wo);
            if (scene.Lights.Any())
            {
                switch (Strategy)
                {
                    // Compute direct lighting for_DirectLightingIntegrator integrator
                    case LightStrategy.UniformSampleAll:
                        L += UniformSampleAllLights(isect, scene, sampler, NLightSamples);
                        break;
                   case LightStrategy.UniformSampleOne:
                        L += UniformSampleOneLight(isect, scene, sampler);
                        break;
                }
            }

            if (depth + 1 < MaxDepth)
            {
                // Trace rays for specular reflection and refraction
                L += SpecularReflect(ray, isect, scene, sampler, depth);
                L += SpecularTransmit(ray, isect, scene, sampler, depth);
            }

            return L;
        }
    }
}