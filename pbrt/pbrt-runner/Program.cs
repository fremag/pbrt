using System.Drawing;
using System.Drawing.Imaging;
using NLog;
using pbrt.Core;
using Pbrt.Demos;
using Pbrt.Demos.renderers;

namespace pbrt_runner
{
    public class Program
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static string ProjectDir = @"E:\Projects\pbrt\Images";
        
        static void Main(string[] args)
        {
            AbstractRenderer[] renderers = 
            {
                new HelloWorldRenderer(Environment.ProcessorCount),
                // new CheckerPlaneRenderer(),
                // new MirrorRenderer(),
                // new GlassRenderer(),
                // new CylinderRenderer(),
                // new DiskRenderer(),
                // new TorusRenderer(),
                // new TriangleRenderer(),
                // new CloverRenderer(),
                // new LensFocalSamplerTestRenderer(),
                //new AreaLightRenderer(1),
                // new AreaLightRenderer(4),
                // new AreaLightRenderer(16),
                // new AreaLightRenderer(64),
                // new AreaLightRenderer(256),
            };

            for (int i = 0; i < renderers.Length; i++)
            {
                Render(renderers[i], i+1, renderers.Length);
            }
            Console.WriteLine("Done.");
        }

        private static void Render(AbstractRenderer renderer, int num, int max)
        {
            renderer.Init();
            using var progress = new RenderProgress(renderer, num,  max);
            var rgbs = renderer.Integrator.Render(renderer.Scene, CancellationToken.None);
            var img = WriteImage(rgbs, renderer.Camera.Film.FullResolution);
            if (renderer.Text != null)
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    var textFont = new Font("Microsoft Sans Serif", 9F);
                    g.DrawString(renderer.Text, textFont, Brushes.White, 10, 10);
                }
            }
            
            img.Save(Path.Combine(ProjectDir, renderer.FileName));
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