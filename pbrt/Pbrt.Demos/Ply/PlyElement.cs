using System.Collections.Generic;
using System.IO;

namespace Pbrt.Demos.Ply
{
    public class PlyElement
    {
        public string Name { get; private set; }
        public int Size { get; private set; }
        public List<PlyProperty> Properties { get; }

        public PlyElement(TextReader stream)
        {
            Properties = new List<PlyProperty>();
            ParseInternal(stream);
        }

        private void ParseInternal(TextReader stream)
        {
            Name = stream.ReadWord();
            Size = int.Parse(stream.ReadWord());
        }
    }
}
