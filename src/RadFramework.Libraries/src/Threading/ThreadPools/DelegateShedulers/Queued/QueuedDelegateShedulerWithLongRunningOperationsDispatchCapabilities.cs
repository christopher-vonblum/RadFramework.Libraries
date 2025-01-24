using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Threading.ThreadPools.DelegateShedulers.Queued
{
    public class QueuedDelegateShedulerWithLongRunningOperationsDispatchCapabilities : QueuedThreadPoolWithLongRunningOperationsDispatchCapabilities<Action>, IDelegateSheduler
    {        
        public QueuedDelegateShedulerWithLongRunningOperationsDispatchCapabilities(
            int processingThreadAmount, 
            ThreadPriority processingThreadPriority,
            int dispatchLongRunningThreadTimeout,
            Action<Action, PoolThread, Exception> processingWorkloadYieldedError,
            ThreadPriority longRunningOperationThreadsPriority,
            string threadDescription = null, 
            Action<PoolThread> onShiftedToLongRunningOperationsPool = null, 
            int longRunningOperationLimit = 0, 
            int longRunningOperationCancellationTimeout = 0) 
            : base(
                processingThreadAmount,
                processingThreadPriority,
                delegate(Action action) { action();  },
                processingWorkloadYieldedError,
                dispatchLongRunningThreadTimeout,
                longRunningOperationThreadsPriority,
                threadDescription,
                onShiftedToLongRunningOperationsPool,
                longRunningOperationLimit,
                longRunningOperationCancellationTimeout)
        {
        }
        
        public void Enqueue(Action task)
        {
            QueuedThreadPoolMixins.Enqueue(this, task);
        }

        public void Enqueue(IEnumerable<Action> tasks)
        {
            QueuedThreadPoolMixins.Enqueue(this, tasks);
        }
    }
}