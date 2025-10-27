using System;
using System.IO;
using System.Text;
using MultiMap.Misc.Native;

namespace MultiMap.Misc.JsonSerializer;

public static partial class JsonSerializer
{
    public static void SerializeToPath(object obj, string path)
    {
        File.Delete(path);
        NativeArray<char> serialized = SerializeToUnsafe(obj);
        using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            var encoder = Encoding.UTF8.GetEncoder();
            ReadOnlySpan<char> span = serialized.AsSpan();
            Span<byte> buffer = stackalloc byte[4096];

            int charUsed, byteUsed;
            while (!span.IsEmpty)
            {
                encoder.Convert(span, buffer, true, out charUsed, out byteUsed, out _);
                fileStream.Write(buffer.Slice(0, byteUsed));
                span = span.Slice(charUsed);
            }
        }
    }

    public static T DeserializeFromPath<T>(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Could not find file {path} to serialize");
        }

        var chars = new NativeList<char>(4096);
        var decoder = Encoding.UTF8.GetDecoder();
        Span<byte> buffer = stackalloc byte[4096];
        Span<char> charBuffer = stackalloc char[4096];
        using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            int bytesRead;
            int charUsed;
            while ((bytesRead = fileStream.Read(buffer)) > 0)
            {
                decoder.Convert(buffer[..bytesRead], charBuffer, false, out _, out charUsed, out _);

                chars.AddRange(charBuffer[..charUsed]);
            }
            
            decoder.Convert(ReadOnlySpan<byte>.Empty, charBuffer, true, out _, out charUsed, out _);
            
            chars.AddRange(charBuffer[..charUsed]);
        }
        
        return DeSerialize<T>(chars.AsReadOnlySpan());
    }
}