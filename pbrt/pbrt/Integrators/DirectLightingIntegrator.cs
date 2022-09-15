using System;
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
        public LightStrategy Strategy { get; }
        public int MaxDepth { get; }
        public List<int> NLightSamples { get; }

        public DirectLightingIntegrator(AbstractSampler sampler, AbstractCamera camera, LightStrategy strategy, int maxDepth, int nbThreads = 1, int tileSize = 16) : base(sampler, camera, nbThreads, tileSize)
        {
            Strategy = strategy;
            MaxDepth = maxDepth;
            NLightSamples = new List<int>();
        }

        public void Preprocess(IScene scene, AbstractSampler sampler)
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

        public Spectrum UniformSampleAllLights(Interaction it, IScene scene, AbstractSampler sampler, List<int> nLightSamples, bool handleMedia = false)
        {
            Spectrum L = new(0);
            for (int j = 0; j < scene.Lights.Length; ++j)
            {
                // Accumulate contribution of j th light to L
                var light = scene.Lights[j];
                int nSamples = nLightSamples[j];
                Point2F[] uLightArray = sampler.Get2DArray(nSamples);
                Point2F[] uScatteringArray = sampler.Get2DArray(nSamples);

                if (uLightArray == null || uScatteringArray == null)
                {
                    // Use a single sample for illumination from light
                    Point2F uLight = sampler.Get2D();
                    Point2F uScattering = sampler.Get2D();
                    L += EstimateDirect(it, uScattering, light, uLight, scene, sampler, handleMedia);
                }
                else
                {
                    // Estimate direct lighting using sample arrays
                    Spectrum Ld = new(0f);
                    for (int k = 0; k < nSamples; ++k)
                    {
                        var uScattering = uScatteringArray[k];
                        var point2F = uLightArray[k];
                        Ld += EstimateDirect(it, uScattering, light, point2F, scene, sampler, handleMedia);
                    }

                    L += Ld / nSamples;
                }
            }

            return L;
        }

        public Spectrum UniformSampleOneLight(Interaction it, IScene scene, AbstractSampler sampler, bool handleMedia = false, Distribution1D lightDistrib = null)
        {
            // Randomly choose a single light to sample
            int nLights = scene.Lights.Length;
            if (nLights == 0)
            {
                return new Spectrum(0f);
            }

            int lightNum;
            float lightPdf;
            if (lightDistrib != null)
            {
                lightNum = lightDistrib.SampleDiscrete(sampler.Get1D(), out lightPdf, out _);
                if (lightPdf == 0)
                {
                    return new Spectrum(0f);
                }
            }
            else
            {
                lightNum = Math.Min((int)(sampler.Get1D() * nLights), nLights - 1);
                lightPdf = 1f / nLights;
            }

            Light light = scene.Lights[lightNum];
            Point2F uLight = sampler.Get2D();
            Point2F uScattering = sampler.Get2D();
            var estimateDirect = EstimateDirect(it, uScattering, light, uLight, scene, sampler, handleMedia);
            return estimateDirect / lightPdf;
        }

        public Spectrum EstimateDirect(Interaction it, Point2F uScattering, Light light, Point2F uLight, IScene scene, AbstractSampler sampler, bool handleMedia = false, bool specular = false)
        {
            BxDFType bsdfFlags = specular ? BxDFType.BSDF_ALL : BxDFType.BSDF_ALL & ~BxDFType.BSDF_SPECULAR;
            Spectrum Ld = new(0f);
            // Sample light source with multiple importance sampling
            float scatteringPdf = 0;
            Spectrum Li = light.Sample_Li(it, uLight, out var wi, out var lightPdf, out var visibility);

            if (lightPdf > 0 && !Li.IsBlack())
            {
                // Compute BSDF or phase function's value for light sample
                Spectrum f;
                if (it.IsSurfaceInteraction())
                {
                    // Evaluate BSDF for light sampling strategy
                    SurfaceInteraction isect = (SurfaceInteraction)it;
                    f = isect.Bsdf.F(isect.Wo, wi, bsdfFlags) * wi.AbsDot(isect.Shading.N);
                    scatteringPdf = isect.Bsdf.Pdf(isect.Wo, wi, bsdfFlags);
                }
                else
                {
                    // Evaluate phase function for light sampling strategy
                    MediumInteraction mi = (MediumInteraction)it;
                    float p = mi.Phase.P(mi.Wo, wi);
                    f = new Spectrum(p);
                    scatteringPdf = p;
                }

                if (!f.IsBlack())
                {
                    // Compute effect of visibility for light source sample
                    if (handleMedia)
                    {
                        Li *= visibility.Tr(scene, sampler);
                    }
                    else
                    {
                        if (!visibility.Unoccluded(scene))
                        {
                            Li = new Spectrum(0f);
                        }
                    }

                    // Add light's contribution to reflected radiance
                    if (!Li.IsBlack())
                    {
                        if (Light.IsDeltaLight(light.Flags))
                        {
                            Ld += f * Li / lightPdf;
                        }
                        else
                        {
                            float weight = MathUtils.PowerHeuristic(1, lightPdf, 1, scatteringPdf);
                            Ld += f * Li * weight / lightPdf;
                        }
                    }
                }
            }

            // Sample BSDF with multiple importance sampling
            if (!Light.IsDeltaLight(light.Flags))
            {
                Spectrum f;
                bool sampledSpecular = false;
                if (it.IsSurfaceInteraction())
                {
                    // Sample scattered direction for surface interactions
                    BxDFType sampledType = BxDFType.BSDF_NONE;
                    SurfaceInteraction isect = (SurfaceInteraction)it;
                    f = isect.Bsdf.Sample_f(isect.Wo, out wi, uScattering, out scatteringPdf, bsdfFlags, ref sampledType);
                    f *= wi.AbsDot(isect.Shading.N);
                    sampledSpecular = (sampledType & BxDFType.BSDF_SPECULAR) != 0;
                }
                else
                {
                    // Sample scattered direction for medium interactions
                    MediumInteraction mi = (MediumInteraction)it;
                    float p = mi.Phase.Sample_P(mi.Wo, out wi, uScattering);
                    f = new Spectrum(p);
                    scatteringPdf = p;
                }

                if (!f.IsBlack() && scatteringPdf > 0)
                {
                    // Account for light contributions along sampled direction wi
                    float weight = 1;
                    if (!sampledSpecular)
                    {
                        lightPdf = light.Pdf_Li(it, wi);
                        if (lightPdf == 0) return Ld;
                        weight = MathUtils.PowerHeuristic(1, scatteringPdf, 1, lightPdf);
                    }

                    // Find intersection and compute transmittance
                    SurfaceInteraction lightIsect;
                    Ray ray = it.SpawnRay(wi);
                    Spectrum Tr = new(1f);
                    bool foundSurfaceInteraction = handleMedia ? scene.IntersectTr(ray, sampler, out lightIsect, out Tr) : scene.Intersect(ray, out lightIsect);

                    // Add light contribution from material sampling
                    Li = new(0f);
                    if (foundSurfaceInteraction)
                    {
                        if (ReferenceEquals(lightIsect.Primitive.GetAreaLight(), light))
                        {
                            Li = lightIsect.Le(-wi);
                        }
                    }
                    else if (ray is RayDifferential rayDiff)
                    {
                        Li = light.Le(rayDiff);
                    }

                    if (!Li.IsBlack())
                    {
                        Ld += f * Li * Tr * (weight / scatteringPdf);
                    }
                }
            }

            return Ld;
        }
    }
}