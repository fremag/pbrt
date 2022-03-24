using System.Drawing;
using pbrt.Core;

namespace pbrt.Integrators
{
    public abstract class Integrator
    {
        public abstract Bitmap Render(Scene scene);
    }
}