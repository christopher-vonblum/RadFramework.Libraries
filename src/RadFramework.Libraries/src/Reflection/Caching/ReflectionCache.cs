using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class ReflectionCache : IReflectionCache
    {
        public static ReflectionCache CurrentCache
        {
            get
            {
                if (contextualCache == null)
                {
                    return DefaultInstance;
                }

                return contextualCache.Peek();
            }
        }

        public static IDisposable UseCache(ReflectionCache cache)
        {
            if (contextualCache == null)
            {
                contextualCache = new Stack<ReflectionCache>();
            }
            
            contextualCache.Push(cache);
            return new CachingContext(() => contextualCache.Pop());
        }

        private class CachingContext : IDisposable
        {
            private readonly Action pop;

            public CachingContext(Action pop)
            {
                this.pop = pop;
            }

            public void Dispose()
            {
                pop();
            }
        }
        
        private static ReflectionCache DefaultInstance = new ReflectionCache();

        [ThreadStatic]
        private static Stack<ReflectionCache> contextualCache;
        
        private ConcurrentDictionary<object, string> keyCache = new ConcurrentDictionary<object, string>();
        private ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
        
        public string BuildTypeKey(Type t)
        {
            return keyCache.GetOrAdd(t, o =>
            {
                string key = t.Assembly.FullName + t.MetadataToken.ToString();

                if (!t.IsConstructedGenericType)
                {
                    return key;
                }

                var typeArguments = t.GetGenericArguments();
                
                foreach (var typeArgument in typeArguments)
                {
                    key += BuildTypeKey(typeArgument);
                }

                return key;
            });
        }
        
        public string BuildMethodKey(MethodBase method)
        {
            return keyCache.GetOrAdd(method, o =>
            {
                string key = BuildTypeKey(method.DeclaringType);

                key += method.MetadataToken.ToString();
                
                if(method is MethodInfo m && m.ReturnType.IsGenericParameter)
                    key += m.ReturnType.MetadataToken.ToString();

                if (!method.IsConstructedGenericMethod)
                {
                    return key;
                }

                var typeArguments = method.GetGenericArguments();
                foreach (var typeArgument in typeArguments)
                {
                    key += BuildTypeKey(typeArgument);
                }

                return key;
            });
        }
        private TCached GetCachedMetaData<TCached, TMetaData>(string key, TMetaData metaData) where TCached : CachedMetadataBase<TMetaData>, new() where TMetaData : MemberInfo
        {
            return (TCached)cache.GetOrAdd(key, s => new TCached() { InnerMetaData = metaData });
        }
        
        public CachedAssembly GetCachedMetaData(Assembly metaData)
        {
            return (CachedAssembly)cache.GetOrAdd(metaData.FullName, s => new CachedAssembly() { InnerMetaData = metaData });
        }
        
        public CachedType GetCachedMetaData(Type metaData)
        {
            return GetCachedMetaData<CachedType, Type>(BuildTypeKey(metaData), metaData);
        }
        
        public CachedMethodInfo GetCachedMetaData(MethodInfo metaData)
        {
            return GetCachedMetaData<CachedMethodInfo, MethodInfo>(BuildMethodKey(metaData), metaData);
        }
        
        public CachedConstructorInfo GetCachedMetaData(ConstructorInfo metaData)
        {
            return GetCachedMetaData<CachedConstructorInfo, ConstructorInfo>(BuildMethodKey(metaData), metaData);
        }
        
        public CachedParameterInfo GetCachedMetaData(ParameterInfo metaData)
        {
            return (CachedParameterInfo)cache.GetOrAdd(BuildMethodKey((MethodBase)metaData.Member) + metaData.MetadataToken, s => new CachedParameterInfo() { InnerMetaData = metaData });
        }
        
        public CachedPropertyInfo GetCachedMetaData(PropertyInfo metaData)
        {
            return GetCachedMetaData<CachedPropertyInfo, PropertyInfo>(BuildTypeKey(metaData.DeclaringType) + metaData.MetadataToken, metaData);
        }
        
        public CachedEventInfo GetCachedMetaData(EventInfo metaData)
        {
            return GetCachedMetaData<CachedEventInfo, EventInfo>(BuildTypeKey(metaData.DeclaringType) + metaData.MetadataToken, metaData);
        }

        public CachedFieldInfo GetCachedMetaData(FieldInfo metaData)
        {
            return GetCachedMetaData<CachedFieldInfo, FieldInfo>(BuildTypeKey(metaData.DeclaringType) + metaData.MetadataToken, metaData);
        }
    }
}