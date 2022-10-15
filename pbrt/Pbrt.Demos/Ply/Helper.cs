using System;

namespace Pbrt.Demos.Ply
{
    public static class Helper
    {
        public static Type PropertyTypeFromString(string t)
        {
            switch (t)
            {
                case "int8":
                case "char":
                    return typeof(sbyte);
                case "uint8":
                case "uchar":
                    return typeof(byte);
                case "int16":
                case "short":
                    return typeof(short);
                case "uint16":
                case "ushort":
                    return typeof(ushort);
                case "int32":
                case "int":
                    return typeof(int);
                case "uint32":
                case "uint":
                    return typeof(uint);
                case "float32":
                case "float":
                    return typeof(float);
                case "float64":
                case "double":
                    return typeof(double);
                default:
                    return null;
            }
        }

        public static string MakeKey(string a, string b) => a + "-" + b;
    }
}
