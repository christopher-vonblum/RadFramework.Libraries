using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace JsonParser
{
    public class JsonObject
    {
        public IEnumerable<JsonProperty> Properties
        {
            get
            {
                return _properties.Value.Select(p => new JsonProperty
                {
                    Name = p.Key,
                    _Value = _properties.Value[p.Key]
                });
            }
        }
        
        private Lazy<IDictionary<string, Lazy<object>>> _properties;

        public object this[string property]
        {
            get
            {
                return _properties.Value[property].Value;
            }
            set
            {
                _properties.Value[property].Value = value;
            }
        }

        public JsonObject()
        {
        }
        
        public JsonObject(string json)
        {
            _properties = new Lazy<IDictionary<string, Lazy<object>>>(() => ParseObject(json));
        }
        
        public static Dictionary<string, Lazy<object>> ParseObject(string json)
        {
            int nesting = 0;

            Dictionary<string, Lazy<object>> objectProperties = new Dictionary<string, Lazy<object>>();
            
            JsonParserCursor cursor = new JsonParserCursor(json, 0);

            do
            {
                cursor.SkipWhitespacesAndNewlines();
                
                if (nesting == 1 && (cursor.CurrentChar.Equals('"') 
                                     || char.IsLetterOrDigit(cursor.CurrentChar)))
                {
                    var prop = ParseJsonProperty(cursor);
                    objectProperties.Add(prop.Item1, prop.Item2);
                }
                else if (cursor.CurrentChar.Equals('"'))
                {
                    cursor.SkipString();
                }

                if (cursor.CurrentChar.Equals('{'))
                {
                    nesting++;
                }
                else if (cursor.CurrentChar.Equals('}'))
                {
                    nesting--;
                }
                
                cursor.Index++;
                
            } while (nesting != 0);

            return objectProperties;
        }

        private static Tuple<string, Lazy<object>> ParseJsonProperty(JsonParserCursor cursor)
        {
            string propertyName = ParsePropertyName(cursor);

            // skip everything until we reach the key value seperator :
            ParserUtils.ReadUntilChars(cursor, new[] { ':' });
            
            // skip : char
            cursor.Index++;
            
            // skip whitespaces
            cursor.SkipWhitespacesAndNewlines();
            
            var type = ParserUtils.DetermineType(cursor.CurrentChar);

            Lazy<object> value;
            
            if (type == JsonTypes.String)
            {
                // skip the leading " of the string
                cursor.Index++;
                
                // skip everything until we reach the end of the string ignore escaped "
                string str = ParserUtils.ReadUntilChars(cursor, new[] {'"'});
                
                value = new Lazy<object>(str);
            }
            else if(type == JsonTypes.Object)
            {
                string json = cursor.CurrentJson;
                value = new Lazy<object>(() => new JsonObject(json));
                cursor.SkipObjectOrArray();
            }
            else if(type == JsonTypes.Array)
            {
                string json = cursor.CurrentJson;
                value = new Lazy<object>(() => new JsonArray(json));
                cursor.SkipObjectOrArray();
            }
            else
            {
                throw new NotImplementedException();
            }
            
            return new Tuple<string, Lazy<object>>(propertyName, value);
        }

        private static string ParsePropertyName(JsonParserCursor cursor)
        {
            if (cursor.CurrentChar.Equals('"'))
            {
                return cursor.ReadString();
            }
            
            return ParserUtils.ReadUntilChars(cursor, new [] { ' ',':' });
        }
    }
}