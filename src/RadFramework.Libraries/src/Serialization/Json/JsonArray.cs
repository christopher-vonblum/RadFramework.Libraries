using System;
using System.Collections;
using System.Collections.Generic;

namespace JsonParser
{
    public class JsonArray : IEnumerable<object>
    {
        private Lazy<List<object>> entries;

        public JsonArray(string jsonArray)
        {
            entries = new Lazy<List<object>>(() => ParseArray(jsonArray));
        }

        public object this[int index]
        {
            get
            {
                return entries.Value[index];
            }
            set
            {
                entries.Value[index] = value;
            }
        }

        public IEnumerator<object> GetEnumerator()
        {
            return entries.Value.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) entries.Value).GetEnumerator();
        }
        
        public static List<object> ParseArray(string jsonArray)
        {
            List<object> entries = new List<object>();

            int nesting = 0;
            
            JsonParserCursor cursor = new JsonParserCursor(jsonArray, 0);
            
            do
            {
                cursor.SkipWhitespacesAndNewlines();

                if (nesting == 1 && cursor.CurrentChar == '"')
                {
                    int startIndex = cursor.Index;
                    string currentJson = cursor.CurrentJson;

                    cursor.SkipString();

                    entries.Add(currentJson.Substring(1, cursor.Index - startIndex - 2));
                }
                else if (nesting == 1 && cursor.CurrentChar == '[')
                {
                    entries.Add(new JsonArray(cursor.CurrentJson));

                    cursor.SkipWhitespacesAndNewlines();
                    
                    cursor.SkipObjectOrArray();
                }
                else if (nesting == 1 && cursor.CurrentChar == '{')
                {
                    entries.Add(new JsonObject(cursor.CurrentJson));
                    
                    cursor.SkipObjectOrArray();
                }
                
                if (cursor.CurrentChar.Equals('[') || cursor.CurrentChar.Equals('{'))
                {
                    nesting++;
                }
                else if (cursor.CurrentChar.Equals(']') || cursor.CurrentChar.Equals('}'))
                {
                    nesting--;
                }

                cursor.Index++;
                
            } while (nesting != 0);

            return entries;
        }
    }
}