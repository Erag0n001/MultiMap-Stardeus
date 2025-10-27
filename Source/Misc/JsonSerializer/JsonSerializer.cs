using System;
using MultiMap.Misc.Native;

namespace MultiMap.Misc.JsonSerializer;

public static partial class JsonSerializer
{
    public static bool IsDebugging = false;

    public static char[] SerializeToBytes(object obj)
    {
        var data = SerializeToUnsafe(obj);
        var result = data.AsManagedArray();
        data.Dispose();
        return result;
    }
    
    public static NativeArray<char> SerializeToUnsafe(object obj)
    {
        var writer = GetWriter();
        try
        {
            SerializeValue(ref writer, obj);
            return writer.Result.AsArray();
        }
        finally
        {
            writer.Result.Clear();
            ReturnWriter(writer);
        }
    }

    public static T DeSerialize<T>(ReadOnlySpan<char> json)
    {
        var parser = GetParser();
        try
        {
            TokenParser.ExtractTokensFromStringUnsafe(json, ref parser);
            if (IsDebugging)
            {
                Console.WriteLine($"Tokens found while decoding json:");
                for (int t = 0; t < parser.Tokens.Count; t++)
                {
                    Console.WriteLine(t + "|" + parser.Tokens[t].Type);
                }
            }

            return (T)DeserializeToken(ref parser, typeof(T), ref parser.CurrentIndex);
        }
        finally
        {
            parser.Chars.Clear();
            parser.Tokens.Clear();
            parser.CurrentIndex = 0;
            ReturnParser(parser);
        }
    }
    
    public static T DeSerialize<T>(string json)
    {
        return DeSerialize<T>(json.AsSpan());
    }
    
}