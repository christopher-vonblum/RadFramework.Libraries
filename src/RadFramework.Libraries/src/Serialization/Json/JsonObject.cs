using RadFramework.Libraries.Serialization.Json.Parser;

namespace RadFramework.Libraries.Serialization.Json
{
    public class JsonObject : IJsonObjectTreeModel
    {
        public IEnumerable<JsonProperty> Properties
        {
            get
            {
                return _properties.Select(p => new JsonProperty
                {
                    Name = p.Key,
                    _Value = _properties[p.Key]
                });
            }
        }
        
        private IDictionary<string, object> _properties;

        public object this[string property]
        {
            get
            {
                return _properties[property];
            }
            set
            {
                _properties[property] = value;
            }
        }

        public JsonObject(IDictionary<string, object> properties)
        {
            _properties = properties;
        }
        
        public JsonObject(string json)
        {
            _properties = ParseObject(json);
        }
        
        public static Dictionary<string, object> ParseObject(string json)
        {
            int nesting = 0;

            Dictionary<string, object> objectProperties = new Dictionary<string, object>();
            
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

        private static Tuple<string, object> ParseJsonProperty(JsonParserCursor cursor)
        {
            string propertyName = ParsePropertyName(cursor);

            // skip everything until we reach the key value seperator :
            ParserUtils.ReadUntilChars(cursor, new[] { ':' });
            
            // skip : char
            cursor.Index++;
            
            // skip whitespaces
            cursor.SkipWhitespacesAndNewlines();
            
            var type = ParserUtils.DetermineType(cursor.CurrentChar);

            object value;
            
            if (type == JsonTypes.String)
            {
                // skip the leading " of the string
                cursor.Index++;
                
                // skip everything until we reach the end of the string ignore escaped "
                string str = ParserUtils.ReadUntilChars(cursor, new[] {'"'});
                
                value = str;
            }
            else if(type == JsonTypes.Object)
            {
                string json = cursor.CurrentJson;
                value = new JsonObject(json);
                cursor.SkipObjectOrArray();
            }
            else if(type == JsonTypes.Array)
            {
                string json = cursor.CurrentJson;
                value = new JsonArray(json);
                cursor.SkipObjectOrArray();
            }
            else
            {
                throw new NotImplementedException();
            }
            
            return new Tuple<string, object>(propertyName, value);
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