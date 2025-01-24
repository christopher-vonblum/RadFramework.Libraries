using System.Collections;
using System.Runtime.Serialization;
using System.Text;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;
using RadFramework.Libraries.Serialization.Json.Parser;
using RadFramework.Libraries.Serialization.Json.Writer;

namespace RadFramework.Libraries.Serialization.Json.ContractSerialization;

public class JsonContractSerializer : IContractSerializer
{
    public static JsonContractSerializer Instance { get; } = new JsonContractSerializer();
    
    private readonly JsonObjectTreeSerializer objectTreeSerializer;
    
    public byte[] Serialize(Type type, object model)
    {
        return Encoding.UTF8.GetBytes(SerializeToJsonString(type, model));
    }
    
    public string SerializeToJsonString(Type type, object model)
    {
        IJsonObjectTreeModel jsonModel = (IJsonObjectTreeModel)CreateJsonObjectForSerialization(type, model);
        return objectTreeSerializer.Serialize(jsonModel);
    }

    public object CreateJsonObjectForSerialization(Type type, object obj)
    {
        if (type == typeof(string))
        {
            return obj;
        }
        
        if (type.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
        {
            return CreateJsonArrayFromEnumerable((IEnumerable)obj);
        }

        return CreateJsonObjectFromRuntimeObject(type, obj);
    }

    private object CreateJsonObjectFromRuntimeObject(Type objectType, object obj)
    {
        CachedType t = objectType ?? obj.GetType();

        var properties = t.Query(TypeQueries.GetProperties);
        
        Dictionary<string, object> propertiesDictionary = new Dictionary<string, object>();

        foreach (var property in properties)
        {
            propertiesDictionary.Add(property.Name, CreateJsonObjectForSerialization(property.PropertyType, property.GetValue(obj)));
        }
        
        return new JsonObject(propertiesDictionary);
    }

    private object CreateJsonArrayFromEnumerable(IEnumerable enumerable)
    {
        List<object> arrayData = new List<object>();
        foreach (object o in enumerable)
        {
            arrayData.Add(CreateJsonObjectForSerialization(o.GetType(), o));
        }
        return new JsonArray(arrayData);
    }

    public object DeserializeFromJsonString(Type t, string jsonString)
    {
        object jsonObject = DeserializeToJsonObject(jsonString);

        if (t.IsArray || typeof(IEnumerable).IsAssignableFrom(t))
        {
            Type enumerableInterface = t.GetInterface(typeof(IEnumerable<>).Name);
            
            Type jsonArrayProxyType = typeof(JsonArrayProxy<>).MakeGenericType(enumerableInterface.GetGenericArguments()[0]);
            
            IJsonArrayProxyInternal jsonArrayProxy = Reflection.Activation.Activator.Activate(jsonArrayProxyType) as IJsonArrayProxyInternal;

            jsonArrayProxy.Data = (JsonArray)jsonObject;
            
            return jsonArrayProxy;
            
        }
        else if (jsonObject is JsonObject o)
        {
            JsonObjectProxy stronglyTypedProxy = (JsonObjectProxy)Reflection.DispatchProxy.DispatchProxy.Create(t, typeof(JsonObjectProxy));
            
            stronglyTypedProxy.Data = o;
            
            return stronglyTypedProxy;
        }
        else if (t.IsPrimitive)
        {
            return Convert.ChangeType(jsonObject, t);
        }
        else
        {
            throw new SerializationException();
        }
    }
    
    public object Deserialize(Type type, byte[] data)
    {
        string json = Encoding.UTF8.GetString(data);
        
        return DeserializeFromJsonString(type, json);
    }

    public object Clone(Type type, object model)
    {
        return Deserialize(
            type,
            Serialize(type, model));
    }

    public object DeserializeToJsonObject(string json)
    {
        json = json.TrimStart();
        
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

        throw new SerializationException();
    }
}