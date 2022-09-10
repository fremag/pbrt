using System;
using System.Drawing;
using System.IO;
using NLog;
using Pbrt.Demos.renderers;

namespace Pbrt.Demos
{
    public class Program
    {
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();
        private static string ProjectDir = @"E:\Projects\pbrt\Images";
        
        static void Main(string[] args)
        {
            AbstractRenderer[] renderers = 
            {
                // new HelloWorldRenderer(),
                // new CheckerPlaneRenderer(),
                // new MirrorRenderer(),
                // new GlassRenderer(),
                // new CylinderRenderer(),
                // new DiskRenderer(),
                // new TorusRenderer(),
                // new TriangleRenderer(),
                // new CloverRenderer(),
                // new LensFocalSamplerTestRenderer(),
                new AreaLightRenderer(1),
                //new AreaLightRenderer(4),
                new AreaLightRenderer(16),
                //new AreaLightRenderer(64),
                new AreaLightRenderer(256),
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
            var img = renderer.Integrator.Render(renderer.Scene);
            if (renderer.Text != null)
            {
                using (Graphics g = Graphics.FromImage(img))
                {
                    var textFont = new Font("Microsoft Sans Serif", 9F);
                    g.DrawString(renderer.Text, textFont, renderer.Brush, 10, 10);
                }
            }
            img.Save(Path.Combine(ProjectDir, renderer.FileName));
        }
    }
}