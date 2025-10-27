using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace MultiMap.Misc.JsonSerializer;

public static partial class JsonSerializer
{
    private static readonly string Null = "null";
    private static readonly string True = "true";
    private static readonly string False = "false";
    private static readonly string DictionaryHeader = $"$Dictionary";

    private static void SerializeValue(ref JsonWriter writer, object obj)
    {
        Type type;
        CachedProps cached;
        int written;
        switch (obj)
        {
            case null:
                writer.Write(Null.AsSpan());
                break;
            case bool b:
                if (b)
                {
                    writer.Write(True.AsSpan());
                }
                else
                {
                    writer.Write(False.AsSpan());
                }

                break;
            case int i:
                const int maxCharForInt = 11;
                Span<char> ri = stackalloc char[maxCharForInt];
                i.TryFormat(ri, out written);
                writer.Write(ri.Slice(0, written));
                break;
            case uint ui:
                const int maxCharForUint = 10;
                Span<char> rui = stackalloc char[maxCharForUint];
                ui.TryFormat(rui, out written);
                writer.Write(rui.Slice(0, written));
                break;
            case long l:
                const int maxCharForLong = 20;
                Span<char> rl = stackalloc char[maxCharForLong];
                l.TryFormat(rl, out written);
                writer.Write(rl.Slice(0, written));
                break;
            case ulong ul:
                const int maxCharForUlong = 20;
                Span<char> rul = stackalloc char[maxCharForUlong];
                ul.TryFormat(rul, out written);
                writer.Write(rul.Slice(0, written));
                break;
            case short s:
                const int maxCharForShort = 6;
                Span<char> rs = stackalloc char[maxCharForShort];
                s.TryFormat(rs, out written);
                writer.Write(rs.Slice(0, written));
                break;
            case ushort us:
                const int maxCharForUshort = 5;
                Span<char> rus = stackalloc char[maxCharForUshort];
                us.TryFormat(rus, out written);
                writer.Write(rus.Slice(0, written));
                break;
            case sbyte b:
                const int maxCharForSignedByte = 4;
                Span<char> rsb = stackalloc char[maxCharForSignedByte];
                b.TryFormat(rsb, out written);
                writer.Write(rsb.Slice(0, written));
                break;
            case byte b:
                const int maxCharForByte = 3;
                Span<char> rb = stackalloc char[maxCharForByte];
                b.TryFormat(rb, out written);
                writer.Write(rb.Slice(0, written));
                break;
            case float f:
                const int maxCharForFloat = 24;
                Span<char> rf = stackalloc char[maxCharForFloat];
                f.TryFormat(rf, out written);
                writer.Write(rf.Slice(0, written));
                break;
            case double d:
                const int maxCharForDouble = 25;
                Span<char> rd = stackalloc char[maxCharForDouble];
                d.TryFormat(rd, out written);
                writer.Write(rd.Slice(0, written));
                break;
            case decimal d:
                const int maxCharForDecimal = 29;
                Span<char> rdecimal = stackalloc char[maxCharForDecimal];
                d.TryFormat(rdecimal, out written);
                writer.Write(rdecimal.Slice(0, written));
                break;
            case string s:
                writer.Write('"');
                writer.Write(s);
                writer.Write('"');
                break;
            case char c:
                writer.Write('"');
                writer.Write(c);
                writer.Write('"');
                break;
            default:
                cached = GetOrCreateCacheFor(obj.GetType());
                type = obj.GetType();
                ;
                if (type.IsEnum)
                {
                    writer.Write('"');
                    writer.Write(Enum.GetName(type, obj));
                    writer.Write('"');
                    break;
                }

                if (type.IsArray)
                {
                    SerializeArray(ref writer, obj, cached);
                    break;
                }

                if (cached.CollectionType == CollectionType.Dictionary)
                {
                    SerializeDictionary(ref writer, obj, cached);
                    break;
                }

                if (cached.CollectionType == CollectionType.List)
                {
                    SerializeList(ref writer, obj, cached);
                    break;
                }

                // Object, most likely
                SerializeObject(ref writer, obj, cached);
                break;
        }
    }

    private static void SerializeDictionary(ref JsonWriter writer, object obj, CachedProps cache)
    {
        var dict = (IDictionary)obj;
        if (cache.GenericKeyType.IsPrimitive || cache.GenericKeyType == typeof(string))
        {
            SerializeCompliantDictionary(ref writer, dict, cache);
        }
        else
        {
            SerializeNonCompliantDictionary(ref writer, dict, cache);
        }

    }

    private static void SerializeCompliantDictionary(ref JsonWriter writer, IDictionary dict, CachedProps cache )
    {
        bool wroteFirstKvp = false;
        writer.Write('{');
        foreach (DictionaryEntry entry in dict)
        {
            if (wroteFirstKvp) writer.Write(',');
            wroteFirstKvp = true;
            
            if(cache.GenericKeyType == typeof(string))
            {
                SerializeValue(ref writer, entry.Key);
            }
            else
            {
                writer.Write('"');
                SerializeValue(ref writer, entry.Key);
                writer.Write('"');
            }
            writer.Write(':');
            SerializeValue(ref writer, entry.Value);
        }
        writer.Write('}');
    }

    private static void SerializeNonCompliantDictionary(ref JsonWriter writer, IDictionary dict, CachedProps props)
    {
        writer.Write('{');
        bool wroteFirstKvp = false;
        writer.Write('"');
        writer.Write(DictionaryHeader.AsSpan());
        writer.Write('"');
        writer.Write(':');
        foreach (DictionaryEntry entry in dict)
        {
            if (wroteFirstKvp) writer.Write(',');
            wroteFirstKvp = true;
            writer.Write('[');
            SerializeValue(ref writer, entry.Key);
            writer.Write(',');
            SerializeValue(ref writer, entry.Value);
            writer.Write(']');
        }
        writer.Write('}');
    }

    private static void SerializeArray(ref JsonWriter writer, object obj, CachedProps cache)
    {
        Array array = (Array)obj;
        writer.Write('[');
        bool wroteItem = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (wroteItem) writer.Write(',');
            wroteItem = true;
            SerializeValue(ref writer, array.GetValue(i));
        }
        writer.Write(']');
    }
    
    private static void SerializeList(ref JsonWriter writer, object obj, CachedProps cache)
    {
        writer.Write('[');
        var enumerable = (ICollection)obj;
        bool wroteItem = false;
        foreach (var item in enumerable)
        {
            if (wroteItem) writer.Write(',');
            wroteItem = true;
            SerializeValue(ref writer, item);
        }
        writer.Write(']');
    }
    
    private static void SerializeObject(ref JsonWriter writer, object obj, CachedProps cache)
    {
        writer.Write('{');
        bool wroteSomething = false;
        foreach (var prop in cache.Properties)
        {
            if (!prop.CanRead)
            {
                continue;
            }
            var value = prop.GetValue(obj);
            
            if (wroteSomething) writer.Write(',');
            wroteSomething = true;
            
            writer.Write('"');
            writer.Write(prop.Name);
            writer.Write('"');
            
            writer.Write(':');
            
            SerializeValue(ref writer, value);
            writer.Write(',');
        }

        foreach (var field in cache.Fields)
        {
            var value = field.GetValue(obj);
            
            if (wroteSomething) writer.Write(',');
            wroteSomething = true;
            
            writer.Write('"');
            writer.Write(field.Name);
            writer.Write('"');
            
            writer.Write(':');
            
            SerializeValue(ref writer, value);
        }
        writer.Write('}');
    }
}