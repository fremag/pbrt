using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using pbrt.Core;
using pbrt.Filters;
using pbrt.Spectrums;

namespace pbrt.Films
{
    public class Film
    {
        public Point2I FullResolution { get; }
        public Bounds2F CropWindow { get; }
        public AbstractFilter Filter { get; }
        public float Diagonal { get; }
        public string Filename { get; }
        public float Scale { get; }

        public Bounds2I CroppedPixelBounds { get; }
        public Pixel[] Pixels { get; }
        static readonly int filterTableWidth = 16;
        float[] filterTable;

        private object objLock = new object();

        public Film(int width, int heigth) : this(new Point2I(width, heigth), new Bounds2F(new Point2F(0, 0), new Point2F(1, 1)), new BoxFilter(new Vector2F(1, 1)), 1, null, 1)
        {
        }

        public Film(Point2I fullResolution, Bounds2F cropWindow, AbstractFilter filter, float diagonal, string filename, float scale)
        {
            FullResolution = fullResolution;
            CropWindow = cropWindow;
            Filter = filter;
            Diagonal = diagonal;
            Filename = filename;
            Scale = scale;

            // Compute film image bounds
            var croppedPMinX = (int)Math.Ceiling(FullResolution.X * CropWindow.PMin.X);
            var croppedPMinY = (int)Math.Ceiling(FullResolution.Y * CropWindow.PMin.Y);
            var croppedPMaxX = (int)Math.Ceiling(FullResolution.X * CropWindow.PMax.X);
            var croppedPMaxY = (int)Math.Ceiling(FullResolution.Y * CropWindow.PMax.Y);

            var croppedPMin = new Point2I(croppedPMinX, croppedPMinY);
            var croppedPMax = new Point2I(croppedPMaxX, croppedPMaxY);
            CroppedPixelBounds = new Bounds2I(croppedPMin, croppedPMax);

            // Allocate film image storage
            Pixels = Enumerable.Range(0, CroppedPixelBounds.SurfaceArea).Select(_ => new Pixel()).ToArray();

            // Precompute filter weight table 
            int offset = 0;
            filterTable = new float[filterTableWidth * filterTableWidth];
            for (int y = 0; y < filterTableWidth; ++y)
            {
                for (int x = 0; x < filterTableWidth; ++x, ++offset)
                {
                    var pX = (x + 0.5f) * filter.Radius.X / filterTableWidth;
                    var pY = (y + 0.5f) * filter.Radius.Y / filterTableWidth;
                    Point2F p = new Point2F(pX, pY);
                    filterTable[offset] = filter.Evaluate(p);
                }
            }
        }

        public Bounds2I GetSampleBounds()
        {
            var v = new Vector2F(0.5f, 0.5f);
            var pMin = ((Point2F)CroppedPixelBounds.PMin + v - Filter.Radius).Floor();
            var pMax = ((Point2F)CroppedPixelBounds.PMax - v + Filter.Radius).Ceil();
            Bounds2I floatBounds = new Bounds2I((Point2I)pMin, (Point2I)pMax);
            return floatBounds;
        }

        public Bounds2F GetPhysicalExtent()
        {
            float aspect = (float)FullResolution.Y / FullResolution.X;
            float x = MathF.Sqrt(Diagonal * Diagonal / (1 + aspect * aspect));
            float y = aspect * x;
            return new Bounds2F(new Point2F(-x / 2, -y / 2), new Point2F(x / 2, y / 2));
        }

        public FilmTile GetFilmTile(Bounds2I sampleBounds)
        {
            // Bound image pixels that samples in sampleBounds contribute to
            Vector2F halfPixel = new Vector2F(0.5f, 0.5f);
            Bounds2F floatBounds = new Bounds2F((Point2F)sampleBounds.PMin, (Point2F)sampleBounds.PMax);
            Point2I p0 = (Point2I)(floatBounds.PMin - halfPixel - Filter.Radius).Ceil();
            Point2I p1 = (Point2I)(floatBounds.PMax - halfPixel + Filter.Radius).Floor() + new Point2I(1, 1);
            Bounds2I tilePixelBounds = new Bounds2I(p0, p1).Intersect(CroppedPixelBounds);

            return new FilmTile(tilePixelBounds, Filter.Radius, filterTable, filterTableWidth);
        }

        public void MergeFilmTile(FilmTile tile)
        {
            lock (objLock)
            {
                foreach (Point2I pixel in tile.PixelBounds)
                {
                    // Merge pixel into Film::pixels 
                    FilmTilePixel tilePixel = tile.GetPixel(pixel);
                    Pixel mergePixel = GetPixel(pixel);
                    float[] xyz = tilePixel.contribSum.ToXYZ();
                    for (int i = 0; i < 3; ++i)
                    {
                        mergePixel.xyz[i] += xyz[i];
                    }

                    mergePixel.filterWeightSum += tilePixel.filterWeightSum;
                }
            }
        }

        public Pixel GetPixel(Point2I p)
        {
            int width = CroppedPixelBounds.PMax.X - CroppedPixelBounds.PMin.X;
            int offset = (p.X - CroppedPixelBounds.PMin.X) + (p.Y - CroppedPixelBounds.PMin.Y) * width;
            return Pixels[offset];
        }

        public void SetImage(Spectrum[] img)
        {
            int nPixels = CroppedPixelBounds.SurfaceArea;
            for (int i = 0; i < nPixels; ++i)
            {
                Pixel p = Pixels[i];
                Spectrum spectrum = img[i];
                p.xyz = spectrum.ToXYZ();
                p.filterWeightSum = 1;
                p.splatXYZ[0] = 0;
                p.splatXYZ[1] = 0;
                p.splatXYZ[2] = 0;
            }
        }

        public void AddSplat(Point2F p, Spectrum v)
        {
            var point2I = (Point2I)p;
            if (!CroppedPixelBounds.InsideExclusive(point2I))
            {
                return;
            }

            float[] xyz = v.ToXYZ();
            Pixel pixel = GetPixel(point2I);
            pixel.AddSplatXyz(xyz[0], xyz[1], xyz[2]);
        }

        public Bitmap WriteImage(float splatScale)
        {
            // Convert image to RGB and compute final pixel values
            float[] rgbs = new float[3 * CroppedPixelBounds.SurfaceArea];
            int offset = 0;
            foreach (Point2I p in CroppedPixelBounds)
            {
                // Convert pixel XYZ color to RGB 
                Pixel pixel = GetPixel(p);
                var rgb = SpectrumUtils.XYZToRGB(pixel.xyz);
                rgbs[3 * offset + 0] = rgb[0];
                rgbs[3 * offset + 1] = rgb[1];
                rgbs[3 * offset + 2] = rgb[2];

                // Normalize pixel with weight sum
                float filterWeightSum = pixel.filterWeightSum;
                if (filterWeightSum != 0)
                {
                    float invWt = 1f / filterWeightSum;
                    rgbs[3 * offset] = MathF.Max(0f, rgbs[3 * offset] * invWt);
                    rgbs[3 * offset + 1] = MathF.Max(0f, rgbs[3 * offset + 1] * invWt);
                    rgbs[3 * offset + 2] = MathF.Max(0f, rgbs[3 * offset + 2] * invWt);
                }

                // Add splat value at pixel 
                float[] splatXYZ = { pixel.splatXYZ[0], pixel.splatXYZ[1], pixel.splatXYZ[2] };
                float[] splatRGB = SpectrumUtils.XYZToRGB(splatXYZ);
                rgbs[3 * offset] += splatScale * splatRGB[0];
                rgbs[3 * offset + 1] += splatScale * splatRGB[1];
                rgbs[3 * offset + 2] += splatScale * splatRGB[2];

                // Scale pixel value by scale 
                rgbs[3 * offset] *= Scale;
                rgbs[3 * offset + 1] *= Scale;
                rgbs[3 * offset + 2] *= Scale;

                ++offset;
            }

            // Write RGB image
            Bitmap bmp = new Bitmap(FullResolution.X, FullResolution.Y, PixelFormat.Format24bppRgb);
            int pos = 0;
            for (int y = 0; y < FullResolution.Y; y++)
            {
                for (int x = 0; x < FullResolution.X; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb((int)rgbs[pos], (int)rgbs[pos + 1], (int)rgbs[pos + 2]));
                    pos += 3;
                }
            }

            return bmp;
        }
    }
}