namespace MultiMap.Misc.JsonSerializer;

public readonly struct JsonToken(TokenType type, int line, int position, int startIndex = 0, int length = 0)
{
    public readonly TokenType Type = type;
    public readonly int StartIndex = startIndex;
    public readonly int Length = length;
    public readonly int Line = line;
    public readonly int Position = position;
}