using System.Diagnostics;
using Pbrt.Demos;

namespace pbrt_runner
{
    public class RenderProgress : IDisposable
    {
        private readonly int num;
        private readonly int max;
        public AbstractRenderer Renderer { get; }
        private int nbTiles;
        private int n;
        readonly Stopwatch sw = Stopwatch.StartNew();
        readonly Stopwatch swTotal = Stopwatch.StartNew();
        private readonly object objLock = new();
        
        public RenderProgress(AbstractRenderer renderer, int num, int max)
        {
            this.num = num;
            this.max = max;
            Renderer = renderer;
            renderer.Integrator.TileRendered += OnTileRendered;
            PrintStats();
        }

        private void PrintStats()
        {
            string msg = $"\r[{num,3} / {max,3}] {Renderer.GetType().Name,-40}: {swTotal.Elapsed:hh\\:mm\\:ss} [{n,5} / {nbTiles,5}] {(100f*n)/nbTiles,-5:##0.00} %";
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