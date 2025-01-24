using System.Collections;
using System.Reflection;

namespace RadFramework.Libraries.Serialization.Json.ContractSerialization
{
    public class JsonObjectProxy : System.Reflection.DispatchProxy
    {
        internal JsonObject Data;
        
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            string methodName = targetMethod.Name;

            if (methodName.StartsWith("get_") )
            {
                object value = Data[methodName.Substring(4)];

                if (value is JsonObject o)
                {
                    JsonObjectProxy proxy = (JsonObjectProxy)DispatchProxy.Create(targetMethod.ReturnType, typeof(JsonObjectProxy));
                    proxy.Data = o;
                    return proxy;
                }
                else if(value is JsonArray a)
                {
                    if (typeof(IEnumerable).IsAssignableFrom(targetMethod.ReturnType))
                    {
                        return a;
                    }
                }

                return value;
            }
            else if (methodName.StartsWith("set_"))
            {
                string key = methodName.Substring(4);
                object @value = args[0];
                
                Data[key] = @value;

                return null;
            }

            throw new NotImplementedException();
        }
    }
}