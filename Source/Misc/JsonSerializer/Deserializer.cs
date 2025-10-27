using System;
using System.Globalization;
using MultiMap.Misc.Native;

// ReSharper disable PossiblyImpureMethodCallOnReadonlyVariable

namespace MultiMap.Misc.JsonSerializer;

public static partial class JsonSerializer
{
    private static object DeserializeToken(ref JsonParser parser, Type type, ref int index)
    {
        var token = parser.CurrentToken();
        // Skip self
        index++;

        if (IsDebugging)
        {
            Console.WriteLine($"Deserializing token {token.Type}");
        }
        
        switch (token.Type)
        {
            case TokenType.String:
                var str = DeserializeCharArrayIntoString(parser.ReadToken(token));
                if (type == typeof(string)) return str;
                return Convert.ChangeType(str, type);
            case TokenType.Number:
            {
                var span = parser.ReadToken(token);

                // --- Integers ---
                if (type == typeof(int) && int.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i32))
                    return i32;
                if (type == typeof(uint) && uint.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var u32))
                    return u32;
                if (type == typeof(long) && long.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i64))
                    return i64;
                if (type == typeof(ulong) && ulong.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var u64))
                    return u64;
                if (type == typeof(short) && short.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i16))
                    return i16;
                if (type == typeof(ushort) && ushort.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var u16))
                    return u16;
                if (type == typeof(byte) && byte.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var u8))
                    return u8;
                if (type == typeof(sbyte) && sbyte.TryParse(span, NumberStyles.Integer, CultureInfo.InvariantCulture, out var i8))
                    return i8;

                // --- Floating-point ---
                if (type == typeof(float) && float.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var f32))
                    return f32;
                if (type == typeof(double) && double.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var f64))
                    return f64;
                if (type == typeof(decimal) && decimal.TryParse(span, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var dec))
                    return dec;

                // --- Fallback for unknown numeric type ---
                var num = DeserializeCharArrayIntoString(parser.ReadToken(token));
                return Convert.ChangeType(num, type, CultureInfo.InvariantCulture);
            }
            case TokenType.True:
                return true;
            case TokenType.False:
                return false;
            case TokenType.Null:
                return null;
            case TokenType.StartObject:
                if (IsDictionary(type, out var cachedProps))
                {
                    if (parser.CurrentToken().Type == TokenType.NonCompliantDictionary)
                    {
                        return DeserializeNonCompliantDictionary(ref parser, type, ref index, cachedProps);
                    }
                    return DeserializeDictionary(token, ref parser, type, ref index, cachedProps);
                }
                var obj = Activator.CreateInstance(type);
                while (parser.CurrentToken().Type != TokenType.EndObject)
                {
                    var key = parser.CurrentToken();
                    if (TokenType.String != key.Type)
                    {
                        throw new Exception(
                            $"Expected property name at line {key.Line} at position {key.Position}, but got {key.Type}");
                    }
                    // Skip field name
                    index++;
                    
                    var colon = parser.CurrentToken();
                    if (TokenType.Colon != colon.Type)
                    {
                        throw new Exception(
                            $"Expected colon after property at {key.Line} at position {key.Position}, but got {colon.Type}");
                    }
                    // Skip coma
                    index++;

                    var propertyName = parser.ReadToken(key);

                    if (!GetPropertyFor(type, propertyName, out var property))
                    {
                        if (!GetFieldFor(type, propertyName, out var field))
                        {
                            throw new Exception($"Failed to find a property or field at line {token.Line} at position {token.Position}");
                        }
                        var value = DeserializeToken(ref parser, field.FieldType, ref index);
                        field.SetValue(obj, value);
                    }
                    else
                    {
                        if (property.CanWrite)
                        {
                            var value = DeserializeToken(ref parser, property?.PropertyType, ref index);
                            property.SetValue(obj, value);
                            
                        }
                        else
                        {
                            // todo add warning later, skip. Throwing for this is overkill
                        }
                    }
                    
                    if (parser.Tokens[index].Type == TokenType.Comma)
                    {
                        index++;
                    }
                }
                // skip end object token
                index++;
                return obj;
            case TokenType.StartArray:
                var arrayStartIndex = index;
                var length = 0;
                var tokenLength = 0;
                while (parser.Tokens[index + tokenLength].Type != TokenType.EndArray)
                {
                    if (parser.Tokens[index + tokenLength].Type != TokenType.Comma)
                    {
                        length++;
                    }

                    tokenLength++;
                }
                
                if (type.IsArray)
                {
                    return DeserializeArray(token, ref parser, type, ref index, length);
                }
                else if (IsList(type, out var props))
                {
                    return DeserializeGenericList(token, ref parser, type, ref index, length, props);
                }
                else if (type == typeof(NativeArray<>))
                {
                }
                else if (type == typeof(NativeList<>))
                {
                    
                }
                
                throw new Exception($"Type {type.AssemblyQualifiedName ?? type.Name} was not a collection type!");
                break;
                
            default:
                throw new Exception($"Failed to deserialize collection token at line {token.Line} at position {token.Position}");
        }
    }

    private static object DeserializeNonCompliantDictionary(ref JsonParser parser, Type type, ref int index,
        CachedProps cached)
    {
        index++;
        if (parser.CurrentToken().Type != TokenType.Colon)
        {
            throw new Exception($"Expected colon at line {parser.CurrentToken().Line} at position {parser.CurrentToken().Position}");
        }
        
        if (parser.NextToken().Type != TokenType.StartArray)
        {
            throw new Exception($"Expected bracket at line {parser.CurrentToken().Line} at position {parser.CurrentToken().Position}, but got {parser.CurrentToken().Type}");
        }
        
        index++;
        
        var dict = (System.Collections.IDictionary)Activator.CreateInstance(cached.GenericCollectionType)!;
        
        while (parser.CurrentToken().Type != TokenType.EndArray)
        {
            var key = DeserializeToken(ref parser, cached.GenericKeyType, ref index);
            if (parser.CurrentToken().Type != TokenType.Comma)
            {
                throw new Exception($"Expected comma for key value pair at line {parser.CurrentToken().Line} at position {parser.CurrentToken().Position}");
            }

            index++;
            var value = DeserializeToken(ref parser, cached.GenericValueType, ref index);
            dict.Add(key, value);
        }

        // Consume End Array and End Object
        index += 2;
        return dict;
    }
    
    private static object DeserializeArray(JsonToken token, ref JsonParser parser, Type type, ref int index, 
        int length)
    {
        if (length <= 0)
        {
            throw new Exception($"Invalid array length at {token.Line} at position {token.Position}");
        }
        var elementType = type.GetElementType()!;
        var array = Array.CreateInstance(elementType, length);
        for (int i = 0; i < length; i++)
        {
            var value = DeserializeToken(ref parser, elementType, ref index);
            array.SetValue(value, i);
            if (parser.Tokens.Count > index && parser.CurrentToken().Type == TokenType.Comma)
            {
                // primitives don't consume their comas, but objects would
                index++;
            }
        }
        // Skip end array
        index++;
        return array;
    }

    private static object DeserializeGenericList(JsonToken token, ref JsonParser parser, Type type, ref int index,
        int length, CachedProps props)
    {
        if (length <= 0)
        {
            throw new Exception($"Invalid array length at {token.Line} at position {token.Position}");
        }

        var elementType = props.GenericValueType;
        var list = (System.Collections.IList)Activator.CreateInstance(props.GenericCollectionType)!;
        for (int i = 0; i < length; i++)
        {
            var value = DeserializeToken(ref parser, elementType, ref index);
            list.Add(value);
            if (parser.Tokens.Count > index && parser.CurrentToken().Type == TokenType.Comma)
            {
                // primitives don't consume their comas, but objects would
                index++;
            }
        }
        // Skip end array
        index++;
        return list;
    }
    
    private static object DeserializeDictionary(JsonToken token, ref JsonParser parser, Type type, ref int index,
        CachedProps props)
    {
        var keyType = props.GenericKeyType;
        var valueType = props.GenericValueType;
        var dict = (System.Collections.IDictionary)Activator.CreateInstance(props.GenericCollectionType)!;
        while (true)
        {
            if (parser.Tokens.Count <= index)
            {
                throw new Exception($"Failed to find end of dictionary at {token.Line} at position {token.Position}");
            }
            var next = parser.CurrentToken();
            if (next.Type == TokenType.EndObject)
            {
                index++; 
                break;
            }
            
            if (next.Type != TokenType.String)
                throw new Exception($"Expected string key in dictionary at line {next.Line}, got {next.Type}");
            
            var key = DeserializeToken(ref parser, keyType, ref index);
            
            if (parser.CurrentToken().Type != TokenType.Colon)
                throw new Exception($"Expected a colon after dictionary key at line {next.Line} at position {next.Position}, but got {parser.Tokens[index].Type}");
            index++;
            
            var value = DeserializeToken(ref parser, valueType, ref index);
            
            dict.Add(key, value);
            if (index < parser.Tokens.Count && parser.CurrentToken().Type == TokenType.Comma)
            {
                // primitives don't consume their comas, but objects would
                index++;
            }
                
        }
        // Skip end array
        return dict;
    }
    
    private static string DeserializeCharArrayIntoString(ReadOnlySpan<char> chars)
    {
        return new string(chars);
    }
}