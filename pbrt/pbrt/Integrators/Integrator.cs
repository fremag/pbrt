using System;
using pbrt.Core;
using System.Threading;
using pbrt.Samplers;

namespace pbrt.Integrators
{
    public abstract class Integrator
    {
        public event Action<int, int, TimeSpan> TileRendered;
        
        public abstract float[] Render(IScene scene, CancellationToken primaryCancellationToken, CancellationToken secondaryCancellationToken);
        public abstract void Preprocess(IScene scene, AbstractSampler sampler);

        protected void OnTileRendered(int numTile, int maxtime, TimeSpan time)
        {
            TileRendered?.Invoke(numTile, maxtime, time);
        }
    }
}