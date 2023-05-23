using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedFieldInfo : CachedMetadataBase<FieldInfo>
    {
        public static implicit operator FieldInfo(CachedFieldInfo cachedFieldInfo)
            => cachedFieldInfo.InnerMetaData;
        
        public static implicit operator CachedFieldInfo(FieldInfo fieldInfo)
            => ReflectionCache.CurrentCache.GetCachedMetaData(fieldInfo);
    }
}