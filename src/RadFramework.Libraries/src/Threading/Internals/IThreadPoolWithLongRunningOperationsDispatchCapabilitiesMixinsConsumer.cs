using System;
using System.Threading;
using RadFramework.Libraries.Threading.ObjectRegistries;

namespace RadFramework.Libraries.Threading.Internals
{
    public interface IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer : IThreadPoolMixinsConsumer
    {
        Action<Thread> OnShiftedToLongRunningOperationsPool { get; }
        int LongRunningThreadDispatchTimeout { get; }
        int LongRunningOperationCancellationTimeout { get; }
        int LongRunningOperationLimit { get; }
        ObjectReferenceRegistry<Thread> LongRunningOperationsRegistry { get; }
        ThreadPriority LongRunningOperationThreadsPriority { get; }
    }
}