using System;
using System.Collections.Generic;
using NUnit.Framework;
using Pbrt.Demos.Ply;

namespace Pbrt.Tests.Ply;

[TestFixture]
public class PlyTests
{
    [Test]
    [Ignore("Manual test only)")]
    public void Test()
    {
        var path = @"E:\Projects\pbrt-scenes\pbrt-v3-scenes\sssdragon\geometry\meshes_0.ply";
        using var f = new PlyFile(path);
        
        var xyz = new List<float>();
        f.RequestPropertyFromElement("vertex", new[] { "x", "y", "z" }, xyz);
        var index = new List<List<int>>();
        f.RequestListPropertyFromElement("face", "vertex_indices", index);
        f.Read();

        foreach (var e in xyz)
        {
            Console.WriteLine("{0}", e);
        }

        foreach (var i in index)
        {
            Console.WriteLine("{0}", i.Count);
        }
    }
}