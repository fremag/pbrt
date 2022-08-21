using pbrt.Core;

namespace Pbrt.Demos.Mesh
{
    public class CurveSweepMesh : AbstractMesh
    {
        public CurveSweepMesh(int n, int m, Path3D path, Curve2D curve) : this(n, m, new Path3DAdapter(path), new Curve2DAdapter(curve))
        {
            
        } 
        
        public CurveSweepMesh(int n, int m, IPath3D path, ICurve2D curve) : base(n, m)
        {
            var vectorY = new Vector3F(0, 1, 0);
            for(int i=0; i < n; i++)
            {
                float u = i * 1.0f / n;
                path.GetPoint(u, out var x0, out var y0, out var z0);
                path.GetPoint(u+0.000001, out var x1, out var y1, out var z1);
                var xTgt = x1-x0;
                var yTgt = y1-y0;
                var zTgt = z1-z0;
                var tgt = new Vector3F((float)xTgt, (float)yTgt, (float)zTgt);
                var rotation = Transform.Rotation(vectorY, tgt.Normalized());
                
                for (int j = 0; j < m; j++)
                {
                    double v = j * 1.0 / m;
                    curve.GetPoint(u, v, out double cx, out double cy);
                    var transformPoint = rotation * new Vector3F((float)cx, 0, (float)cy);
                    double x = x0 + transformPoint.X;
                    double y = y0 + transformPoint.Y;
                    double z = z0 + transformPoint.Z;
                    
                    Points[i][j] = new Point3F((float)x, (float)y, (float)z);
                }
            }
        }
    }
}