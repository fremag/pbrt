using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using NLog;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Reflections;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators
{
    public abstract class SamplerIntegrator : Integrator
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        protected AbstractSampler Sampler { get; }
        protected AbstractCamera Camera { get; }
        public int NbThreads { get; }
        const int tileSize = 16;

        protected SamplerIntegrator(AbstractSampler sampler, AbstractCamera camera, int nbThreads=1)
        {
            Sampler = sampler;
            Camera = camera;
            NbThreads = nbThreads;
        }

        public override Bitmap Render(IScene scene)
        {
            Preprocess(scene);

            Bounds2I sampleBounds = Camera.Film.GetSampleBounds();
            Vector2I sampleExtent = sampleBounds.Diagonal();
            Point2I nTiles = new Point2I((sampleExtent.X + tileSize - 1) / tileSize, (sampleExtent.Y + tileSize - 1) / tileSize);
            var nbTiles = nTiles.X * nTiles.Y;

            void RenderTile(int tile)
            {
                Stopwatch sw = Stopwatch.StartNew();
                Render(tile, nTiles, sampleBounds, scene);
                sw.Stop();
                OnTileRendered(tile, nbTiles, sw.Elapsed);
            }

            Parallel.For(0, nbTiles,
                    new ParallelOptions { MaxDegreeOfParallelism = NbThreads },
                    RenderTile)
                ;

            return Camera.Film.WriteImage(1);
        }

        public void Render(int numTile, Point2I nTiles, Bounds2I sampleBounds, IScene scene)
        {
            var tileX = numTile % nTiles.X;
            var tileY = numTile / nTiles.X;

            // Get sampler instance for tile 
            int seed = tileY * nTiles.X + tileX;
            var tileSampler = Sampler.Clone(seed);

            // Compute sample bounds for tile
            int x0 = sampleBounds.PMin.X + tileX * tileSize;
            int x1 = Math.Min(x0 + tileSize, sampleBounds.PMax.X);
            int y0 = sampleBounds.PMin.Y + tileY * tileSize;
            int y1 = Math.Min(y0 + tileSize, sampleBounds.PMax.Y);
            Bounds2I tileBounds = new Bounds2I(new Point2I(x0, y0), new Point2I(x1, y1));
            
            // Get FilmTile for tile 
            FilmTile filmTile = Camera.Film.GetFilmTile(tileBounds);

            // Loop over pixels in tile to render them 
            foreach (Point2I pixel in tileBounds)
            {
                tileSampler.StartPixel(pixel);
                do
                {
                    // Initialize CameraSample for current sample 
                    CameraSample cameraSample = tileSampler.GetCameraSample(pixel);

                    // Generate camera ray for current sample 
                    float rayWeight = Camera.GenerateRayDifferential(cameraSample, out var ray);
                    ray.ScaleDifferentials(1 / MathF.Sqrt(tileSampler.SamplesPerPixel));

                    // Evaluate radiance along camera ray 
                    Spectrum l = new Spectrum(0f);
                    if (rayWeight > 0)
                    {
                        l = Li(ray, scene, tileSampler);
                        // Issue warning if unexpected radiance value is returned
                        if (l.HasNaNs())
                        {
                            Logger.Error("Not-a-number radiance value returned for image sample.  Setting to black.");
                            l = new Spectrum(0f);
                        }
                        else if (l.Y() < -1e-5)
                        {
                            Logger.Error($"Negative luminance value, {l.Y()}, returned for image sample.  Setting to black.");
                            l = new Spectrum(0f);
                        }
                        else if (float.IsInfinity(l.Y()))
                        {
                            Logger.Error("Infinite luminance value returned for image sample.  Setting to black.");
                            l = new Spectrum(0f);
                        }
                    }

                    // Add camera ray’s contribution to image 
                    filmTile.AddSample(cameraSample.PFilm, l, rayWeight);
                } while (tileSampler.StartNextSample());
            }

            // Merge image tile into Film
            Camera.Film.MergeFilmTile(filmTile);
        }

        public abstract Spectrum Li(RayDifferential ray, IScene scene, AbstractSampler sampler, int depth = 0);

        protected virtual void Preprocess(IScene scene)
        {
        }

        public Spectrum SpecularReflect(RayDifferential ray, SurfaceInteraction isect, IScene scene, AbstractSampler sampler, int depth)
        {
            // Compute specular reflection direction _wi_ and BSDF value
            Vector3F wo = isect.Wo;
            Vector3F wi;
            float pdf;
            BxDFType type = BxDFType.BSDF_REFLECTION | BxDFType.BSDF_SPECULAR;
            BxDFType sampledType = BxDFType.BSDF_NONE;
            Spectrum f = isect.Bsdf.Sample_f(wo, out wi, sampler.Get2D(), out pdf, type, ref sampledType);

            // Return contribution of specular reflection
            Normal3F ns = isect.Shading.N;
            if (pdf > 0f && !f.IsBlack() && wi.AbsDot(ns) != 0f)
            {
                // Compute ray differential _rd_ for specular reflection
                var spawnRay = isect.SpawnRay(wi);
                RayDifferential rd = new RayDifferential(spawnRay);
                if (ray.HasDifferentials)
                {
                    rd.HasDifferentials = true;
                    rd.RxOrigin = isect.P + isect.DpDx;
                    rd.RyOrigin = isect.P + isect.DpDy;
                    // Compute differential reflected directions
                    Normal3F dndx = isect.Shading.DnDu * isect.DuDx + isect.Shading.DnDv * isect.DvDx;
                    Normal3F dndy = isect.Shading.DnDu * isect.DuDy + isect.Shading.DnDv * isect.DvDy;
                    Vector3F dwodx = -ray.RxDirection - wo;
                    Vector3F dwody = -ray.RyDirection - wo;
                    float dDNdx = dwodx.Dot(ns) + wo.Dot(dndx);
                    float dDNdy = dwody.Dot(ns) + wo.Dot(dndy);
                    rd.RxDirection = wi - dwodx + 2f * new Vector3F(wo.Dot(ns) * dndx + dDNdx * ns);
                    rd.RyDirection = wi - dwody + 2f * new Vector3F(wo.Dot(ns) * dndy + dDNdy * ns);
                }

                return f * Li(rd, scene, sampler, depth + 1) * wi.AbsDot(ns) / pdf;
            }

            return new Spectrum(0f);
        }

        public Spectrum SpecularTransmit(RayDifferential ray, SurfaceInteraction isect, IScene scene, AbstractSampler sampler, int depth)
        {
            Vector3F wo = isect.Wo;
            Vector3F wi;
            float pdf;
            Point3F p = isect.P;
            BSDF bsdf = isect.Bsdf;
            BxDFType sampledType = BxDFType.BSDF_NONE;
            Spectrum f = bsdf.Sample_f(wo, out wi, sampler.Get2D(), out pdf, BxDFType.BSDF_TRANSMISSION | BxDFType.BSDF_SPECULAR, ref sampledType);
            Spectrum L = new Spectrum(0f);
            Normal3F ns = isect.Shading.N;
            if (pdf > 0f && !f.IsBlack() && wi.AbsDot(ns) != 0f)
            {
                // Compute ray differential _rd_ for specular transmission
                var spawnRay = isect.SpawnRay(wi);
                RayDifferential rd = new RayDifferential(spawnRay);
                if (ray.HasDifferentials)
                {
                    rd.HasDifferentials = true;
                    rd.RxOrigin = p + isect.DpDx;
                    rd.RyOrigin = p + isect.DpDy;

                    Normal3F dndx = isect.Shading.DnDu * isect.DuDx + isect.Shading.DnDv * isect.DvDx;
                    Normal3F dndy = isect.Shading.DnDu * isect.DuDy + isect.Shading.DnDv * isect.DvDy;

                    // The BSDF stores the IOR of the interior of the object being
                    // intersected.  Compute the relative IOR by first out by
                    // assuming that the ray is entering the object.
                    float eta = 1 / bsdf.Eta;
                    if (wo.Dot(ns) < 0)
                    {
                        // If the ray isn't entering, then we need to invert the
                        // relative IOR and negate the normal and its derivatives.
                        eta = 1 / eta;
                        ns = -ns;
                        dndx = -dndx;
                        dndy = -dndy;
                    }

                    /*
                      Notes on the derivation:
                      - pbrt computes the refracted ray as: \wi = -\eta \omega_o + [ \eta (\wo \cdot \N) - \cos \theta_t ] \N
                        It flips the normal to lie in the same hemisphere as \wo, and then \eta is the relative IOR from
                        \wo's medium to \wi's medium.
                      - If we denote the term in brackets by \mu, then we have: \wi = -\eta \omega_o + \mu \N
                      - Now let's take the partial derivative. (We'll use "d" for \partial in the following for brevity.)
                        We get: -\eta d\omega_o / dx + \mu dN/dx + d\mu/dx N.
                      - We have the values of all of these except for d\mu/dx (using bits from the derivation of specularly
                        reflected ray deifferentials).
                      - The first term of d\mu/dx is easy: \eta d(\wo \cdot N)/dx. We already have d(\wo \cdot N)/dx.
                      - The second term takes a little more work. We have:
                         \cos \theta_i = \sqrt{1 - \eta^2 (1 - (\wo \cdot N)^2)}.
                         Starting from (\wo \cdot N)^2 and reading outward, we have \cos^2 \theta_o, then \sin^2 \theta_o,
                         then \sin^2 \theta_i (via Snell's law), then \cos^2 \theta_i and then \cos \theta_i.
                      - Let's take the partial derivative of the sqrt expression. We get:
                        1 / 2 * 1 / \cos \theta_i * d/dx (1 - \eta^2 (1 - (\wo \cdot N)^2)).
                      - That partial derivatve is equal to:
                        d/dx \eta^2 (\wo \cdot N)^2 = 2 \eta^2 (\wo \cdot N) d/dx (\wo \cdot N).
                      - Plugging it in, we have d\mu/dx =
                        \eta d(\wo \cdot N)/dx - (\eta^2 (\wo \cdot N) d/dx (\wo \cdot N))/(-\wi \cdot N).
                     */
                    Vector3F dwodx = -ray.RxDirection - wo;
                    Vector3F dwody = -ray.RyDirection - wo;
                    float dDNdx = dwodx.Dot(ns) + wo.Dot(dndx);
                    float dDNdy = dwody.Dot(ns) + wo.Dot(dndy);

                    float mu = eta * wo.Dot(ns) - wi.AbsDot(ns);
                    float dmudx = (eta - (eta * eta * wo.Dot(ns)) / wi.AbsDot(ns)) * dDNdx;
                    float dmudy = (eta - (eta * eta * wo.Dot(ns)) / wi.AbsDot(ns)) * dDNdy;

                    rd.RxDirection = wi - eta * dwodx + new Vector3F(mu * dndx + dmudx * ns);
                    rd.RyDirection = wi - eta * dwody + new Vector3F(mu * dndy + dmudy * ns);
                }

                L = f * Li(rd, scene, sampler, depth + 1) * wi.AbsDot(ns) / pdf;
            }

            return L;
        }
    }
}