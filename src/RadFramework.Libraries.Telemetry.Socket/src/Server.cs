using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RadFramework.Libraries.Serialization;
using RadFramework.Libraries.Telemetry.Packages;
using RadFramework.Libraries.Threading.Tasks;

namespace RadFramework.Libraries.Telemetry.Socket;

public class Server : IDisposable
{
    private readonly System.Net.Sockets.Socket _listenerSocket;
    private readonly int _perConnectionSocketAmount;
    private readonly IContractSerializer _serializer;
    private readonly PortRangeConfiguration _portRangeConfiguration;
    private Dictionary<Guid, TelemetryConnection> channels = new Dictionary<Guid, TelemetryConnection>();
    private bool isDisposed;
    
    private Dictionary<int, System.Net.Sockets.Socket> clients = new Dictionary<int, System.Net.Sockets.Socket>()

    public Server(System.Net.Sockets.Socket listenerSocket, int perConnectionSocketAmount, IContractSerializer serializer, PortRangeConfiguration portRangeConfiguration)
    {
        _listenerSocket = listenerSocket;
        _perConnectionSocketAmount = perConnectionSocketAmount;
        _serializer = serializer;
        _portRangeConfiguration = portRangeConfiguration;
    }

    public void ListenerLoop()
    {
        _listenerSocket.Listen(10);

        while (!isDisposed)
        {
            try
            {
                System.Net.Sockets.Socket client = _listenerSocket.Accept();
                Thread establishTelemetryConnectionThread =
                    new Thread(o => EstablishTelemetryConnection((System.Net.Sockets.Socket)o));
                establishTelemetryConnectionThread.Start(client);
            }
            catch
            {
            }
        }
    }

    private void EstablishTelemetryConnection(System.Net.Sockets.Socket client)
    {
        ISequentialDataTransferComponent controlChannel = new SocketDataTransferComponent(() => client);

        byte[] handshakePackageHeader = controlChannel.Receive();

        EstablishTelemetryConnectionRequest connectionRequest = 
            (EstablishTelemetryConnectionRequest)
                _serializer.Deserialize(typeof(EstablishTelemetryConnectionRequest), handshakePackageHeader);

        EstablishTelemetryConnectionResponse response = new EstablishTelemetryConnectionResponse();
        
        response.ClientId = Guid.NewGuid();

        List<System.Net.Sockets.Socket> listeners = new List<System.Net.Sockets.Socket>();
        
        List<ThreadedTask<System.Net.Sockets.Socket>> clients = new List<ThreadedTask<System.Net.Sockets.Socket>>();
        
        for (int i = 0; i < Environment.ProcessorCount; i++)
        {
            System.Net.Sockets.Socket listener = new System.Net.Sockets.Socket(SocketType.Stream, ProtocolType.Tcp);
            
            listeners.Add(listener);
            
            listener.Bind(new IPEndPoint(IPAddress.Any, 0));
            
            clients.Add(new ThreadedTask<System.Net.Sockets.Socket>(() =>
            {
                listener.Listen(0);
                return listener.Accept();
            }));
            
            response.Ports.Add(((IPEndPoint)listener.LocalEndPoint).Port);
        }

        clients.ForEach(s => s.Start());

        controlChannel.Send(_serializer.Serialize(typeof(EstablishTelemetryConnectionResponse), response));
        
        
        
        //MultiplexTelemetryConnection connection = new MultiplexTelemetryConnection()
    }

    private void AcceptSequentialDataTransferComponent()
    {
        
    }

    public void Dispose()
    {
        isDisposed = true;
        _listenerSocket.Shutdown(SocketShutdown.Both);
    }
}

public class PortRangeConfiguration
{
    public int StartPort { get; set; }
    public int StopPort { get; set; }
}