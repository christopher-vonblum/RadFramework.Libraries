namespace RadFramework.Libraries.Telemetry.Abstractions;

public interface ISequentialDataTransferComponent : IDisposable
{
    void Send(byte[] data);
    byte[] Receive();
}