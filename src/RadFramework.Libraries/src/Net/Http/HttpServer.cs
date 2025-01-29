using System.Net.Sockets;
using RadFramework.Libraries.Net.Socket;
using RadFramework.Libraries.Serialization.Json.ContractSerialization;
using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Net.Http;

public class HttpServer : IDisposable
{
    private readonly HttpRequestHandler processRequest;
    private readonly Action<System.Net.Sockets.Socket> webSocketConnected;
    private SocketConnectionListener listener;
    private QueuedThreadPool<System.Net.Sockets.Socket> httpRequestProcessingPool;
    
    public HttpServer(
        int port,
        HttpRequestHandler processRequest, 
        Action<System.Net.Sockets.Socket, PoolThread, Exception> onException, 
        Action<System.Net.Sockets.Socket> webSocketConnected = null)
    {
        this.processRequest = processRequest;
        this.webSocketConnected = webSocketConnected;

        httpRequestProcessingPool = 
            new QueuedThreadPool<System.Net.Sockets.Socket>(
                2,
                ThreadPriority.Highest,
                ProcessHttpSocketConnection,
                onException,
                "RadFramework.Libraries.Net.Http.HttpServer-processing-pool");
        
        listener = new SocketConnectionListener(
            SocketType.Stream,
            ProtocolType.Tcp,
            port,
            OnSocketAccepted);
    }

    private void OnSocketAccepted(System.Net.Sockets.Socket connectionSocket)
    {
        httpRequestProcessingPool.Enqueue(connectionSocket);
    }

    private void ProcessHttpSocketConnection(System.Net.Sockets.Socket socketConnection)
    {
        NetworkStream networkStream = new NetworkStream(socketConnection);
        
        StreamReader requestReader = new StreamReader(networkStream);
        
        string firstRequestLine = requestReader.ReadLine();
        
        HttpRequest requestModel = new HttpRequest();
        
        requestModel.Method = HttpRequestParser.ExtractHttpMethod(firstRequestLine);
        requestModel.Url = HttpRequestParser.ExtractUrl(firstRequestLine);
        requestModel.UrlPath = HttpRequestParser.ExtractUrl(firstRequestLine);
        requestModel.QueryString = HttpRequestParser.ExtractQueryString(requestModel.Url);
        requestModel.HttpVersion = HttpRequestParser.ExtractHttpVersion(firstRequestLine);

        string currentHeaderLine = null;
        
        while ((currentHeaderLine = requestReader.ReadLine()) != "")
        {
            var header = HttpRequestParser.ReadHeader(currentHeaderLine);
            requestModel.Headers.Add(header.header, header.value);
        }

        if (webSocketConnected != null && ((requestModel.Headers.ContainsKey("Upgrade") && requestModel.Headers["Upgrade"] == "websocket") 
                                        && (requestModel.Headers.ContainsKey("Connection") && requestModel.Headers["Connection"] == "Upgrade")))
        {
            networkStream.Dispose();
            requestReader.Dispose();

            webSocketConnected(socketConnection);
            
            return;
        }
            
        HttpConnection connection =
            new HttpConnection
            {
                Request = requestModel,
                RequestReader = requestReader,
                UnderlyingStream = networkStream,
                UnderlyingSocket = socketConnection
            };

        try
        {
            processRequest(connection);
        }
        catch (Exception e)
        {
            connection.ServerContext.Logger.LogError("Error while processing request: \r\n" + JsonContractSerializer.Instance.SerializeToJsonString(typeof(HttpRequest), requestModel) + "\n" + e.ToString());
        }
        
        networkStream.Flush();
        
        connection.Response.Dispose();
        
        requestReader.Dispose();
        networkStream.Dispose();
            
        socketConnection.Close();
        socketConnection.Dispose();
    }

    public void Dispose()
    {
        listener.Dispose();
        httpRequestProcessingPool.Dispose();
    }
}