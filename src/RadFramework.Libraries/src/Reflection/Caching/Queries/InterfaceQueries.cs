using System;
using System.Linq;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching.Queries
{
    public static class InterfaceQueries
    {
        public static PropertyInfo[] GetProperties(Type type) => 
            GetBaseInterfaces(type)
                .SelectMany(GetProperties)
                .Concat(type.GetProperties())
                .ToArray();
        public static EventInfo[] GetEvents(Type type) => 
            GetBaseInterfaces(type)
                .SelectMany(GetEvents)
                .Concat(type.GetEvents())
                .ToArray();

        public static MethodInfo[] GetMethods(Type type) =>
            GetBaseInterfaces(type)
                .SelectMany(GetMethods)
                .Concat(type.GetMethods().Where(m => !m.IsSpecialName))
                .ToArray();

        public static Type[] GetBaseInterfaces (Type type) => type.GetInterfaces();
    }
}