using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedPropertyInfo : CachedMetadataBase<PropertyInfo>
    {
        public static implicit operator PropertyInfo(CachedPropertyInfo cachedPropertyInfo)
            => cachedPropertyInfo.InnerMetaData;
    
        public static implicit operator CachedPropertyInfo(PropertyInfo propertyInfo)
            => ReflectionCache.CurrentCache.GetCachedMetaData(propertyInfo);
    }
}