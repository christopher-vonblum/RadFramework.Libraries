using System.Net.Sockets;

namespace RadFramework.Libraries.Net.Http;

public class HttpConnection : IDisposable
{
    public HttpServerContext ServerContext { get; set; }
    private HttpResponse response;
    
    public HttpRequest Request;
    public HttpResponse Response => response ??= new(this);

    public StreamReader RequestReader;
    public NetworkStream UnderlyingStream;
    public System.Net.Sockets.Socket UnderlyingSocket;

    public void Dispose()
    {
        RequestReader?.Dispose();
        UnderlyingStream?.Dispose();
        UnderlyingSocket?.Dispose();
    }
}