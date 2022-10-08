using System.Drawing;
using System.Drawing.Imaging;
using NLog;
using pbrt.Core;
using Pbrt.Demos;
using Pbrt.Demos.Demos;

namespace pbrt_runner
{
    public class Program
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static string ProjectDir = @"E:\Projects\pbrt\Images";

        static void Main(string[] args)
        {
            AbstractDemo[] demos =
            {
                new HelloWorldDemo(),
                // new CheckerPlaneDemo(),
                // new MirrorDemo(),
                // new GlassDemo(),
                // new CylinderDemo(),
                // new DiskDemo(),
                // new TorusDemo(),
                // new TriangleDemo(),
                // new CloverDemo(),
                // new LensFocalSamplerTestDemo(),
                // new AreaLightDemo(1),
                // new AreaLightDemo(4),
                // new AreaLightDemo(16),
                // new AreaLightDemo(64),
                // new AreaLightDemo(256),
            };

            for (int i = 0; i < demos.Length; i++)
            {
                Render(demos[i], i + 1, demos.Length);
            }

            Console.WriteLine("Done.");
        }

        private static void Render(AbstractDemo demo, int num, int max)
        {
            using var progress = new RenderProgress(demo, num, max);
            demo.Init(progress.OnTileRendered);
            var rgbs = demo.Render(CancellationToken.None);
            var img = WriteImage(rgbs, demo.Camera.Film.FullResolution);
            if (demo.Text != null)
            {
                using var graphics = Graphics.FromImage(img);
                var textFont = new Font("Microsoft Sans Serif", 9F);
                graphics.DrawString(demo.Text, textFont, Brushes.White, 10, 10);
            }

            img.Save(Path.Combine(ProjectDir, demo.FileName));
        }

        private static Image WriteImage(float[] rgbs, Point2I fullResolution)
        {
            // Write RGB image
            Bitmap bmp = new Bitmap(fullResolution.X, fullResolution.Y, PixelFormat.Format24bppRgb);
            int pos = 0;
            for (int y = 0; y < fullResolution.Y; y++)
            {
                for (int x = 0; x < fullResolution.X; x++)
                {
                    var red = (int)rgbs[pos];
                    var green = (int)rgbs[pos + 1];
                    var blue = (int)rgbs[pos + 2];
                    var r = Math.Min(255, red);
                    var g = Math.Min(255, green);
                    var b = Math.Min(255, blue);

                    var fromArgb = Color.FromArgb(r, g, b);
                    bmp.SetPixel(x, y, fromArgb);
                    pos += 3;
                }
            }

            return bmp;
        }
    }
}