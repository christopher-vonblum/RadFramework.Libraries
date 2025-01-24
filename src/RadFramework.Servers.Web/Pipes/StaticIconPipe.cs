using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Net.Http;

namespace RadFramework.Servers.Web.Pipes;

public class StaticIconPipe : SynchronousPipeBase<HttpConnection, byte[]>
{
    public override byte[] Process(HttpConnection input)
    {
        throw new NotImplementedException();
    }
}