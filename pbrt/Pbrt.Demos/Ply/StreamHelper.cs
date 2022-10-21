using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Pbrt.Demos.Ply;

public static class StreamHelper
{
    private static object FromByteArray(byte[] rawValue, Type t)
    {
        GCHandle handle = GCHandle.Alloc(rawValue, GCHandleType.Pinned);
        object structure = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
        handle.Free();
        return structure;
    }

    public static string ReadWord(this TextReader stream)
    {
        // skip whitespace
        while (stream.Peek() > 0 && char.IsWhiteSpace((char)stream.Peek()))
        {
            stream.Read();
        }

        string word = string.Empty;
        int i;
        while ((i = stream.Read()) > -1)
        {
            var c = (char)i;
            if (char.IsWhiteSpace(c))
            {
                break;
            }

            word += c;
        }
        return word;
    }

    public static object ReadBinaryData(this BinaryReader stream, Type t)
    {
        var size = Marshal.SizeOf(t);
        byte[] buf = new byte[size];
        stream.Read(buf, 0, size);
        return FromByteArray(buf, t);
    }

    public static object ReadBinaryDataBigEndian(this BinaryReader stream, Type t)
    {
        var size = Marshal.SizeOf(t);
        byte[] buf = new byte[size];
        stream.Read(buf, 0, size);
        switch (size)
        {
            case 2:
                (buf[0], buf[1]) = (buf[1], buf[0]);
                break;
            case 4:
                (buf[0], buf[3]) = (buf[3], buf[0]);
                (buf[1], buf[2]) = (buf[2], buf[1]);
                break;
            case 8:
                (buf[0], buf[7]) = (buf[7], buf[0]);
                (buf[1], buf[6]) = (buf[6], buf[1]);
                (buf[2], buf[5]) = (buf[5], buf[2]);
                (buf[3], buf[4]) = (buf[4], buf[3]);
                break;
        }

        return FromByteArray(buf, t);
    }
        
    public static void SkipBinaryData(this BinaryReader stream, Type t)
    {
        var size = Marshal.SizeOf(t);
        for (int i = 0; i < size; i++)
        {
            stream.ReadByte();
        }
    }
        
    public static object ReadTextData(this TextReader stream, Type t)
    {
        var w = stream.ReadWord();
        if (w.Equals("nan", StringComparison.OrdinalIgnoreCase))
        {
            w = "NaN";
        }
        return Convert.ChangeType(w, t);
    }

    public static void SkipTextData(this TextReader stream, Type t)
    {
        stream.ReadWord();
    }
}