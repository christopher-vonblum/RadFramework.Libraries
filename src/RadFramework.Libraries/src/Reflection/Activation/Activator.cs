using System;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Reflection.Caching.Queries;
using RadFramework.Libraries.Reflection.DispatchProxy;

namespace RadFramework.Libraries.Reflection.Activation
{
    public static class Activator
    {
        public static object Activate(Type t)
        {
            if (t.IsValueType)
            {
                return System.Activator.CreateInstance(t);
            }
            else if (t.IsInterface)
            {
                return DispatchProxy.DispatchProxy.Create(t, typeof(InterfaceProxy));
            }
            else if (t.IsClass)
            {
                CachedType type = t;

                return type.Query(ClassQueries.GetDefaultConstructor).Invoke(null);
            }

            return null;
        }
    }
}