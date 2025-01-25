using System.Collections;
using System.Reflection;
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
    
    public byte[] Serialize(CachedType type, object model)
    {
        return Encoding.UTF8.GetBytes(SerializeToJsonString(type, model));
    }
    
    public string SerializeToJsonString(CachedType type, object model)
    {
        IJsonObjectTreeModel jsonModel = (IJsonObjectTreeModel)CreateJsonObjectForSerialization(type, model);
        
        return new JsonObjectTreeSerializer().Serialize(jsonModel);
    }

    public object CreateJsonObjectForSerialization(CachedType type, object obj)
    {
        if (type == typeof(string))
        {
            return obj;
        }
        
        if (type.InnerMetaData.IsArray || typeof(IEnumerable).IsAssignableFrom(type))
        {
            return CreateJsonArrayFromEnumerable((IEnumerable)obj);
        }

        if (type.InnerMetaData.IsConstructedGenericType && type.InnerMetaData.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            Type[] args = type.InnerMetaData.GetGenericArguments();

            return CreateJsonObjectFromDictionaryObject(args[0], args[1], obj);
        }

        return CreateJsonObjectFromRuntimeObject(type, obj);
    }

    private object CreateJsonObjectFromDictionaryObject(CachedType tKey, Type tValue, object dictionary)
    {
        IEnumerable dict = ((IEnumerable)dictionary);

        Type kvType = typeof(KeyValuePair<,>).MakeGenericType(tKey, tValue);

        PropertyInfo keyInfo = kvType.GetProperty("Key");
        PropertyInfo valueInfo = kvType.GetProperty("Value");

        Dictionary<string, object> properties = new Dictionary<string, object>();
        
        foreach (object kv in dict)
        {
            properties.
                Add(
                    keyInfo
                        .GetValue(kv)
                        .ToString(), 
                    CreateJsonObjectForSerialization(
                        valueInfo.PropertyType, 
                        valueInfo.GetValue(kv)));
        }
        
        return new JsonObject(properties);
    }
    
    private object CreateJsonObjectFromRuntimeObject(CachedType objectType, object obj)
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

    public object DeserializeFromJsonString(CachedType t, string jsonString)
    {
        object jsonObject = DeserializeToJsonObject(jsonString);

        if (t.InnerMetaData.IsArray || typeof(IEnumerable).IsAssignableFrom(t))
        {
            Type enumerableInterface = t.InnerMetaData.GetInterface(typeof(IEnumerable<>).Name);
            
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
        else if (t.InnerMetaData.IsPrimitive)
        {
            return Convert.ChangeType(jsonObject, t);
        }
        else
        {
            throw new SerializationException();
        }
    }
    
    public object Deserialize(CachedType type, byte[] data)
    {
        string json = Encoding.UTF8.GetString(data);
        
        return DeserializeFromJsonString(type, json);
    }

    public object Clone(CachedType type, object model)
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