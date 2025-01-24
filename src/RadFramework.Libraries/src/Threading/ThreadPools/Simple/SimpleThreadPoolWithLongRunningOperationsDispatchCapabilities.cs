using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ObjectRegistries;

namespace RadFramework.Libraries.Threading.ThreadPools.Simple
{
    /// <summary>
    /// A specialized MultiThreadProcessor that can host long running operations.
    /// </summary>
    public class SimpleThreadPoolWithLongRunningOperationsDispatchCapabilities 
        : ThreadPoolBase, 
          IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer
    {
        /// <summary>
        /// The milliseconds to wait for a thread to get removed from the pool and treated as a long running operation.
        /// </summary>
        public int LongRunningThreadDispatchTimeout { get; }

        /// <summary>
        /// Threads that are long running get stored here.
        /// </summary>
        public ObjectReferenceRegistry<PoolThread> LongRunningOperationsRegistry { get; } =
            new ObjectReferenceRegistry<PoolThread>();

        /// <summary>
        /// The priority that gets assigned to a thread when leaving the pool as a long running operation.
        /// </summary>
        public ThreadPriority LongRunningOperationThreadsPriority { get; }

        /// <summary>
        /// Gets called when a pool thread turns an long running operation.
        /// </summary>
        public Action<PoolThread> OnShiftedToLongRunningOperationsPool { get; }
        
        /// <summary>
        /// Limit of long running operations at the same time.
        /// </summary>
        public int LongRunningOperationLimit { get; }
        
        /// <summary>
        /// The timeout for long running operation threads to get terminated.
        /// </summary>
        public int LongRunningOperationCancellationTimeout { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processingThreadAmount">Amount of processing threads to create</param>
        /// <param name="processingThreadPriority">Priority for the processing threads</param>
        /// <param name="processingMethod">Method to call inside of the processing threads loops</param>
        /// <param name="longRunningThreadDispatchTimeout">Timeout after that a thread counts as long running</param>
        /// <param name="longRunningOperationThreadsPriority">Priority to assign to long running processing threads</param>
        /// <param name="threadDescription">Description the Debugger shows for processing threads</param>
        /// <param name="onShiftedToLongRunningOperationsPool">Called when a thread was identified as long running</param>
        /// <param name="longRunningOperationLimit">Limit of long running threads</param>
        /// <param name="longRunningOperationCancellationTimeout">Timeout after that a long running thread gets terminated</param>
        public SimpleThreadPoolWithLongRunningOperationsDispatchCapabilities(
            int processingThreadAmount,
            ThreadPriority processingThreadPriority,
            Action processingMethod,
            int longRunningThreadDispatchTimeout,
            ThreadPriority longRunningOperationThreadsPriority,
            string threadDescription = null,
            Action<PoolThread> onShiftedToLongRunningOperationsPool = null, 
            int longRunningOperationLimit = 0,
            int longRunningOperationCancellationTimeout = 0) 
            : base(processingThreadAmount, processingThreadPriority, processingMethod, threadDescription)
        {
            LongRunningThreadDispatchTimeout = longRunningThreadDispatchTimeout;
            LongRunningOperationThreadsPriority = longRunningOperationThreadsPriority;
            OnShiftedToLongRunningOperationsPool = onShiftedToLongRunningOperationsPool;
            LongRunningOperationLimit = longRunningOperationLimit;
            LongRunningOperationCancellationTimeout = longRunningOperationCancellationTimeout;
            this.StartThreads();
        }
        
        public override void ProcessWorkloadUnit()
        {
            PoolThread thread = this.CreateNewThread(
                ProcessWorkloadDelegate, 
                PoolThread.GetPoolThread(Thread.CurrentThread).AssignedCore);
            
            this.AwaitThreadRunningPotentialLongRunningOperationAndReplaceThreadInPool(thread, this.ProcessingLoop);
        }

        public override void Dispose()
        {
            base.Dispose();
            AwaitAllProcessingThreadsExit(LongRunningOperationsRegistry);
        }
    }
}