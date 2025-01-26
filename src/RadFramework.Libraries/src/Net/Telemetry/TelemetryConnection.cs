using RadFramework.Libraries.Net.Socket;
using RadFramework.Libraries.Reflection.Caching;
using RadFramework.Libraries.Serialization;
using ZeroFormatter.Internal;

namespace RadFramework.Libraries.Net.Telemetry;

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
        
        return ReceivePackage(header);
    }

    private object ReceivePackage(PackageHeader header)
    {
        byte[] serializedPackage = new byte[header.PayloadSize];

        SocketBond.ReceiveSocket.Receive(serializedPackage);

        IContractSerializer serializer = 
            header.PayloadSerializerType != null ?
            (IContractSerializer)SocketManager
            .SimpleCache
            .GetOrSet(
                header.PayloadSerializerType,
                () => Type
                    .GetType(header.PayloadSerializerType)
                    .GetConstructor(null)
                    .Invoke(null)) 
            : SocketManager.HeaderSerializer;
        
        return serializer
            .Deserialize(header.PayloadType, serializedPackage);
    }

    public void SendPackage(CachedType packageType, object package, byte[] responseToken = null)
    {
        byte[] serializedPackage = SocketManager.HeaderSerializer.Serialize(packageType, package);

        PackageHeader header = new PackageHeader();

        header.PayloadType = packageType.InnerMetaData.AssemblyQualifiedName;
        
        // evaluate on per client base which request matches which response
        // means no global registry for req/res
        header.ResponseToken = responseToken;
        header.

        byte[] serializedHeader = SocketManager.HeaderSerializer.Serialize(typeof(PackageHeader), header);
        
        //serializedHeader.Length
        
        byte[] serializedOverallSize = new byte[sizeof(ulong)];
        
        BinaryUtil.WriteInt32(ref serializedOverallSize, 0, serializedHeader.Length);

        this.SocketBond.SendSocket.Send(serializedOverallSize);
        this.SocketBond.SendSocket.Send(serializedHeader);
        this.SocketBond.SendSocket.Send(serializedPackage);
    }
}