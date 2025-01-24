using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;

namespace RadFramework.Servers.Web.Pipelines.Http;

public class StaticHtmlPipe : IHttpPipe
{
    private readonly ISimpleCache cache;
    private string WWWRootPath = "wwwroot";

    public StaticHtmlPipe(ISimpleCache cache)
    {
        this.cache = cache;
    }
    
    public void Process(HttpConnection connection, ExtensionPipeContext pipeContext)
    {
        if (connection.Request.UrlPath == "/")
        {
            connection.Response.TryServeStaticHtmlFile(WWWRootPath + "/index.html");
            pipeContext.Return();
            return;
        }
        
        connection.Response.TryServeStaticHtmlFile(WWWRootPath + connection.Request.UrlPath);
        pipeContext.Return();
        return;
    }
}