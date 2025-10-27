using System;
using System.Runtime.CompilerServices;
using MultiMap.Misc.Native;

namespace MultiMap.Misc.JsonSerializer;

public struct JsonParser
{
    public NativeList<JsonToken> Tokens = new NativeList<JsonToken>(128);
    public NativeList<char> Chars = new NativeList<char>(256);
    private bool Disposed;
    public int CurrentIndex = 0;
    public JsonToken GetCurrentToken => Tokens[CurrentIndex];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JsonToken CurrentToken()
    {
        if (Tokens.Count <= CurrentIndex )
        {
            throw new IndexOutOfRangeException($"Tried accessing a token out of range, forgot to close a bracket?");
        }
        return Tokens[CurrentIndex];
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public JsonToken NextToken()
    {
        if (Tokens.Count <= CurrentIndex + 1)
        {
            throw new IndexOutOfRangeException($"Tried accessing a token out of range, forgot to close a bracket?");
        }
        return Tokens[++CurrentIndex];
    }
    
    public JsonParser()
    {
        
    }

    public ReadOnlySpan<char> ReadToken(JsonToken token)
    {
        var span = Chars.AsReadOnlySpan();
        return span.Slice(token.StartIndex, token.Length);
    }
    
    public unsafe void WriteString(ReadOnlySpan<char> json)
    {
        var startIndex = Chars.Count;
        Chars.SimulateAdd(json.Length);
        char* dest = Chars.Buffer + startIndex;
        fixed (char* src = json)
        {
            Buffer.MemoryCopy(src,  dest, json.Length * sizeof(char), json.Length * sizeof(char));
        }
    }
    
    public void Dispose()
    {
        if (Disposed)
        {
            //todo Warn
            return;
        }
        Disposed = true;
        Tokens.Dispose();
        Chars.Dispose();
    }
}