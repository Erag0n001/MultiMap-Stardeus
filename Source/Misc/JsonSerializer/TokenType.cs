namespace MultiMap.Misc.JsonSerializer;

public enum TokenType
{
    StartObject, EndObject,
    StartArray, EndArray,
    String, Number,
    True, False, Null, Colon, Comma, Type, Id, NonCompliantDictionary
}