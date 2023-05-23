using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RadFramework.Libraries.Caching
{
    class SimpleCache : ISimpleCache
    {
        private readonly SimpleCache _parentCache;
        private ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
        
        [ThreadStatic]
        private Stack<Dictionary<string, object>> contextualCaches;
        
        public SimpleCache()
        {
        }
        
        public SimpleCache(SimpleCache parentCache)
        {
            _parentCache = parentCache;
        }
        
        public TObject Get<TObject>(string key)
        {
            if (contextualCaches != null)
            {
                foreach (var cachingLayer in contextualCaches)
                {
                    if (cachingLayer.ContainsKey(key))
                    {
                        return (TObject)cachingLayer[key];
                    }
                }
            }

            if (_parentCache != null && _parentCache.cache.ContainsKey(key))
            {
                return _parentCache.Get<TObject>(key);
            }
            
            return (TObject)cache[key];
        }

        public TObject GetOrSet<TObject>(string key, Func<TObject> factory)
        {
            TObject result = default;

            try
            {
                result = Get<TObject>(key);
            }
            catch (KeyNotFoundException)
            {
                result = factory();
                Set(key, result);
            }

            return result;
        }

        public void Set<TObject>(string key, TObject o)
        {
            if (contextualCaches != null)
            {
                contextualCaches.Peek()[key] = o;
                return;
            }

            cache[key] = o;
        }

        public ISimpleCache CreateChildCache()
        {
            return new SimpleCache(this);
        }

        public IDisposable EnterScope()
        {
            if (contextualCaches == null)
            {
                contextualCaches = new Stack<Dictionary<string, object>>();
            }
            
            contextualCaches.Push(new Dictionary<string, object>());
            return new CachingContext(() => contextualCaches.Pop());
        }

        private class CachingContext : IDisposable
        {
            private readonly Action _pop;

            public CachingContext(Action pop)
            {
                _pop = pop;
            }
            public void Dispose()
            {
                _pop();
            }
        }
    }
}