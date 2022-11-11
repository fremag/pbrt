using System;
using System.Linq;
using pbrt.Core;
using pbrt.Spectrums;

namespace pbrt.Films
{
    public class FilmTile
    {
        public FilmTilePixel[] Pixels { get; set; }
        public Bounds2I PixelBounds { get; }
        public Vector2F FilterRadius { get; }
        public Vector2F InvFilterRadius { get; set; }
        public float[] FilterTable { get; }
        public int FilterTableSize { get; }

        public FilmTile(Bounds2I pixelBounds, Vector2F filterRadius, float[] filterTable, int filterTableSize)
        {
            PixelBounds = pixelBounds;
            FilterRadius = filterRadius;
            InvFilterRadius = new Vector2F(1 / FilterRadius.X, 1 / FilterRadius.Y);

            FilterTable = filterTable;
            FilterTableSize = filterTableSize;

            Pixels = Enumerable.Range(0, pixelBounds.SurfaceArea).Select(_ => new FilmTilePixel()).ToArray();
        }

        public void AddSample(Point2F pFilm, Spectrum L, float sampleWeight = 1f)
        {
            // Compute sample’s raster bounds>> 
            Point2F pFilmDiscrete = pFilm - new Vector2F(0.5f, 0.5f);
            Point2I p0 = (Point2I)(pFilmDiscrete - FilterRadius).Ceil();
            Point2I p1 = (Point2I)(pFilmDiscrete + FilterRadius).Floor() + new Point2I(1, 1);
            p0 = Point2I.Max(p0, PixelBounds.PMin);
            p1 = Point2I.Min(p1, PixelBounds.PMax);
            
            // Loop over filter support and add sample to pixel arrays
            // Precompute x and y filter table offsets 
            int[] ifx = new int[p1.X - p0.X];
            for (int x = p0.X; x < p1.X; ++x) 
            {
                float fx = MathF.Abs((x - pFilmDiscrete.X) * InvFilterRadius.X * FilterTableSize);
                ifx[x - p0.X] = Math.Min((int)Math.Floor(fx), FilterTableSize - 1);
            }
            
            int[] ify = new int[p1.Y - p0.Y];
            for (int y = p0.Y; y < p1.Y; ++y) 
            {
                float fy = MathF.Abs((y - pFilmDiscrete.Y) * InvFilterRadius.Y * FilterTableSize);
                ify[y - p0.Y] = Math.Min((int)MathF.Floor(fy), FilterTableSize - 1);
            }
            
            for (int y = p0.Y; y < p1.Y; ++y) 
            {
                for (int x = p0.X; x < p1.X; ++x) 
                {
                    // Evaluate filter value at (x, y) pixel 
                    int offset = ify[y - p0.Y] * FilterTableSize + ifx[x - p0.X];
                    float filterWeight = FilterTable[offset];
                    
                    // Update pixel values with filtered sample contribution 
                    FilmTilePixel pixel = GetPixel(x, y);
                    pixel.contribSum.AddMul(L, sampleWeight * filterWeight);
                    pixel.filterWeightSum += filterWeight;
                }
            }            
        }
        
        public  FilmTilePixel GetPixel(Point2I pixel) => GetPixel(pixel.X, pixel.Y);
        public FilmTilePixel GetPixel(int x, int y) 
        {
            int width = PixelBounds.PMax.X - PixelBounds.PMin.X;
            int offset = (x - PixelBounds.PMin.X) + (y - PixelBounds.PMin.Y) * width;
            return Pixels[offset];
        }
    }
}