namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedType : CachedMetadataBase<Type>
    {
        public static implicit operator Type(CachedType cachedPropertyInfo)
            => cachedPropertyInfo.InnerMetaData;
    
        public static implicit operator CachedType(Type type)
            => ReflectionCache.CurrentCache.GetCachedMetaData(type);
        
        public static implicit operator CachedType(string assemblyQualifiedName)
            => ReflectionCache.CurrentCache.GetCachedMetaData(Type.GetType(assemblyQualifiedName));
    }
}