using System;
using pbrt.Core;
using System.Threading;

namespace pbrt.Integrators
{
    public abstract class Integrator
    {
        public event Action<int, int, TimeSpan> TileRendered;
        public abstract float[] Render(IScene scene, CancellationToken cancellationToken);

        protected virtual void OnTileRendered(int numTile, int maxtime, TimeSpan time)
        {
            TileRendered?.Invoke(numTile, maxtime, time);
        }
    }
}