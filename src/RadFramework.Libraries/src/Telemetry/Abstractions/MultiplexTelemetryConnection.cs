using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ObjectPools;
using RadFramework.Libraries.Threading.ObjectRegistries;
using RadFramework.Libraries.Threading.ThreadPools.DelegateShedulers.Queued;
using RadFramework.Libraries.Threading.ThreadPools.Queued;
using RadFramework.Libraries.Threading.ThreadPools.Simple;

namespace RadFramework.Libraries.Telemetry;

public class MultiplexTelemetryConnection
{
    private readonly ObjectPoolWithFactory<ISequentialDataTransferComponent> _dataTransferComponents;

    private QueuedThreadPool<byte[]> sendPool;
    private SimpleThreadPool receivePool;
    
    public MultiplexTelemetryConnection(IEnumerable<ISequentialDataTransferComponent> dataTransferComponents)
    {
        //_dataTransferComponents = new ObjectPoolWithFactory<ISequentialDataTransferComponent>();
        sendPool = new QueuedThreadPool<byte[]>(dataTransferComponents.Count(), ThreadPriority.Highest, SendInternal);
    }

    public void Send(byte[] data)
    {
        sendPool.Enqueue(data);
    }

    private void SendInternal(byte[] data)
    {
        ISequentialDataTransferComponent component = _dataTransferComponents.Reserve();
        component.Send(BitConverter.GetBytes(data.Length));
        component.Send(data);
        _dataTransferComponents.Release(component);
    }
}

/*public  class TelemetryConnection
{
    public TelemetryConnection(ISequentialDataTransferComponent dataTransferComponent)
    {
        _dataTransferComponents = new ObjectPoolWithFactory<ISequentialDataTransferComponent>();
        sendPool = new QueuedThreadPool<byte[]>(dataTransferComponents.Count(), ThreadPriority.Highest, SendInternal);
    }

    public virtual void Send(byte[] data)
    {
        sendPool.Enqueue(data);
    }

    private void SendInternal(byte[] data)
    {
        ISequentialDataTransferComponent component = _dataTransferComponents.Reserve();
        component.Send(BitConverter.GetBytes(data.Length));
        component.Send(data);
        _dataTransferComponents.Release(component);
    }
}*/