using System;
using System.Text;

namespace JsonParser
{
    public static class ParserUtils
    {
        public static JsonTypes DetermineType(char c)
        {
            switch (c)
            {
                case '{':
                    return JsonTypes.Object;
                case '[':
                    return JsonTypes.Array;
                case '"':
                    return JsonTypes.String;
                case '/':
                    return JsonTypes.Comment;
                default:
                {
                    if (char.IsDigit(c))
                    {
                        return JsonTypes.NumericValues;
                    }

                    break;
                }
            }

            throw new NotImplementedException();
        }

        public static string ReadUntilChars(JsonParserCursor cursor, char[] stopChars)
        {
            StringBuilder builder = new StringBuilder();
            
            while (!CharArrayContains(cursor.CurrentChar, stopChars))
            {
                builder.Append(cursor.CurrentChar);
                cursor.Index++;
            }
            
            return builder.ToString();
        }

        private static bool CharArrayContains(char aChar, char[] chars)
        {
            for (int j = 0; j < chars.Length; j++)
            {
                if (aChar.Equals(chars[j]))
                {
                    return true;
                }
            }

            return false;
        }
    }
}