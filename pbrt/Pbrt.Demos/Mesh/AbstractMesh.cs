using pbrt.Core;

namespace Pbrt.Demos.Mesh
{
    public interface IMesh
    {
        int N { get; }
        int M { get; }
        Point3F[][] Points { get; }
    }

    public abstract class AbstractMesh : IMesh
    {
        public int N { get; }
        public int M { get; }
        public Point3F[][] Points { get; }

        public AbstractMesh(int n, int m)
        {
            N = n;
            M = m;
            Points = new Point3F[n][];
            for (int i = 0; i < n; i++)
            {
                Points[i] = new Point3F[m];
            }
        }
    }
}