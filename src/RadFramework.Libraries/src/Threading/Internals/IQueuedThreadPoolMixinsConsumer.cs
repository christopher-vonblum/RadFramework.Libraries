using System.Collections.Concurrent;
using RadFramework.Libraries.Threading.Semaphores;

namespace RadFramework.Libraries.Threading.Internals
{
    public interface IQueuedThreadPoolMixinsConsumer<TQueueTask> : IThreadPoolMixinsConsumer
    {
        CounterSemaphore ProcessIncomingWorkSemaphore { get; }
        ConcurrentQueue<TQueueTask> Queue { get; }
        Action<TQueueTask> ProcessWorkloadDelegate { get; }
    }
}