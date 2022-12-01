using System;
using System.Diagnostics.CodeAnalysis;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Reflections;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators;

[ExcludeFromCodeCoverage]
public class PathIntegrator : SamplerIntegrator 
{
    int maxDepth;
    public PathIntegrator(int maxDepth, AbstractSampler sampler, AbstractCamera camera, int nbThreads, int tileSize) : base(sampler, camera, nbThreads, tileSize)
    {
        this.maxDepth = maxDepth;
    }

    public override void Preprocess(IScene scene, AbstractSampler sampler)
    {
        // Nothing ot do
    }

    public override Spectrum Li(RayDifferential r, IScene scene, ISampler sampler, int depth=0) 
    {
        var L = new Spectrum(0f);
        var beta = new Spectrum(1f);
            
        RayDifferential ray = new RayDifferential(r);
        bool specularBounce = false;
        for (int bounces = 0; ; ++bounces) 
        {
            // Find next path vertex and accumulate contribution
            
            // Intersect ray with scene and store intersection in isect
            SurfaceInteraction isect;
            bool foundIntersection = scene.Intersect(ray, out isect);
            
            // Possibly add emitted light at intersection
            if (bounces == 0 || specularBounce) 
            {
                // Add emitted light at path vertex or from the environment
                if (foundIntersection)
                {
                    L += beta * isect.Le(-ray.D);
                }
                else
                {
                    foreach( var light in scene.Lights)
                    {
                        L += beta * light.Le(ray);
                    }
                }
            }
            
            // Terminate path if ray escaped or maxDepth was reached
            if (!foundIntersection || bounces >= maxDepth)
            {
                break;
            }
            
            // Compute scattering functions and skip over medium boundaries
            isect.ComputeScatteringFunctions(ray, null, true, TransportMode.Radiance);
            if (isect.Bsdf == null) 
            {
                var spawnRay = isect.SpawnRay(ray.D);
                ray = new RayDifferential(spawnRay);
                bounces--;
                continue;
            }
            
            // Sample illumination from lights to find path contribution
            var oneLight = LightSampler.UniformSampleOneLight(isect, scene, sampler);
            oneLight.Mul(beta);
            L.Add(oneLight);
            
            // Sample BSDF to get new path direction
            Vector3F wo = -ray.D;
            Vector3F wi;
            
            float pdf;
            BxDFType flags = default;
            Spectrum f = isect.Bsdf.Sample_f(wo, out wi, sampler.Get2D(), out pdf, BxDFType.BSDF_ALL, ref flags);
            if (f.IsBlack() || pdf == 0f)
            {
                break;
            }

            var absDot = wi.AbsDot(isect.Shading.N);
            beta.Mul( absDot / pdf );
            beta.Mul( f );
            
            specularBounce = (flags & BxDFType.BSDF_SPECULAR) != 0;
            var spawnRay1 = isect.SpawnRay(wi);
            ray = new RayDifferential(spawnRay1);
            
            // Account for subsurface scattering, if applicable
            // Later...
            
            // Possibly terminate the path with Russian roulette
            if (bounces > 3) 
            {
                float q = MathF.Max(0.05f, 1 - beta.Y());
                if (sampler.Get1D() < q)
                    break;
                beta /= 1 - q;
            }
        }
        
        return L;
    }

}