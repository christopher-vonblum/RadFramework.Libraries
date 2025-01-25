using System.Collections.Concurrent;
using RadFramework.Libraries.Utils;

namespace RadFramework.Libraries.Net.Socket;

public class TelemetrySocketManager
{
    private ConcurrentDictionary<byte[], System.Net.Sockets.Socket[]> clientToSocketsMapping = new();
    
    public TelemetrySocketManager()
    {
    }

    public void RegisterNewClientSocket(System.Net.Sockets.Socket socket)
    {
        byte[] clientToken = ByteUtil.GenerateRandomToken(1024);

        if (clientToSocketsMapping.TryAdd(clientToken, new[] { socket }));
    }
    
    
    
    public void WritePackage(Stream stream, MyPackage package)
    {
        byte[] serializedPackage = MessagePackSerializer.Serialize(package);

        PackageHeader header = new PackageHeader();

        header.PayloadType = typeof(MyPackage).FullName;
        header.PayloadSize = serializedPackage.Length;
        header.ResponseToken = Guid.NewGuid();

        byte[] serializedHeader = MessagePackSerializer.Serialize(header);

        MessagePackSerializer.Typeless.
        
        byte[] serializedHeaderSize = new byte[sizeof(int)];

        BinaryUtil.WriteInt32(ref serializedHeaderSize, 0, serializedHeader.Length);

        stream.Write(serializedHeaderSize);
        stream.Write(serializedHeader);
        stream.Write(serializedPackage);
        stream.Flush();
    }
    
    private MyPackage ReadPackage(Stream stream)
    {
        stream.Position = 0;
        
        byte[] headerSizeBuffer = new byte[sizeof(int)];
        
        stream.Read(headerSizeBuffer, 0, sizeof(int));
        
        int headerSize = BinaryUtil.ReadInt32(ref headerSizeBuffer, 0);

        byte[] serializedHeader = new byte[headerSize];

        stream.Read(serializedHeader, 0, headerSize);

        PackageHeader header = MessagePackSerializer.Deserialize<PackageHeader>(serializedHeader);

        byte[] serializedPackage = new byte[header.PayloadSize];

        stream.Read(serializedPackage, 0, header.PayloadSize);
        
        return MessagePackSerializer.Deserialize<MyPackage>(serializedPackage);
    }

    private void Read(Stream stream, byte[] buffer, int numBytes)
    {
        int bytesRead = 0;
        do
        {
            int n = stream.Read(buffer, bytesRead, numBytes - bytesRead);
            if (n == 0)
            {
                throw new Exception("Package malformed or stream ended.");
            }

            bytesRead += n;
        } while (bytesRead < numBytes);
    }
}