using System.Collections.Generic;
using System.IO;
using System.Linq;
using pbrt.Core;
using pbrt.Shapes;

namespace Pbrt.Demos.Ply;

public class MeshFactory
{
    public Point3F[] Points { get; private set; }
    public int[] Indexes { get; private set;}

    public MeshFactory(string path) : this(File.OpenRead(path))
    {
    }

    public MeshFactory(Stream stream)
    {
        using var plyFile = new PlyFile(stream);
        Init(plyFile);
    }

    private void Init(PlyFile plyFile)
    {
        var xyz = new List<float>();
        var index = new List<List<int>>();
        plyFile.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, xyz);
        plyFile.RequestListPropertyFromElement("face", "vertex_indices", index);
        plyFile.Read();
        Indexes = index.SelectMany(d => d).ToArray();
        Points = Enumerable.Range(0, xyz.Count/3).Select(i => new Point3F(xyz[3*i], xyz[3*i+1], xyz[3*i+2])).ToArray();
    }

    public TriangleMesh BuildTriangleMesh(Transform transform)
    {
        var triangleMesh = new TriangleMesh(transform, Indexes.Length/3, Indexes, Indexes.Length, Points, null, null, null, null, null, null);
        return triangleMesh;
    }
}