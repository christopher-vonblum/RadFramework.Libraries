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
    private readonly ExtensionPipeline<(HttpConnection connection, Exception e)> httpErrorPipeline;

    public HttpServerWithPipeline(
        int port,
        PipelineDefinition httpPipelineDefinition,
        PipelineDefinition httpErrorPipelineDefinition,
        IocContainer iocContainer,
        Action<System.Net.Sockets.Socket> webSocketConnected = null)
    {
        iocContainer.RegisterSingleton<HttpServerContext>();
        ServerContext = iocContainer.Resolve<HttpServerContext>();
        this.httpPipeline = new ExtensionPipeline<HttpConnection>(httpPipelineDefinition, iocContainer);
        this.httpErrorPipeline = new ExtensionPipeline<(HttpConnection connection, Exception e)>(httpErrorPipelineDefinition, iocContainer);
        server = new HttpServer(port, ProcessRequestUsingPipeline, null, webSocketConnected);
    }

    private void ProcessRequestUsingPipeline(HttpConnection connection)
    {
        connection.ServerContext = ServerContext;

        try
        {
            if (!httpPipeline.Process(connection))
            {
                httpErrorPipeline.Process((connection, null));
            }
        }
        catch (Exception e)
        {
            try
            {
                httpErrorPipeline.Process((connection, e));
            }
            catch (Exception ee)
            {
                ServerContext.Logger.LogError("HttpPipeline crashed while processing request. When the error should have been dealt with an exception occured too. Review your pipeline implementations.");
            }
        }
    }
    
    public void Dispose()
    {
        server.Dispose();
    }
}