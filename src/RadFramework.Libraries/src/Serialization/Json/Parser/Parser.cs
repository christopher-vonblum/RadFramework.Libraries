using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonParser
{
    public static partial class Parser
    {
        public static object ParseJson(string json)
        {
            var type = ParserUtils.DetermineType(json[0]);
            
            switch (type)
            {
                case JsonTypes.Array :
                    return new JsonArray(json);
                case JsonTypes.Object :
                    return new JsonObject(json);
                case JsonTypes.String :
                    return json.Trim('\"');
            }

            throw new NotImplementedException();
        }
    }
}