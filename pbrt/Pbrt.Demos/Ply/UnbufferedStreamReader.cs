using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Pbrt.Demos.Ply;

public class UnbufferedStreamReader : TextReader
{
    private Stream s;

    public UnbufferedStreamReader(Stream stream)
    {
        s = stream;
    }
        
    public override string ReadLine()
    {
        List<byte> bytes = new List<byte>();
        int current;
        while ((current = Read()) != -1 && current != '\n')
        {
            byte b = (byte)current;
            bytes.Add(b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
    }

    public override int Read()
    {
        return s.ReadByte();
    }
    protected override void Dispose(bool disposing)
    {
        s?.Dispose();
        s = null;
    }

    public override int Peek()
    {
        throw new NotImplementedException();
    }

    public override int Read(char[] buffer, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override int ReadBlock(char[] buffer, int index, int count)
    {
        throw new NotImplementedException();
    }

    public override string ReadToEnd()
    {
        throw new NotImplementedException();
    }
}