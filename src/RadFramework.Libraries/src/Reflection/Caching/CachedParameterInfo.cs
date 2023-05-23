using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedParameterInfo : CachedMetadataBase<ParameterInfo>
    {
        public static implicit operator ParameterInfo(CachedParameterInfo cachedPropertyInfo)
            => cachedPropertyInfo.InnerMetaData;
    
        public static implicit operator CachedParameterInfo(ParameterInfo parameterInfo)
            => ReflectionCache.CurrentCache.GetCachedMetaData(parameterInfo);
    }
}