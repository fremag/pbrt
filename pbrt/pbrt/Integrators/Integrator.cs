using System;
using System.Drawing;
using pbrt.Core;

namespace pbrt.Integrators
{
    public abstract class Integrator
    {
        public event Action<int, int, TimeSpan> TileRendered;
        public abstract Bitmap Render(IScene scene);

        protected virtual void OnTileRendered(int numTile, int maxtime, TimeSpan time)
        {
            TileRendered?.Invoke(numTile, maxtime, time);
        }
    }
}