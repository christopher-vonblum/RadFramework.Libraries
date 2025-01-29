using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Net.Http.Pipelined;
using RadFramework.Servers.Web.Pipelines.Http;

namespace RadFramework.Servers.Web.Pipelines.HttpError;

public class Send500InternalServerError : IHttpPipe
{
    public void Process(HttpConnection context, ExtensionPipeContext pipeContext)
    {
        context.Response.Send500();
    }
}