//#define WriteFile
using System;
#if WriteFile
using System.Diagnostics;
using System.IO;
#endif
using System.Linq;
using NFluent;
using NUnit.Framework;
using pbrt.Core;
using pbrt.Films;
using pbrt.Filters;
using pbrt.Spectrums;

namespace Pbrt.Tests.Films
{
    public class FilmTests
    {
        [Test]
        public void ConstructorTest()
        {
            float scale = 1.23f;
            Point2I resolution = new Point2I(320, 200);
            Bounds2F cropWindows = new Bounds2F(new Point2F(0,0), new Point2F(0.5f, 0.5f));
            var filter = new BoxFilter(new Vector2F(3, 2));
            var diagonal = 0.5f;
            var fileName = "img.png";
            var film = new Film(resolution, cropWindows, filter, diagonal, fileName, scale);
            Check.That(film.FullResolution).IsEqualTo(resolution );
            Check.That(film.CropWindow).IsEqualTo(cropWindows );
            Check.That(film.Filter).IsSameReferenceAs(filter);
            Check.That(film.Diagonal).IsEqualTo(diagonal);
            Check.That(film.Filename).IsEqualTo(fileName);
            Check.That(film.Scale).IsEqualTo(scale);
        }
        
        [Test]
        public void GetSampleBoundsTest()
        {
            var film = new Film(320, 200);
            Check.That(film.CropWindow.PMin.X).IsEqualTo(0);
            Check.That(film.CropWindow.PMin.Y).IsEqualTo(0);
            Check.That(film.CropWindow.PMax.X).IsEqualTo(1);
            Check.That(film.CropWindow.PMax.Y).IsEqualTo(1);
            var bounds = film.GetSampleBounds();
            Check.That(bounds.PMin.X).IsEqualTo(-1);
            Check.That(bounds.PMin.Y).IsEqualTo(-1);
            Check.That(bounds.PMax.X).IsEqualTo(321);
            Check.That(bounds.PMax.Y).IsEqualTo(201);
        }

        [Test]
        public void GetPhysicalExtentTest()
        {
            var film = new Film(320, 200);
            var physicalExtent = film.GetPhysicalExtent();
            var aspect = 200f / 320;
            var x = MathF.Sqrt(1 / (aspect * aspect + 1));
            Check.That(physicalExtent.PMin.X).IsEqualTo(-x / 2);
            Check.That(physicalExtent.PMin.Y).IsEqualTo(-aspect * x / 2);
            Check.That(physicalExtent.PMax.X).IsEqualTo(x / 2);
            Check.That(physicalExtent.PMax.Y).IsEqualTo(aspect * x / 2);
        }

        [Test]
        public void GetFilmTileTest()
        {
            var film = new Film(320, 200);
            Bounds2I sampleBounds = new Bounds2I(new Point2I(0, 0), new Point2I(9, 9));
            var filmTile = film.GetFilmTile(sampleBounds);

            Check.That(filmTile.FilterRadius.X).IsEqualTo(1f);
            Check.That(filmTile.FilterRadius.Y).IsEqualTo(1f);
            Check.That(filmTile.FilterTableSize).IsEqualTo(16);
            for (int i = 0; i < 16 * 16; i++)
            {
                Check.That(filmTile.FilterTable[i]).IsEqualTo(1);
            }

            Check.That(filmTile.Pixels).CountIs(10 * 10);
        }

        [Test]
        public void MergeFilmTileTest()
        {
            var film = new Film(320, 200);
            Bounds2I sampleBounds = new Bounds2I(new Point2I(0, 0), new Point2I(10, 10));
            var filmTile = film.GetFilmTile(sampleBounds);
            Check.That(filmTile.Pixels.All(pixel => pixel.contribSum.IsBlack())).IsTrue();
            Check.That(film.Pixels.All(pixel => pixel.xyz[0] == 0)).IsTrue();
            Check.That(film.Pixels.All(pixel => pixel.xyz[1] == 0)).IsTrue();
            Check.That(film.Pixels.All(pixel => pixel.xyz[2] == 0)).IsTrue();

            filmTile.AddSample(Point2F.Zero, new Spectrum(SampledSpectrum.NSpectralSamples, 1));
            film.MergeFilmTile(filmTile);
            Check.That(filmTile.Pixels.All(pixel => pixel.contribSum.IsBlack())).IsFalse();
        }

        [Test]
        public void SetImageTest()
        {
            var film = new Film(320, 200);
            Spectrum[] img = Enumerable.Range(0, 320*200).Select(i => new Spectrum(SampledSpectrum.NSpectralSamples, i)).ToArray();
            film.SetImage(img);

            for (int i = 0; i < 320 * 200; i++)
            {
                var x = i % 320;
                var y = i / 320;
                var point2I = new Point2I(x, y);
                var pixel = film.GetPixel(point2I);
                var xyz = img[i].ToXYZ();
                Check.That(pixel.xyz[0]).IsEqualTo(xyz[0]);
                Check.That(pixel.xyz[1]).IsEqualTo(xyz[1]);
                Check.That(pixel.xyz[2]).IsEqualTo(xyz[2]);
                Check.That(pixel.splatXYZ[0]).IsZero();
                Check.That(pixel.splatXYZ[1]).IsZero();
                Check.That(pixel.splatXYZ[2]).IsZero();
                Check.That(pixel.filterWeightSum).IsEqualTo(1);
            }
        }

        [Test]
        public void AddSplatTest()
        {
            var film = new Film(320, 200);
            var point = new Point2F(0, 0);
            var pixel = film.GetPixel((Point2I)point);
            Check.That(pixel.splatXYZ).ContainsExactly(0f, 0f, 0f);
            
            film.AddSplat(new Point2F(-1, 0), null);
            
            pixel = film.GetPixel((Point2I)point);
            Check.That(pixel.splatXYZ).ContainsExactly(0f, 0f, 0f);

            point = new Point2F(0, 0);
            film.AddSplat(point, new Spectrum(SampledSpectrum.NSpectralSamples, 1));
            
            pixel = film.GetPixel((Point2I)point);
            Check.That(pixel.splatXYZ).ContainsExactly(0.9974301f ,0.99941075f, 0.99511874f);
        }

        [Test]
        public void WriteImageTest()
        {
            var n = 100;
            var film = new Film(n, n);
            var filterTable = Enumerable.Range(0, n*n).Select(i => 1f).ToArray();
            FilmTile filmTile = new FilmTile(new Bounds2I(new Point2I(0,0), new Point2I(n-1, n-1)), new Vector2F(1,1), filterTable, n);
            var gray = new Spectrum(SampledSpectrum.FromRgb(new[] { 0.5f, 0.5f, 0.5f }, SpectrumType.Illuminant));

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var sampledSpectrum = new Spectrum(SampledSpectrum.FromRgb(new[] { 0, (float)i/n, (float)j/n }, SpectrumType.Illuminant));
                    film.AddSplat(new Point2F(i, j), sampledSpectrum);

                    filmTile.AddSample(new Point2F(i, j), gray, sampleWeight: 2f);
                }
            }
            
            film.MergeFilmTile(filmTile);
            var rgb = film.GetRgb();

#if WriteFile
            var tempFileName = Path.GetTempFileName();
            tempFileName = Path.ChangeExtension(tempFileName, "bmp");
            
            bmp.Save(tempFileName);
            ProcessStartInfo startInfo = new ProcessStartInfo(tempFileName)
            {
                Verb = "open"
            };
            Process.Start(startInfo);
            File.Delete(tempFileName);
#endif
        }
    }
}