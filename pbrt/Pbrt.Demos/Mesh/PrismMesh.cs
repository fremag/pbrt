using System.Collections.Generic;
using System.Linq;
using pbrt.Core;

namespace Pbrt.Demos.Mesh
{
    public class PrismMesh : AbstractMesh
    {
        public PrismMesh(IEnumerable<Point2F> points, bool close=false) : base(points.Count(), close ? 4 : 2)
        {
            int i = 0;
            foreach(var p in points)
            {
                if (close)
                {
                    Points[i][0] = new Point3F(0, 0, 0);
                    Points[i][1] =  new Point3F(p.X, 0, p.Y);
                    Points[i][2] =  new Point3F(p.X, 1, p.Y);
                    Points[i][3] =  new Point3F(0, 1, 0);
                }
                else
                {
                    Points[i][0] =  new Point3F(p.X, 0, p.Y);
                    Points[i][1] = new Point3F(p.X, 1, p.Y);
                }

                i++;
            }
        }
    }
}