using System;
using System.Drawing;
using System.Threading.Tasks;
using NLog;
using pbrt.Cameras;
using pbrt.Core;
using pbrt.Films;
using pbrt.Samplers;
using pbrt.Spectrums;

namespace pbrt.Integrators
{
    public abstract class SamplerIntegrator : Integrator
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
    
        protected AbstractSampler Sampler { get; }
        protected AbstractCamera Camera { get; }
        const int tileSize = 16;

        protected SamplerIntegrator(AbstractSampler sampler, AbstractCamera camera)
        {
            Sampler = sampler;
            Camera = camera;
        }

        public override Bitmap Render(Scene scene)
        {
            Preprocess(scene);
            
            Bounds2I sampleBounds = Camera.Film.GetSampleBounds();
            Vector2I sampleExtent = sampleBounds.Diagonal();
            Point2I nTiles = new Point2I((sampleExtent.X + tileSize - 1) / tileSize, (sampleExtent.Y + tileSize - 1) / tileSize);
            Parallel.For(0, nTiles.X * nTiles.Y, 
                    new ParallelOptions { MaxDegreeOfParallelism = 1 }, 
                    tile => Render(tile, nTiles, sampleBounds, scene))
            ;
            
            return Camera.Film.WriteImage(1);
        }

        public void Render(int tile, Point2I nTiles, Bounds2I sampleBounds, Scene scene)
        {
            var tileX = tile % nTiles.X;
            var tileY = tile / nTiles.X;

            // Get sampler instance for tile 
            int seed = tileY * nTiles.X + tileX;
            var tileSampler = Sampler.Clone(seed);
            
            // Compute sample bounds for tile>> 
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
                            Logger.Error($"Negative luminance value, {l.Y()}, returned for image sample.  Setting to black." );
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
                }
                while (tileSampler.StartNextSample());
            }
            // Merge image tile into Film>>
            Camera.Film.MergeFilmTile(filmTile);
        }

        public abstract Spectrum Li(RayDifferential ray, Scene scene, AbstractSampler sampler, int depth = 0);
        
        protected virtual void Preprocess(Scene scene)
        {
            
        }
    }
}