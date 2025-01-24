using System.Collections;

namespace RadFramework.Libraries.Serialization.Json.Proxy;

public class JsonArrayProxy<TEntry> : IEnumerable<TEntry>, IJsonArrayProxyInternal
{
    private IEnumerable<TEntry> DataAsEnumerableOfEntry
    {
        get
        {
            return Data.OfType<TEntry>();
        }
    }

    public IEnumerator<TEntry> GetEnumerator()
    {
        return DataAsEnumerableOfEntry.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return DataAsEnumerableOfEntry.GetEnumerator();
    }

    public JsonArray Data { get; set; }
}