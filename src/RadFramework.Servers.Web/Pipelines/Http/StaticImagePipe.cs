using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Utils;

namespace RadFramework.Servers.Web.Pipelines.Http;

public class StaticImagePipe : IHttpPipe
{
    private string WWWRootPath = "wwwroot";
    
    private string[] imgExtensions = new[]
    {
        "png",
        "bmp",
        "jpeg",
        "jpg"
    };
    
    public void Process(HttpConnection context, ExtensionPipeContext pipeContext)
    {
        string fileExtension = context.Request.UrlPath.Substring(context.Request.UrlPath.LastIndexOf('.') + 1);

        if (imgExtensions.Contains(fileExtension))
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