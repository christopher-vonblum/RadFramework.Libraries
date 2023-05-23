using System;
using System.Linq;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching.Queries
{
    public class TypeQueries
    {
        public static PropertyInfo[] GetProperties(Type type) => (type.IsInterface ? InterfaceQueries.GetProperties(type) : ClassQueries.GetPublicImplementedProperties(type)).ToArray();
        public static EventInfo[] GetEvents(Type type) => (type.IsInterface ? InterfaceQueries.GetEvents(type) : ClassQueries.GetPublicImplementedEvents(type)).ToArray();
        public static MethodInfo[] GetMethods(Type type) => (type.IsInterface ? InterfaceQueries.GetMethods(type) : ClassQueries.GetPublicImplementedMethods(type)).ToArray();
    }
}