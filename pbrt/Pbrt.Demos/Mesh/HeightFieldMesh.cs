using pbrt.Core;

namespace Pbrt.Demos.Mesh
{
    public class HeightField : AbstractMesh
    {
        public HeightField(int n, int m, Func1D func) : base(n, m)
        {
            for (int i = 0; i < n; i++)
            {
                double u = i * 1.0 / n;
                for (int j = 0; j < n; j++)
                {
                    double v = j * 1.0 / m;
                    double x = -0.5 + u;
                    double y = func(u, v);
                    double z = -0.5 + v;
                    Points[i][j] = new Point3F((float)x, (float)y, (float)z);
                }
            }
        }
    }
}