using System.Collections.Generic;
using pbrt.Core;
using pbrt.Shapes;

namespace Pbrt.Demos.Scenes
{
    public class TriangleScene  : DemoScene
    {
        public TriangleScene()
        {
            Floor();
            AddPrimitives(BuildTriangles());
            PointLight(5, 15, -10, 500f);
        }

        private IEnumerable<IPrimitive> BuildTriangles()
        {
            var vertexIndices = new List<int>();
            var points = new List<Point3F>();

            vertexIndices.AddRange(new[]{0, 1, 2});
            points.Add((-1,0,1));
            points.Add((0,1,1));
            points.Add((1,0,1));

            vertexIndices.AddRange(new[]{3, 4, 5});
            points.Add((-3,0,0));
            points.Add((-1,1,0));
            points.Add((-2,0,0));

            vertexIndices.AddRange(new[]{6, 7, 8});
            points.Add((1,1,-2f));
            points.Add((0,0,-2f));
            points.Add((-1,1,-2f));

            int nbTriangles = vertexIndices.Count / 3;
            Transform transform = Translate(0, 1, 0);
            var mesh = new TriangleMesh(transform, nbTriangles, vertexIndices.ToArray(), vertexIndices.Count, points.ToArray(), null, null, null, null, null, null);

            var triangles = BuildTriangles(mesh, MatteMaterialGreen());
            return triangles;
        }
    }
}