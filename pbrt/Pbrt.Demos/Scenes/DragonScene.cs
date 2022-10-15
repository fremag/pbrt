using System.Collections.Generic;
using System.IO;
using System.Linq;
using pbrt.Core;
using Pbrt.Demos.Ply;
using pbrt.Shapes;

namespace Pbrt.Demos.Scenes;

public class DragonScene : DemoScene
{
    public DragonScene()
    {
        var path = @"E:\Projects\pbrt-scenes\pbrt-v3-scenes\dragon\geometry\dragon_remeshed.ply";
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            var f = new PlyFile(stream);
            var xyz = new List<float>();
            f.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, xyz);
            var index = new List<List<int>>();
            f.RequestListPropertyFromElement("face", "vertex_indices", index);
            f.Read(stream);
            var nbTri = index.Count;
            var indexes = index.SelectMany(d => d).ToArray();
            var nbVertices = index.Count;
            Point3F[] points = Enumerable.Range(0, xyz.Count/3).Select(i => new Point3F(xyz[3*i], xyz[3*i+1], xyz[3*i+2])).ToArray();
            Transform transform1 =  (RotateY(45)*RotateZ(90)*RotateY(45));
            var scale = Scale(0.05f);
            transform1 = Translate(tY: 2.2f, tX: -3f) * transform1 * scale;
            TriangleMesh dragon1 = new TriangleMesh(transform1, nbTri, indexes, nbVertices, points, null, null, null, null, null, null);
            Transform transform2 =  (RotateY(-15)*RotateZ(90)*RotateY(45));
            transform2 = Translate(tY: 2.2f, tX: 5f) * transform2 * scale;
            TriangleMesh dragon2 = new TriangleMesh(transform2, nbTri, indexes, nbVertices, points, null, null, null, null, null, null);

            Floor();
            AddPrimitives(BuildTriangles(dragon1, PlasticMaterialGreen));
            AddPrimitives(BuildTriangles(dragon2, MakeMatteMaterial(30, 0.7f, 0.7f, 1f)));
            PointLight( 0, 1, -20, 500f);
            PointLight(0, 50, 0, 2500f);
        }
    }
}