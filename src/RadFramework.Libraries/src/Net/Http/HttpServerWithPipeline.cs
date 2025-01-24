using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerWithPipeline : IDisposable
{
    private readonly ExtensionPipeline<HttpConnection> httpPipeline;
    private HttpServer server;

    public HttpServerWithPipeline(int port, PipelineDefinition httpPipelineDefinition, IocContainer iocContainer)
    {
        this.httpPipeline = new ExtensionPipeline<HttpConnection>(httpPipelineDefinition, iocContainer);
        server = new HttpServer(port, ProcessRequestUsingPipeline);
    }

    private void ProcessRequestUsingPipeline(HttpConnection connection)
    {
        httpPipeline.Process(connection);
    }
    
    public void Dispose()
    {
        server.Dispose();
    }
}