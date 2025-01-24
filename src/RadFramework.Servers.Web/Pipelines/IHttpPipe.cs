using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Net.Http;

namespace RadFramework.Servers.Web.Pipelines;

public interface IHttpPipe : IExtensionPipe<HttpConnection>
{
}