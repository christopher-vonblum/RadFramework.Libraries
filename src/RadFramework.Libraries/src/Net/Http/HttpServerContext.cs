using RadFramework.Libraries.Caching;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerContext
{
    private readonly ISimpleCache commonCache;

    public HttpServerContext(ISimpleCache commonCache)
    {
        this.commonCache = commonCache;
    }

    public ISimpleCache CommonCache => commonCache;
}