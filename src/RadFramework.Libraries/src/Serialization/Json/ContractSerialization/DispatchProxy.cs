using System.Collections.Concurrent;
using System.Reflection;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Serialization.Json.ContractSerialization
{
    public class DispatchProxy
    {
        private static ConcurrentDictionary<CachedType, CachedMethodInfo> createProxyMethodsCache =
            new ConcurrentDictionary<CachedType, CachedMethodInfo>();

        private static MethodInfo createMethod =
            typeof(System.Reflection.DispatchProxy).GetMethod("Create", BindingFlags.Static | BindingFlags.Public);

        public static object Create(CachedType t, CachedType proxyType)
        {
            CachedMethodInfo proxyMethod = createProxyMethodsCache.GetOrAdd(t, type => type.Query(t => createMethod.MakeGenericMethod(t, proxyType)));

            return proxyMethod.InnerMetaData.Invoke(null, new object[]{ t, proxyMethod });
        }
    }
}