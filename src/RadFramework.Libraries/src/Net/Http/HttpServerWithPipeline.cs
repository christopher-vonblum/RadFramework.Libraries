using RadFramework.Abstractions.Extensibility.Pipeline;

namespace RadFramework.Libraries.Net.Http;

public class HttpServerWithPipeline : IDisposable
{
    private HttpServer server;
    
    public HttpServerWithPipeline(int port, IPipeline<HttpConnection, byte[]> httpPipeline)
    {
        server = new HttpServer(port, connection =>
        {
            byte[] response = httpPipeline.Process(connection);
            
            connection.ResponseStream.Write(response);
            
            connection.ResponseStream.Flush();
            connection.ResponseStream.Dispose();
            
            connection.UnderlyingSocket.Close();
            connection.UnderlyingSocket.Dispose();
        });
    }

    public void Dispose()
    {
        server.Dispose();
    }
}