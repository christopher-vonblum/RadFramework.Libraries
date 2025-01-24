using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;
using RadFramework.Libraries.Threading.Internals;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerWithPipeline : IDisposable
{
    private readonly ExtensionPipeline<HttpConnection> httpPipeline;
    private HttpServer server;
    private HttpServerContext ServerContext;

    public HttpServerWithPipeline(int port, PipelineDefinition httpPipelineDefinition, Action<System.Net.Sockets.Socket, PoolThread, Exception> onException, IocContainer iocContainer)
    {
        ServerContext = iocContainer.Resolve<HttpServerContext>();
        this.httpPipeline = new ExtensionPipeline<HttpConnection>(httpPipelineDefinition, iocContainer);
        server = new HttpServer(port, ProcessRequestUsingPipeline, onException);
    }

    private void ProcessRequestUsingPipeline(HttpConnection connection)
    {
        connection.ServerContext = ServerContext;
        httpPipeline.Process(connection);
    }
    
    public void Dispose()
    {
        server.Dispose();
    }
}