using System;
using System.Collections.Generic;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.DispatchProxy
{
    public class InterfaceProxy : System.Reflection.DispatchProxy
    {
        Dictionary<string, object> properties = new Dictionary<string, object>();
        
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (!targetMethod.IsSpecialName)
            {
                return null;
            }

            string propertyName = targetMethod.Name.Substring(4);
            
            if (targetMethod.Name.StartsWith("set_"))
            {
                properties[propertyName] = args[0];
            }
            else if (targetMethod.Name.StartsWith("get_"))
            {
                if (!properties.ContainsKey(propertyName))
                {
                    if (targetMethod.ReturnType.IsValueType)
                    {
                        return Activator.CreateInstance(targetMethod.ReturnType);
                    }
                    
                    return null;
                }
                
                return properties[propertyName];
            }

            return null;
        }
    }
}