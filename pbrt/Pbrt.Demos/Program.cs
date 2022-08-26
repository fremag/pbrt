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
            // Render(new HelloWorldRenderer());
            // Render(new CheckerPlaneRenderer());
            // Render(new MirrorRenderer());
            // Render(new GlassRenderer());
            // Render(new CylinderRenderer());
            // Render(new DiskRenderer());
            // Render(new TorusRenderer());
            // Render(new TriangleRenderer());
            // Render(new CloverRenderer());
            Render(new LensFocalSamplerTestRenderer());
            Console.WriteLine("Done.");
        }

        private static void Render(AbstractRenderer renderer)
        {
            using var progress = new RenderProgress(renderer);
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