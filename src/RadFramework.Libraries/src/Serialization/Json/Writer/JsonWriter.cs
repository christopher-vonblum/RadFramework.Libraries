namespace RadFramework.Libraries.Serialization.Json.Writer;

public class JsonWriter
{
    public string WriteObject(JsonObject toSerialize)
    {
        string json = "";
        WriteObject(ref json, toSerialize);
        return json;
    }
    
    public string WriteObject(ref string json, JsonObject toSerialize)
    {
        json += "{ ";
        
        foreach (JsonProperty property in toSerialize.Properties)
        {
            WriteProperty(ref json, property);
        }
        
        json += " }";
        
        return json;
    }

    public string WriteProperty(JsonProperty property)
    {
        string json = "";
        WriteProperty(ref json, property);
        return json;
    }
    
    public void WriteProperty(ref string json, JsonProperty property)
    {
        json += $"\"{property.Name}\" : ";
        
        if (property.Value is string str)
        {
             json += $"\"{str ?? string.Empty}\"";
        }
        else if (property.Value is JsonArray a)
        {
            json += WriteArray(ref json, a);
        }
        else if (property.Value is JsonObject o)
        {
            json += WriteObject(ref json, o);
        }
        
        json += ", ";
    }

    public string WriteArray(JsonArray toSerialize)
    {
        string json = "";
        WriteArray(ref json, toSerialize);
        return json;
    }

    public string WriteArray(ref string json, JsonArray toSerialize)
    {
        json += "[ ";
        
        foreach (object arrayEntry in toSerialize)
        {
            if (arrayEntry is string str)
            {
                json += $"\"{str}\"";
                json += ", ";
            }
            else if (arrayEntry is JsonArray a)
            {
                json += WriteArray(ref json, a);
                json += ", ";
            }
            else if (arrayEntry is JsonObject o)
            {
                json += WriteObject(ref json, o);
                json += ", ";
            }
        }
        
        json += " ]";
        
        return json;
    }
}