using System;
using System.IO;

namespace Pbrt.Demos.Ply;

public class PlyProperty
{
    public Type ListType { get; }
    public Type PropertyType { get; }
    public bool IsList { get; }
    public string Name { get; }
        
    public PlyProperty(TextReader stream)
    {
        var t = stream.ReadWord();
        if (t == "list")
        {
            var countType = stream.ReadWord();
            t = stream.ReadWord();
            ListType = Helper.PropertyTypeFromString(countType);
            IsList = true;
        }
        PropertyType = Helper.PropertyTypeFromString(t);
        Name = stream.ReadWord();
    }
}