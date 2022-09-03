using System;
using System.Collections.Generic;
using pbrt.Core;
using Pbrt.Demos.Scenes;
using pbrt.Shapes;

namespace Pbrt.Demos.Scenes
{
    public class TorusScene  : DemoScene
    {
        public TorusScene()
        {

            Floor();
            AddPrimitives(BuildTorus());
            PointLight(00, 15, -10, 500f);
        }

        private IEnumerable<IPrimitive> BuildTorus()
        {
            Transform transform = Transform.Translate(0, 1, 0) * Transform.RotateX(90) * Transform.RotateZ(45);
            int n = 30;
            int m = 30;
            float r1 = 2;
            float r2 = 1;
 
            var vertexIndices = new List<int>();
            var points = new List<Point3F>();

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var theta = 2 * (MathF.PI / n) * i;
                    var phi = 2 * (MathF.PI / m) * j;
                    var x = (r1 + r2 * MathF.Cos(theta))*MathF.Cos(phi);
                    var y =  (r1 + r2 * MathF.Cos(theta))*MathF.Sin(phi);
                    var z =  r2 * MathF.Sin(theta);
                    
                    points.Add(new Point3F(x, y, z));
                }
            }

            int Idx(int i, int j) => ((i % n)*n)+ (j%m); 
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    var idx = Idx(i,j);
                    var idx1 = Idx(i+1, j);
                    var idx2 = Idx(i, j+1);

                    vertexIndices.Add(idx);
                    vertexIndices.Add(idx1);
                    vertexIndices.Add(idx2);

                    idx = Idx(i+1,j+1);
                    idx1 = Idx(i+1, j);
                    idx2 = Idx(i, j+1);

                    vertexIndices.Add(idx);
                    vertexIndices.Add(idx1);
                    vertexIndices.Add(idx2);
                    
                }
            }

            int nbTriangles = vertexIndices.Count / 3;
            var mesh = new TriangleMesh(transform, nbTriangles, vertexIndices.ToArray(), vertexIndices.Count, points.ToArray(), null, null, null, null, null, null);
            var triangles = BuildTriangles(mesh, MatteMaterialGreen());
            return triangles;
        }
    }
}