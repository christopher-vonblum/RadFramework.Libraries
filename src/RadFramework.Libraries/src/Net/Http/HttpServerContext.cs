using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Logging;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerContext
{
    private readonly ISimpleCache cache;
    private readonly ILogger logger;

    public HttpServerContext(ISimpleCache cache, ILogger logger)
    {
        this.cache = cache;
        this.logger = logger;
    }

    public ISimpleCache Cache => cache;
    public ILogger Logger => logger;
    public string NotFoundPage { get; set; } = "wwwroot/404.html";
}