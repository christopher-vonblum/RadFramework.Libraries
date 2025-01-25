using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Net.Socket;

public interface ITelemetryConnection
{
    public TelemetrySocketManager SocketManager { get; set; }
    public SocketBond SocketBond { get; set; }

    object ReceivePackage();
    void SendPackage(CachedType type, object package, byte[] responseToken = null);

}