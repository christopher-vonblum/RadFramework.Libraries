using System;
using System.Collections;
using System.Reflection;

namespace JsonParser
{
    public class ObjectProxy : System.Reflection.DispatchProxy
    {
        internal JsonObject _o;
        
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            string methodName = targetMethod.Name;

            if (methodName.StartsWith("get_") )
            {
                object value = _o[methodName.Substring(4)];

                if (value is JsonObject o)
                {
                    ObjectProxy proxy = (ObjectProxy)DispatchProxy.Create(targetMethod.ReturnType, typeof(ObjectProxy));
                    proxy._o = o;
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
                
                _o[key] = @value;

                return null;
            }

            throw new NotImplementedException();
        }
    }
}