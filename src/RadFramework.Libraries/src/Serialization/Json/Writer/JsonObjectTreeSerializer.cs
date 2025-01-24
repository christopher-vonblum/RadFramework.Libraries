using System.Runtime.Serialization;
using RadFramework.Libraries.Serialization.Json.ContractSerialization;

namespace RadFramework.Libraries.Serialization.Json.Writer;

public class JsonObjectTreeSerializer
{
    private JsonWriter writer = new JsonWriter();
    
    public string Serialize(object obj)
    {
        if (obj is string s)
        { 
            return s;
        }
        
        if (obj is JsonObject o)
        {
            return writer.WriteObject(o);
        }
        
        if (obj is JsonArray a)
        {
            return writer.WriteArray(a);
        }
        
        if (obj is JsonProperty p)
        {
            return writer.WriteProperty(p);
        }
        
        if (obj is JsonObjectProxy proxy)
        {
            return writer.WriteObject(proxy.Data);
        }
        
        if (obj is IJsonArrayProxyInternal proxyArray)
        {
            return writer.WriteArray(proxyArray.Data);
        }
        
        throw new SerializationException($"Unable to deserialize object of type {obj.GetType().FullName}");
    }




}