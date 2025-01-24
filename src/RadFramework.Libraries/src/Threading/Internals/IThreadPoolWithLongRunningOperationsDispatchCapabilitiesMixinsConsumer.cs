using RadFramework.Libraries.Collections;

namespace RadFramework.Libraries.Threading.Internals
{
    public interface IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer : IThreadPoolMixinsConsumer
    {
        Action<PoolThread> OnShiftedToLongRunningOperationsPool { get; }
        int LongRunningThreadDispatchTimeout { get; }
        int LongRunningOperationCancellationTimeout { get; }
        int LongRunningOperationLimit { get; }
        ObjectReferenceRegistry<PoolThread> LongRunningOperationsRegistry { get; }
        ThreadPriority LongRunningOperationThreadsPriority { get; }
    }
}