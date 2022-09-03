using System;
using System.Collections.Generic;
using pbrt.Core;
using Pbrt.Demos.Mesh;
using Pbrt.Demos.Scenes;
using pbrt.Shapes;

namespace Pbrt.Demos.Scenes
{
    public class CloverScene  : DemoScene
    {
        public CloverScene()
        {
            Floor();
            
            AddPrimitives(BuildClover());
            PointLight(0, 2, -2, 8f);
        }

        private IEnumerable<IPrimitive> BuildClover()
        {
            const int n = 360;
            const int m = 20;
            const double r1 = 0.5;
            const double r2 = 0.25;

            void CloverPath(double t, out double x, out double y, out double z)
            {
                var alpha = 2 * MathF.PI * t;
                x = r1 * (Math.Cos(alpha) + 2 * Math.Cos(3 * alpha));
                y = r1 * (Math.Sin(alpha) - 2 * Math.Sin(3 * alpha));
                z = 2 * r2 * Math.Sin(4 * alpha);
            }

            void CircleCurve(double u, double v, out double x, out double y)
            {
                x = r2 * Math.Cos(2 * MathF.PI* v);
                y = r2 * Math.Sin(2 * MathF.PI* v);
            }

            CurveSweepMesh cloverMesh = new CurveSweepMesh(n, m, CloverPath, CircleCurve);
            Point3F[] points = new Point3F[n*m];

            var vertexIndices = new List<int>();
            int Idx(int i, int j) => ((i % n)*m)+ (j%m); 
            for (int i = 0; i < n; i++)
            {
                var cloverMeshPoint = cloverMesh.Points[i];

                for (int j = 0; j < m; j++)
                {
                    var index = i * m + j;
                    points[index] = cloverMeshPoint[j];
                    
                    var idx = Idx(i, j);
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
            var transform = Translate(0, 2, 0);
            var mesh = new TriangleMesh(transform, nbTriangles, vertexIndices.ToArray(), vertexIndices.Count, points, null, null, null, null, null, null);
            
            var triangles =  BuildTriangles(mesh,  MatteMaterialGreen());
            return triangles;
        }
    }
}