using System;

namespace MultiMap.Misc.JsonSerializer;

public static class TokenParser
{
    public static void ExtractTokensFromStringUnsafe(ReadOnlySpan<char> json, ref JsonParser parser)
    {
        int startIndex;
        int length;
        int i = 0;
        int line = 0;
        int totalPrevious = 0;
        while (i < json.Length)
        {
            char c = json[i];

            switch (c)
            {
                case '/':
                    if (json[i + 1] == '/')
                    {
                        i += 2;
                    }

                    break;
                case '{':
                    parser.Tokens.Add(new JsonToken(TokenType.StartObject, line, i - totalPrevious));
                    i++;
                    break;
                case '}':
                    parser.Tokens.Add(new JsonToken(TokenType.EndObject, line, i - totalPrevious));
                    i++;
                    break;
                case '[':
                    parser.Tokens.Add(new JsonToken(TokenType.StartArray, line, i - totalPrevious));
                    i++;
                    break;
                case ']':
                    parser.Tokens.Add(new JsonToken(TokenType.EndArray, line, i - totalPrevious));
                    i++;
                    break;
                case ':':
                    parser.Tokens.Add(new JsonToken(TokenType.Colon, line, i - totalPrevious));
                    i++;
                    break;
                case ',':
                    parser.Tokens.Add(new JsonToken(TokenType.Comma, line, i - totalPrevious));
                    i++;
                    break;
                case '"':
                    if (json[i + 1] == '$')
                    {
                        // Skip " and $
                        i += 2;
                        if (json[i..].StartsWith("Type"))
                        {
                            // Accounts for coma
                            i += 5;
                            ParseString(json, ref i, ref parser, out startIndex, out length);
                            parser.Tokens.Add(new JsonToken(TokenType.Type, line, i - totalPrevious, startIndex, length));
                            // Account for closing "
                            i++;
                            break;
                        }
                        else if (json[i..].StartsWith("Id"))
                        {
                            // Accounts for coma
                            i += 3;
                            ParseString(json, ref i, ref parser, out startIndex, out length);
                            parser.Tokens.Add(new JsonToken(TokenType.Id, line, i - totalPrevious, startIndex, length));
                            // Account for closing "
                            i++;
                            break;
                        }
                        else if (json[i..].StartsWith("Dictionary"))
                        {
                            i += 10;
                            parser.Tokens.Add(new JsonToken(TokenType.NonCompliantDictionary, line, i - totalPrevious));
                            // Account for closing "
                            i++;
                            break;
                        }
                        // didn't find a special tag, revert index
                        i -= 2;
                    }
                    ParseString(json, ref i, ref parser, out var start, out length);
                    parser.Tokens.Add(new JsonToken(TokenType.String, line, i - totalPrevious, start, length));
                    break;
                default:
                    if (c == '\n')
                    {
                        i++;
                        line++;
                        totalPrevious = i;
                        continue;
                    }

                    if (char.IsWhiteSpace(c))
                    {
                        i++;
                        continue;
                    }

                    if (char.IsDigit(c) || c == '-')
                    {
                        int startNum = i;
                        while (i < json.Length &&
                               (char.IsDigit(json[i]) || json[i] == '.' || json[i] == 'e' || json[i] == 'E' ||
                                json[i] == '+' || json[i] == '-'))
                            i++;
                        int len = i - startNum;
                        startIndex = parser.Chars.Count;
                        parser.WriteString(json.Slice(startNum, len));
                        parser.Tokens.Add(new JsonToken(TokenType.Number, line, i - totalPrevious, startIndex, len));
                        continue;
                    }

                    if (json[i..].StartsWith("true"))
                    {
                        i += 4;
                        parser.Tokens.Add(new JsonToken(TokenType.True, line, i - totalPrevious));
                        continue;
                    }

                    if (json[i..].StartsWith("false"))
                    {
                        i += 5;
                        parser.Tokens.Add(new JsonToken(TokenType.False, line, i - totalPrevious));
                        continue;
                    }

                    if (json[i..].StartsWith("null"))
                    {
                        i += 4;
                        parser.Tokens.Add(new JsonToken(TokenType.Null, line, i - totalPrevious));
                        continue;
                    }

                    throw new Exception(
                        $"Error deserializing character {json[i]} at line {line} at position {i - totalPrevious}");
            }
        }
    }

    private static void ParseString(ReadOnlySpan<char> json, ref int i, ref JsonParser parser, out int startIndex, out int length)
    {
        int start = ++i;
        while (i < json.Length && json[i] != '"') i++;

        length = i - start;
        startIndex = parser.Chars.Count;
        parser.WriteString(json.Slice(start, length));
        i++;
    }
}