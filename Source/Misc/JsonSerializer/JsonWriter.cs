using System;
using MultiMap.Misc.Native;

namespace MultiMap.Misc.JsonSerializer;

public struct JsonWriter
{
    public NativeList<char> Result = new NativeList<char>(256);
    private bool Disposed = false;
    
    public JsonWriter()
    {
    }

    public unsafe void Write(ReadOnlySpan<char> value)
    {
        var startIndex = Result.Count;
        Result.SimulateAdd(value.Length);
        char* dest = Result.Buffer + startIndex;
        fixed (char* src = value)
        {
            Buffer.MemoryCopy(src,  dest, value.Length * sizeof(char), value.Length * sizeof(char));
        }
    }

    public void Write(char c)
    {
        Result.Add(c);
    }

    public void Dispose()
    {
        if (Disposed)
        {
            // todo warn
            return;
        }
        Disposed = true;
        Result.Dispose();
    }
}