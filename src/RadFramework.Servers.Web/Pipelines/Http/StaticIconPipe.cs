using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;

namespace RadFramework.Servers.Web.Pipelines.Http;

public class StaticIconPipe : IHttpPipe
{
    public void Process(HttpConnection context, ExtensionPipeContext pipeContext)
    {
    }
}