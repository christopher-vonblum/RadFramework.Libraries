using RadFramework.Libraries.Extensibility.Pipeline.Extension;

namespace RadFramework.Libraries.Net.Http.Pipelined;

public interface IHttpErrorPipe : IExtensionPipe<(HttpConnection connection, Exception e)>
{
}