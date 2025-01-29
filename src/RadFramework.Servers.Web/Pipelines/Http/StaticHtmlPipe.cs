using System.Reflection;
using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Net.Http.Pipelined;

namespace RadFramework.Servers.Web.Pipelines.Http;

public class StaticHtmlPipe : IHttpPipe
{
    private string WWWRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/wwwroot";
    
    private string[] htmlExtensions = new[]
    {
        "htm",
        "html"
    };
    
    public void Process(HttpConnection connection, ExtensionPipeContext pipeContext)
    {
        string urlPath = connection.Request.UrlPath;
        
        if (connection.Request.UrlPath == "/" || connection.Request.UrlPath == "")
        {
            connection.Response.TryServeStaticHtmlFile(WWWRootPath + "/index.html");
            pipeContext.Return();
            return;
        }
        
        if (urlPath.EndsWith('/'))
        {
            urlPath = urlPath.TrimEnd('/');
        }

        string[] segments = urlPath.Split('/');

        string fileName = segments.Last();

        if (fileName.Contains('.'))
        {
            string fileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1);

            if (htmlExtensions.Contains(fileExtension))
            {
                connection.Response.TryServeStaticHtmlFile(WWWRootPath + connection.Request.UrlPath);
                pipeContext.Return();
                return;             
            }
        }
    }
}