using System.Net.Sockets;
using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Net.Http;

public class HttpServer : IDisposable
{
    private readonly HttpRequestHandler processRequest;
    private SocketConnectionListener listener;
    private QueuedThreadPool<Socket> httpRequestProcessingPool;
    
    public HttpServer(int port, HttpRequestHandler processRequest)
    {
        this.processRequest = processRequest;
        
        httpRequestProcessingPool = 
            new QueuedThreadPool<Socket>(
                Environment.ProcessorCount,
                ThreadPriority.Highest,
                ProcessHttpSocketConnection);
        
        listener = new SocketConnectionListener(
            SocketType.Stream,
            ProtocolType.Tcp,
            port,
            OnSocketAccepted);
    }

    private void OnSocketAccepted(Socket connectionSocket)
    {
        httpRequestProcessingPool.Enqueue(connectionSocket);
    }

    private void ProcessHttpSocketConnection(Socket socketConnection)
    {
        NetworkStream networkStream = new NetworkStream(socketConnection);
        
        StreamReader requestReader = new StreamReader(networkStream);
        
        string firstRequestLine = requestReader.ReadLine();
        
        HttpRequest requestModel = new HttpRequest();
        
        requestModel.Method = HttpRequestParser.ExtractHttpMethod(firstRequestLine);
        requestModel.Url = HttpRequestParser.ExtractUrl(firstRequestLine);
        requestModel.QueryString = HttpRequestParser.ExtractQueryString(requestModel.Url);
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