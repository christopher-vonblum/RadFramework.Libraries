using System.Net;
using System.Net.Sockets;

namespace RadFramework.Libraries.Net.Http;

public class SocketConnectionListener : IDisposable
{
    private readonly Action<Socket> onSocketAccepted;
    private Socket listenerSocket;

    private Thread acceptThread;
    
    private bool disposed = false;
    
    public SocketConnectionListener(SocketType socketType, ProtocolType protocolType, int port, Action<Socket> onSocketAccepted)
    {
        this.onSocketAccepted = onSocketAccepted;
        IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
        
        listenerSocket = new Socket(IPAddress.Any.AddressFamily, socketType, protocolType);
        
        listenerSocket.Bind(endPoint);
        listenerSocket.Listen();
        acceptThread = new Thread(AcceptSockets);
        acceptThread.Priority = ThreadPriority.Highest;
        acceptThread.Start();
    }

    private void AcceptSockets()
    {
        while (!disposed)
        {
            try
            {
                onSocketAccepted(listenerSocket.Accept());
            }
            catch
            {
            }
        }
    }
    
    public void Dispose()
    {
        disposed = true;
        listenerSocket.Dispose();
    }
}