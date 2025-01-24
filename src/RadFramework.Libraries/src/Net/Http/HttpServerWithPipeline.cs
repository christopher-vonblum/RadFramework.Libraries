using RadFramework.Libraries.Extensibility.Pipeline;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerWithPipeline : IDisposable
{
    private readonly IPipeline<HttpConnection, byte[]> httpPipeline;
    private HttpServer server;
    
    public HttpServerWithPipeline(int port, IPipeline<HttpConnection, byte[]> httpPipeline)
    {
        this.httpPipeline = httpPipeline;
        server = new HttpServer(port, ProcessRequestUsingPipeline);
    }
    
    private void ProcessRequestUsingPipeline(HttpConnection connection)
    {
        byte[] response = httpPipeline.Process(connection);

        connection.ResponseStream.Write(response);

        connection.ResponseStream.Flush();
        connection.ResponseStream.Dispose();

        connection.UnderlyingSocket.Close();
        connection.UnderlyingSocket.Dispose();
    }
    
    public void Dispose()
    {
        server.Dispose();
    }
}