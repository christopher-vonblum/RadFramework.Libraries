using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedAssembly : CachedMetadataBase<Assembly>
    {
        public static implicit operator Assembly(CachedAssembly cachedAssembly) => cachedAssembly.InnerMetaData;
        public static implicit operator CachedAssembly(Assembly assembly) => ReflectionCache.CurrentCache.GetCachedMetaData(assembly);
    }
}