using System.Net.Sockets;

namespace RadFramework.Libraries.Net.Http;

public class HttpServer : IDisposable
{
    private readonly HttpRequestHandler processRequest;
    private SocketConnectionListener listener;
    
    public HttpServer(int port, HttpRequestHandler processRequest)
    {
        this.processRequest = processRequest;
        
        listener = new SocketConnectionListener(
            SocketType.Stream,
            ProtocolType.Tcp,
            port,
            ProcessSocketConnection);
        
    }

    protected void ProcessSocketConnection(Socket socketConnection)
    {
        NetworkStream networkStream = new NetworkStream(socketConnection);
        
        StreamReader requestReader = new StreamReader(networkStream);
        
        string firstRequestLine = requestReader.ReadLine();
        
        HttpRequest requestModel = new HttpRequest();
        
        requestModel.Method = HttpRequestParser.ExtractHttpMethod(firstRequestLine);
        requestModel.Url = HttpRequestParser.ExtractUrl(firstRequestLine);
        requestModel.HttpVersion = HttpRequestParser.ExtractHttpVersion(firstRequestLine);

        string currentHeaderLine = null;
        
        while ((currentHeaderLine = requestReader.ReadLine()) != "")
        {
            var header = HttpRequestParser.ReadHeader(currentHeaderLine);
            requestModel.Headers.Add(header.header, header.value);
        }

        processRequest(new HttpConnection
        {
            Request = requestModel,
            RequestReader = requestReader,
            ResponseStream = networkStream,
            UnderlyingSocket = socketConnection
        });
    }

    public void Dispose()
    {
        listener.Dispose();
    }
}