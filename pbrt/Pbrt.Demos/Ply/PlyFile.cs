using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Pbrt.Demos.Ply;

public class PlyFile
{
    private readonly List<string> requestedElements = new ();
    private readonly Dictionary<string, DataCursor> userDataTable = new ();
    private List<string> Comments { get; } = new();
    private List<PlyElement> Elements { get; } = new();
    private List<string> ObjInfo { get; } = new();
    private bool IsBinary { get; set; }
    private bool IsBigEndian { get; set; }

    public PlyFile(Stream stream)
    {
        var reader = new UnbufferedStreamReader(stream);
        ParseHeader(reader);
    }

    public void Read(Stream stream)
    {
        if (IsBinary)
        {
            ReadBinaryInternal(stream);
        }
        else
        {
            ReadTextInternal(stream);
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
            if (!requestedElements.Contains(elementKey))
            {
                requestedElements.Add(elementKey);
            }
        }
        else
        {
            return 0;
        }

        var cursor = new DataCursor
        {
            Vector = data
        };

        var instanceCounts = new List<int>();
        var propertyKeyList = propertyKeys.ToList();

        foreach (var key in propertyKeyList)
        {
            var instanceCount = InstanceCounter<T>(key, elementKey);
            if (instanceCount != 0)
            {
                instanceCounts.Add(instanceCount);
                var userKey = Helper.MakeKey(elementKey, key);
                if (userDataTable.ContainsKey(userKey))
                {
                    throw new Exception("property has already been requested: " + key);
                }

                userDataTable[userKey] = cursor;
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

        if (Elements.FindIndex(x => x.Name == elementKey) >= 0)
        {
            if (!requestedElements.Contains(elementKey))
            {
                requestedElements.Add(elementKey);
            }
        }
        else
        {
            return;
        }

        var cursor = new DataCursor();

        var userKey = Helper.MakeKey(elementKey, propertyKey);
        if (userDataTable.ContainsKey(userKey))
        {
            throw new Exception("property has already been requested: " + propertyKey);
        }
        userDataTable[userKey] = cursor;
        cursor.Vector = data;
        cursor.IsMultiVector = true;
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
            var idx = requestedElements.FindIndex(x => x == element.Name);
            if (idx != -1)
            {
                for (long count = 0; count < element.Size; ++count)
                {
                    foreach (var property in element.Properties)
                    {
                        DataCursor cursor;
                        if (userDataTable.TryGetValue(Helper.MakeKey(element.Name, property.Name), out cursor))
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
       
    private void ParseHeader(TextReader stream)
    {
        bool endOfHeader = false;
        while(! endOfHeader)
        {
            var line = stream.ReadLine();
            if (line == null)
            {
                break;
            }
            if(string.IsNullOrEmpty(line))
            {
                continue;
            }
            
            using var ls = new StringReader(line);
            var token = ls.ReadWord();
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
                case "comment": 
                    ReadHeaderText(ls, Comments);
                    break;
                case "format":
                    ReadHeaderFormat(ls);
                    break;
                case "element":
                    ReadHeaderElement(ls);
                    break;
                
                case "property" :
                    ReadHeaderProperty(ls);
                    break;
                case "obj_info":
                    ReadHeaderText(ls, ObjInfo);
                    break;
                
                case "end_header":
                    endOfHeader = true;
                    break;
                
                default:
                    throw new Exception("invalid header");
            }
        }
    }
}