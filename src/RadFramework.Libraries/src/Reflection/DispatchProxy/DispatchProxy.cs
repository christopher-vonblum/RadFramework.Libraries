using System;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.DispatchProxy
{
    public static class DispatchProxy
    {
        private static MethodInfo createMethod;
        
        static DispatchProxy()
        {
            createMethod = typeof(System.Reflection.DispatchProxy).GetMethod("Create", BindingFlags.Public | BindingFlags.Static);
        }

        public static object Create(Type t, Type tProxy)
        {
            return createMethod.MakeGenericMethod(t, tProxy).Invoke(null, null);
        }
    }
}