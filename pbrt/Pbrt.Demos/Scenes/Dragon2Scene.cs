using System.Collections.Generic;
using System.IO;
using System.Linq;
using pbrt.Core;
using Pbrt.Demos.Ply;
using pbrt.Shapes;

namespace Pbrt.Demos.Scenes;

public class Dragon2Scene : DemoScene
{
    public Dragon2Scene()
    {
        var path = @"E:\Projects\pbrt-scenes\pbrt-v3-scenes\sssdragon\geometry\dragon_mini.ply";
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
            Transform transform1 =  RotateY(45);
            var scale = Scale(0.1f);
            transform1 = Translate(tY: 4f, tX: -2f) * transform1 * scale;
            TriangleMesh dragon1 = new TriangleMesh(transform1, nbTri, indexes, nbVertices, points, null, null, null, null, null, null);
            
            Floor();
            AddPrimitives(BuildTriangles(dragon1, PlasticMaterialGreen));
            PointLight( 0, 3, -20, 300f);
            PointLight( 0, 12, 0, 300f);
        }
    }
}