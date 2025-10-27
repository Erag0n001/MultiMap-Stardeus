using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MultiMap.Misc.JsonSerializer;

public static partial class JsonSerializer
{
    private static readonly BindingFlags Flags = BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Public |
                                                 BindingFlags.NonPublic |
                                                 BindingFlags.FlattenHierarchy;

    private enum CollectionType
    {
        None,
        List,
        Dictionary
    }
    
    private class CachedProps(CollectionType props = CollectionType.None, Type collectionType = null, Type valueType = null, Type keyType = null)
    {
        public readonly List<FieldInfo> Fields = new List<FieldInfo>();
        public readonly List<PropertyInfo> Properties = new List<PropertyInfo>();
        public readonly CollectionType CollectionType = props;
        public readonly Type GenericValueType = valueType;
        public readonly Type GenericKeyType = keyType;
        public readonly Type GenericCollectionType = collectionType;
    }
    private static readonly Dictionary<Type, CachedProps> CachedTypes = new();
    private static readonly ConcurrentBag<JsonParser> Parsers = new();
    private static readonly ConcurrentBag<JsonWriter> Writers = new();
    private static JsonParser GetParser()
    {
        if (Parsers.TryTake(out var parser))
        {
            return parser;
        }
        parser = new JsonParser();
        return parser;
    }

    private static JsonWriter GetWriter()
    {
        if (Writers.TryTake(out var parser))
        {
            return parser;
        }
        parser = new JsonWriter();
        return parser;
    }

    private static void ReturnWriter(JsonWriter writer)
    {
        Writers.Add(writer);
    }
    
    private static void ReturnParser(JsonParser parser)
    {
        Parsers.Add(parser);
    }
    
    private static CachedProps GetOrCreateCacheFor(Type type)
    {
        if (CachedTypes.TryGetValue(type, out var cached))
        {
            return cached;
        }

        lock (CachedTypes)
        {
            if (CachedTypes.TryGetValue(type, out cached))
                return cached;

            CollectionType collectionType = CollectionType.None;
            Type valueType = null;
            Type keyType = null;
            Type cType = null;
            if (type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
            {
                collectionType = CollectionType.Dictionary;
                valueType = type.GetGenericArguments()[1];
                keyType = type.GetGenericArguments()[0];
                cType = typeof(Dictionary<,>).MakeGenericType(keyType, valueType);
            }
            else if (!type.IsArray && type.GetInterfaces().Any(i => i.IsGenericType &&
                                                   (i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                                   i.GetGenericTypeDefinition() == typeof(IList<>))))
            {
                collectionType = CollectionType.List;
                valueType = type.GetGenericArguments()[0];
                cType = typeof(List<>).MakeGenericType(valueType);
            }
            
            cached = new CachedProps(collectionType, cType, valueType, keyType);

            foreach (var prop in type.GetProperties(Flags))
            {
                cached.Properties.Add(prop);
            }

            foreach (var field in type.GetFields(Flags))
            {
                cached.Fields.Add(field);
            }

            CachedTypes[type] = cached;
            return cached;
        }
    }

    private static bool GetFieldFor(Type type, ReadOnlySpan<char> name , out FieldInfo field)
    {
        var cached = GetOrCreateCacheFor(type);
        foreach (var prop in cached.Fields)
        {
            var propName = prop.Name.AsSpan();
            if(propName.Length != name.Length)
                continue;
            if (propName.SequenceEqual(name))
            {
                field = prop;
                return true;
            }
        }
        field = null;
        return false;
    }

    private static bool GetPropertyFor(Type type, ReadOnlySpan<char> name, out PropertyInfo property)
    {
        var cached = GetOrCreateCacheFor(type);
        foreach (var prop in cached.Properties)
        {
            var propName = prop.Name.AsSpan();
            if(propName.Length != name.Length)
                continue;
            if (propName.SequenceEqual(name))
            {
                property = prop;
                return true;
            }
        }
        property = null;
        return false;
    }

    private static bool IsList(Type type, out CachedProps props)
    {
        props = GetOrCreateCacheFor(type);
        return props.CollectionType == CollectionType.List;
    }

    private static bool IsDictionary(Type type, out CachedProps props)
    { 
        props = GetOrCreateCacheFor(type);
        return props.CollectionType == CollectionType.Dictionary;
    }
}