using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Pbrt.Demos.Ply;

public class PlyFile : IDisposable
{
    private Stream Stream { get; }
    private List<string> Comments { get; } = new();
    private List<PlyElement> Elements { get; } = new();
    private List<string> ObjInfo { get; } = new();
    private bool IsBinary { get; set; }
    private bool IsBigEndian { get; set; }
    private List<string> RequestedElements { get; } = new ();
    private Dictionary<string, DataCursor> UserDataTable { get; } = new ();

    public PlyFile(Stream stream)
    {
        Stream = stream;
        ParseHeader(stream);
    }

    public PlyFile(string path) : this(new FileStream(path, FileMode.Open, FileAccess.Read))
    {
    }

    public void Read()
    {
        if (IsBinary)
        {
            ReadBinaryInternal(Stream);
        }
        else
        {
            ReadTextInternal(Stream);
        }
    }

    public int RequestPropertyFromElement<T>(string elementKey, IEnumerable<string> propertyKeys, List<T> data)
    {
        if (!Elements.Any())
        {
            return 0;
        }

        if (Elements.FindIndex(x => x.Name == elementKey) >= 0)
        {
            if (!RequestedElements.Contains(elementKey))
            {
                RequestedElements.Add(elementKey);
            }
        }
        else
        {
            return 0;
        }

        var cursor = new DataCursor(data);

        var instanceCounts = new List<int>();
        var propertyKeyList = propertyKeys.ToList();

        foreach (var key in propertyKeyList)
        {
            var instanceCount = InstanceCounter<T>(key, elementKey);
            if (instanceCount != 0)
            {
                instanceCounts.Add(instanceCount);
                var userKey = Helper.MakeKey(elementKey, key);
                if (UserDataTable.ContainsKey(userKey))
                {
                    throw new Exception("property has already been requested: " + key);
                }

                UserDataTable[userKey] = cursor;
            }
            else
            {
                return 0;
            }
        }

        var totalInstanceSize = instanceCounts.Sum(x => x);
        return totalInstanceSize / propertyKeyList.Count;
    }

    private int InstanceCounter<T>(string propertyKey, string elementKey)
    {
        foreach (var e in Elements)
        {
            if (e.Name != elementKey)
            {
                continue;
            }

            foreach (var p in e.Properties)
            {
                if (p.Name == propertyKey)
                {
                    if (typeof(T) != p.PropertyType)
                    {
                        throw new Exception("destination vector is wrongly typed to hold this property");
                    }

                    return e.Size;
                }
            }
        }
        return 0;
    }

    public void RequestListPropertyFromElement<T>(string elementKey, string propertyKey, List<List<T>> data)
    {
        if (!Elements.Any())
        {
            return;
        }

        if (Elements.FindIndex(x => x.Name == elementKey) < 0)
        {
            return;
        }

        if (!RequestedElements.Contains(elementKey))
        {
            RequestedElements.Add(elementKey);
        }

        var userKey = Helper.MakeKey(elementKey, propertyKey);
        if (UserDataTable.ContainsKey(userKey))
        {
            throw new Exception("property has already been requested: " + propertyKey);
        }
        UserDataTable[userKey] = new DataCursor(data, true);
    }

    private void ReadHeaderText(TextReader line, List<string> comments)
    {
        var l = line.ReadLine();
        comments.Add(l);
    }

    private void ReadHeaderFormat(TextReader line)
    {
        var s = line.ReadWord();
        switch (s)
        {
            case "binary_little_endian":
                IsBinary = true;
                IsBigEndian = false;
                break;
            case "binary_big_endian":
                IsBinary = true;
                IsBigEndian = true;
                break;
        }
    }

    private void ReadHeaderProperty(TextReader stream)
    {
        Elements.Last().Properties.Add(new PlyProperty(stream));
    }

    private void ReadHeaderElement(TextReader stream)
    {
        Elements.Add(new PlyElement(stream));
    }

    private void Read(Func<Type, object> readData, Action<Type> skipData)
    {
        foreach (var element in Elements)
        {
            var idx = RequestedElements.FindIndex(x => x == element.Name);
            if (idx == -1)
            {
                continue;
            }

            for (long count = 0; count < element.Size; ++count)
            {
                foreach (var property in element.Properties)
                {
                    if (UserDataTable.TryGetValue(Helper.MakeKey(element.Name, property.Name), out var cursor))
                    {
                        if (property.IsList)
                        {
                            var listSize = Convert.ToUInt32(readData(property.ListType));

                            IList sourceList = cursor.Vector;
                            if (cursor.IsMultiVector)
                            {
                                var listType = typeof(List<>);
                                var listGenericType = listType.MakeGenericType(property.PropertyType);
                                var list = (IList)Activator.CreateInstance(listGenericType);
                                sourceList.Add(list);
                                sourceList = list;
                            }

                            for (var i = 0; i < listSize; ++i)
                            {
                                sourceList.Add(readData(property.PropertyType));
                            }
                        }
                        else
                        {
                            cursor.Vector.Add(readData(property.PropertyType));
                        }
                    }
                    else
                    {
                        skipData(property.PropertyType);
                    }
                }
            }
        }
    }

    private void ReadBinaryInternal(Stream stream)
    {
        var bs = new BinaryReader(stream);
        if (IsBigEndian)
        {
            Read(bs.ReadDataBigEndian, bs.SkipData);
        }
        else
        {
            Read(bs.ReadData, bs.SkipData);
        }
    }

    private void ReadTextInternal(Stream stream)
    {
        var sr = new StreamReader(stream);
        {
            Read(sr.ReadData, sr.SkipData);
        }
        sr.DiscardBufferedData();
    }

    private string ReadLine(Stream stream)
    {
        var bytes = new List<byte>();
        int current;
        while ((current = stream.ReadByte()) != -1 && current != '\n')
        {
            byte b = (byte)current;
            bytes.Add(b);
        }
        return Encoding.UTF8.GetString(bytes.ToArray(), 0, bytes.Count);
    }
    
    private void ParseHeader(Stream stream)
    {
        bool endOfHeader = false;
        while(! endOfHeader)
        {
            var line = ReadLine(stream);
            if (line == null)
            {
                break;
            }
            
            if(string.IsNullOrEmpty(line))
            {
                continue;
            }
            
            using var stringReader = new StringReader(line);
            var token = stringReader.ReadWord();
            if (string.IsNullOrEmpty(token))
            {
                continue;
            }
            
            if (token.Equals("ply", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            
            switch(token)
            {
                case "end_header":
                    endOfHeader = true;
                    break;
                
                case "format":
                    ReadHeaderFormat(stringReader);
                    break;
                
                case "element":
                    ReadHeaderElement(stringReader);
                    break;
                
                case "property" :
                    ReadHeaderProperty(stringReader);
                    break;
                
                case "comment": 
                    ReadHeaderText(stringReader, Comments);
                    break;
                
                case "obj_info":
                    ReadHeaderText(stringReader, ObjInfo);
                    break;
                
                default:
                    throw new Exception("invalid header");
            }
        }
    }

    public void Dispose()
    {
        Stream?.Dispose();
    }
}