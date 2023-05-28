namespace RadFramework.Libraries.Telemetry;

public interface ISequentialDataTransferComponent : IDisposable
{
    void Send(byte[] data);
    byte[] Receive();
}