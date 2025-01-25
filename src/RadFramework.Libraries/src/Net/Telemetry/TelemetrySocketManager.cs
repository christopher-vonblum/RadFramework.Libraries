using System.Collections.Concurrent;
using RadFramework.Libraries.Net.Socket.Requests;
using RadFramework.Libraries.Serialization;
using RadFramework.Libraries.Utils;

namespace RadFramework.Libraries.Net.Socket;

public class TelemetrySocketManager
{
    public IContractSerializer HeaderSerializer { get; set; }
    private ConcurrentDictionary<byte[], SocketBond> clientToSocketsMapping = new();

    public TelemetrySocketManager(IContractSerializer headerSerializer)
    {
        this.HeaderSerializer = headerSerializer;
    }
    
    public void RegisterNewClientSocket(System.Net.Sockets.Socket socket)
    {
        ITelemetryConnection temporaryConnection = new TelemetryConnection();

        temporaryConnection.SocketManager = this;

        temporaryConnection.SocketBond = new SocketBond() { ReceiveSocket = socket, SendSocket = socket };
        
        object package = temporaryConnection.ReceivePackage();

        if (package is FirstClientToServerHello)
        {
            byte[] clientToken = ByteUtil.GenerateRandomToken(1024);
            
 
            while (!clientToSocketsMapping.TryAdd(clientToken, new() { SendSocket = socket}))
            {
                clientToken = ByteUtil.GenerateRandomToken(1024);
            }
            
            temporaryConnection.SendPackage(typeof(SecondTokenToClientPackage), new SecondTokenToClientPackage(){ Token = clientToken });;
        }
        else if (package is ThirdClientConnectsSecondSocketPackage p)
        {
            if (!clientToSocketsMapping.TryGetValue(p.Token, out var socketBond))
            {
                throw new Exception("First socket not found.");
            }

            if (socketBond.ReceiveSocket != null)
            {
                throw new Exception("Already two sockets connected.");
            }
            
            socketBond.ReceiveSocket = socket;
        }
    }

}