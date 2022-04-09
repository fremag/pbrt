using System;
using System.Diagnostics;
using System.Threading;

namespace Pbrt.Demos
{
    internal class RenderProgress : IDisposable
    {
        public AbstractRenderer Renderer { get; }
        private int nbTiles;
        private int n;
        Stopwatch sw = Stopwatch.StartNew();
        Stopwatch swTotal = Stopwatch.StartNew();
        private object objLock = new object();
        public RenderProgress(AbstractRenderer renderer)
        {
            Renderer = renderer;
            renderer.Integrator.TileRendered += OnTileRendered;
            PrintStats();
        }

        private void PrintStats()
        {
            string msg = $"\r{Renderer.GetType().Name,20}: {n/(float)nbTiles:p2} {swTotal.Elapsed:hh\\:mm\\:ss\\.fff}";
            Console.Write(msg);
        }

        private void OnTileRendered(int numTile, int nbTiles, TimeSpan time)
        {
            this.nbTiles = nbTiles;
            Interlocked.Increment(ref n);
            lock (objLock)
            {
                if (sw.ElapsedMilliseconds > 1_000)
                {
                    sw.Restart();
                    PrintStats();
                }
            }
        }

        public void Dispose()
        {
            Renderer.Integrator.TileRendered -= OnTileRendered;
            PrintStats();
            Console.WriteLine();
        }
    }
}