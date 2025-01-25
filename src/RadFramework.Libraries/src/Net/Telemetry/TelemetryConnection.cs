using RadFramework.Libraries.Reflection.Caching;
using ZeroFormatter.Internal;

namespace RadFramework.Libraries.Net.Socket;

public class TelemetryConnection : ITelemetryConnection
{
    public TelemetrySocketManager SocketManager { get; set; }
    public SocketBond SocketBond { get; set; }
    
    public object ReceivePackage()
    {
        byte[] headerSizeBuffer = new byte[sizeof(uint)];
        
        SocketBond.ReceiveSocket.Receive(headerSizeBuffer);
        
        uint headerSize = BinaryUtil.ReadUInt32(ref headerSizeBuffer, 0);

        byte[] serializedHeader = new byte[headerSize];

        SocketBond.ReceiveSocket.Receive(serializedHeader);

        PackageHeader header = (PackageHeader)SocketManager.HeaderSerializer.Deserialize(typeof(PackageHeader), serializedHeader);
        
        byte[] serializedPackage = new byte[header.PayloadSize];

        SocketBond.ReceiveSocket.Receive(serializedPackage);
        
        return SocketManager.HeaderSerializer.Deserialize(Type.GetType(header.PayloadType), serializedPackage);
    }

    public void SendPackage(CachedType packageType, object package, byte[] responseToken = null)
    {
        byte[] serializedPackage = SocketManager.HeaderSerializer.Serialize(packageType, package);

        PackageHeader header = new PackageHeader();

        header.PayloadType = packageType.InnerMetaData.AssemblyQualifiedName;
        
        // evaluate on per client base which request matches which response
        // means no global registry for req/res
        header.ResponseToken = responseToken;

        byte[] serializedHeader = SocketManager.HeaderSerializer.Serialize(typeof(PackageHeader), header);
        
        //serializedHeader.Length
        
        byte[] serializedOverallSize = new byte[sizeof(ulong)];
        
        BinaryUtil.WriteInt32(ref serializedOverallSize, 0, serializedHeader.Length);

        this.SocketBond.SendSocket.Send(serializedOverallSize);
        this.SocketBond.SendSocket.Send(serializedHeader);
        this.SocketBond.SendSocket.Send(serializedPackage);
    }
}