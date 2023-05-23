using System;
using System.Collections.Concurrent;

namespace RadFramework.Libraries.Reflection.Caching
{
    public class CachedMetadataBase<TMetadata>
    {
        public TMetadata InnerMetaData { get; internal set; }
    
        private ConcurrentDictionary<string, object> queryCache = new ConcurrentDictionary<string, object>();

        public TResult Query<TResult>(Func<TMetadata, TResult> query)
        {
            return (TResult) queryCache.GetOrAdd(query.Method.DeclaringType.Assembly.FullName + query.Method.MetadataToken.ToString(), (i) => query(InnerMetaData));
        }
    }
}