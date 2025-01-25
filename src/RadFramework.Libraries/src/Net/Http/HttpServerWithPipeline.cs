using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Extension;
using RadFramework.Libraries.Ioc;
using RadFramework.Libraries.Threading.Internals;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerWithPipeline : IDisposable
{
    private readonly ExtensionPipeline<HttpConnection> httpPipeline;
    private HttpServer server;
    private HttpServerContext ServerContext;

    public HttpServerWithPipeline(
        int port,
        PipelineDefinition httpPipelineDefinition,
        Action<System.Net.Sockets.Socket, PoolThread, Exception> onException,
        IocContainer iocContainer,
        Action<System.Net.Sockets.Socket> webSocketConnected = null)
    {
        ServerContext = iocContainer.Resolve<HttpServerContext>();
        this.httpPipeline = new ExtensionPipeline<HttpConnection>(httpPipelineDefinition, iocContainer);
        server = new HttpServer(port, ProcessRequestUsingPipeline, onException, webSocketConnected);
    }

    private void ProcessRequestUsingPipeline(HttpConnection connection)
    {
        connection.ServerContext = ServerContext;
        
        if (!httpPipeline.Process(connection))
        {
            connection
                .Response
                .Send404(
                connection
                    .Response
                    .GetFileFromCacheOrDisk(
                        connection.ServerContext.NotFoundPage));
        }
    }
    
    public void Dispose()
    {
        server.Dispose();
    }
}