namespace RadFramework.Libraries.Caching
{
    public interface ISimpleCache
    {
        TObject Get<TObject>(string key);
        TObject GetOrSet<TObject>(string key, Func<TObject> factory);
        void Set<TObject>(string key, TObject o);
        ISimpleCache CreateChildCache();
        IDisposable EnterScope();
    }
}