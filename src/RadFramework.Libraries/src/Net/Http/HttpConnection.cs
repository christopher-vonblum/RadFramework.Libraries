using System.Net.Sockets;

namespace RadFramework.Libraries.Net.Http;

public struct HttpConnection
{
    public HttpRequest Request;
    public StreamReader RequestReader;
    public NetworkStream ResponseStream;
    public System.Net.Sockets.Socket UnderlyingSocket;
}