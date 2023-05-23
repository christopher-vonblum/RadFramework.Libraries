using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedMethodInfo : CachedMetadataBase<MethodInfo>
    {
        public static implicit operator MethodInfo(CachedMethodInfo methodInfo)
            => methodInfo.InnerMetaData;
        
        public static implicit operator CachedMethodInfo(MethodInfo methodInfo)
            => ReflectionCache.CurrentCache.GetCachedMetaData(methodInfo);
    }
}