using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching.Queries
{
    public static class ClassQueries
    {
        public static IEnumerable<ConstructorInfo> GetPublicConstructors(Type type) =>
            type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        public static ConstructorInfo GetPublicEmptyConstructor(Type type) =>
            type.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public,
                null,
                CallingConventions.Standard,
                new Type[0],
                null);

        public static IEnumerable<FieldInfo> GetPublicFields(Type type) =>
            type
                .GetFields(BindingFlags.Instance | BindingFlags.Public);

        public static IEnumerable<EventInfo> GetPublicImplementedEvents(Type type) =>
            type
                .GetEvents(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);

        public static IEnumerable<Type> GetPublicImplementedInterfaces(Type type) =>
            type.GetInterfaces();

        public static IEnumerable<MethodInfo> GetPublicImplementedMethods(Type type) =>
            type
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                .Where(method => !method.IsSpecialName);

        public static IEnumerable<PropertyInfo> GetPublicImplementedProperties(Type type) =>
            type
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

        public static IEnumerable<MethodInfo> GetPublicStaticMethods(Type type) =>
            type
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(method => !method.IsSpecialName)
                .ToList();

        public static IEnumerable<Type> GetTypeArguments(Type type)
        {
            if (type.IsConstructedGenericType || type.IsGenericTypeDefinition)
            {
                return type.GetGenericArguments();
            }

            return new Type[0];
        }

        public static Type GetGenericTypeDefinition(Type type)
        {
           if (type.IsGenericTypeDefinition)
           {
               return type;
           }

           if (type.IsGenericType)
           {
               return type.GetGenericTypeDefinition();
           }

           return null;
        }

        public static ConstructorInfo GetDefaultConstructor(Type type) =>
            type.GetConstructor(
               BindingFlags.Instance | BindingFlags.Public,
               null,
               CallingConventions.Standard,
               new Type[0],
               null);

        public static Type[] GetGenericArguments(Type type)
        {
            return type.GetGenericArguments();
        }
    }
}