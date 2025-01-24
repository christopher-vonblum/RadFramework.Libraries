using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public interface IReflectionCache
    {
        CachedAssembly GetCachedMetaData(Assembly metaData);
        CachedType GetCachedMetaData(Type metaData);
        CachedMethodInfo GetCachedMetaData(MethodInfo metaData);
        CachedConstructorInfo GetCachedMetaData(ConstructorInfo metaData);
        CachedParameterInfo GetCachedMetaData(ParameterInfo metaData);
        CachedPropertyInfo GetCachedMetaData(PropertyInfo metaData);
        CachedEventInfo GetCachedMetaData(EventInfo metaData);
        CachedFieldInfo GetCachedMetaData(FieldInfo metaData);
    }
}