using System.Text;
using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Net.Http;

namespace RadFramework.Servers.Web.Pipes;

public class StaticHtmlPipe : SynchronousPipeBase<HttpConnection, byte[]>
{
    private readonly ISimpleCache cache;
    private string WWWRootPath = "wwwroot";

    public StaticHtmlPipe(ISimpleCache cache)
    {
        this.cache = cache;
    }
    
    public override byte[] Process(HttpConnection connection)
    {
        if (connection.Request.UrlPath == "/")
        {
            return StaticHtmlFileBytes(WWWRootPath + "/index.html");
        }
        
        return StaticHtmlFileBytes(WWWRootPath + connection.Request.UrlPath);
    }

    byte[] StaticHtmlFileBytes(string path)
    {
        try
        {
            
        }
        byte[] fileContents = Encoding.UTF8.GetBytes( cache.GetOrSet(path, () => File.ReadAllText(path)));
    }
}