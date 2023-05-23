using System.Reflection;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedConstructorInfo : CachedMetadataBase<ConstructorInfo>
    {
        public static implicit operator ConstructorInfo(CachedConstructorInfo cachedConstructorInfo)
            => cachedConstructorInfo.InnerMetaData;
        public static implicit operator CachedConstructorInfo(ConstructorInfo constructorInfo)
            => ReflectionCache.CurrentCache.GetCachedMetaData(constructorInfo);

    }
}