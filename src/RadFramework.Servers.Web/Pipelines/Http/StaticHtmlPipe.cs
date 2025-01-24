using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Net.Http;
using RadFramework.Servers.Web.Pipelines;

namespace RadFramework.Servers.Web.Pipes;

public class StaticHtmlPipe : IHttpPipe
{
    private readonly ISimpleCache cache;
    private string WWWRootPath = "wwwroot";

    public StaticHtmlPipe(ISimpleCache cache)
    {
        this.cache = cache;
    }
    
    public void Process(HttpConnection connection)
    {
        if (connection.Request.UrlPath == "/")
        {
            connection.Response.TryServeStaticHtmlFile(WWWRootPath + "/index.html");
        }
        
        connection.Response.TryServeStaticHtmlFile(WWWRootPath + connection.Request.UrlPath);
    }
}