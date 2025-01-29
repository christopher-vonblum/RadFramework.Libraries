using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Net.Http.Pipelined;

namespace RadFramework.Servers.Web.Pipelines.Http;

public class FaviconPipe : IHttpPipe
{
    private string WWWRootPath = "wwwroot";
    
    public void Process(HttpConnection context, ExtensionPipeContext pipeContext)
    {
        string fileExtension = context.Request.UrlPath.Substring(context.Request.UrlPath.LastIndexOf('.') + 1);

        if (fileExtension == "ico")
        {
            context.Response.SendFile(
                context
                    .ServerContext
                    .Cache
                    .GetOrSet(
                        context.Request.UrlPath, 
                        () => File.ReadAllBytes(WWWRootPath + context.Request.UrlPath)));
            
            pipeContext.Return();
            return;
        }
    }
}