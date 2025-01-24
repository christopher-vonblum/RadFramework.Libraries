using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ObjectRegistries;

namespace RadFramework.Libraries.Threading.ThreadPools.Queued
{
    /// <summary>
    /// A MultiThreadProcessor that queues up tasks and executes them on a pool of threads.
    /// This specific processor also shifts pool threads to another pool for long running operations.
    /// </summary>
    /// <typeparam name="TQueueTask">Type of workloads that the loop processes.</typeparam>
    public class QueuedThreadPoolWithLongRunningOperationsDispatchCapabilities<TQueueTask> 
        : QueuedThreadPool<TQueueTask>, 
          IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer
    {
        /// <summary>
        /// Gets invoked when a thread is identified as a long running operation
        /// </summary>
        public Action<PoolThread> OnShiftedToLongRunningOperationsPool { get; }
        
        /// <summary>
        /// Defines the timeout in milliseconds after a thread goes to the long running operations pool.
        /// </summary>
        public int LongRunningThreadDispatchTimeout { get; }
        
        /// <summary>
        /// Defines the timeout in milliseconds after that a long running operation should be cancelled.
        /// </summary>
        public int LongRunningOperationCancellationTimeout { get; }
        
        /// <summary>
        /// Amount of long running operation threads allowed.
        /// </summary>
        public int LongRunningOperationLimit { get; }

        /// <summary>
        /// Threads that are long running get stored here.
        /// </summary>
        public ObjectReferenceRegistry<PoolThread> LongRunningOperationsRegistry { get; } =
            new ObjectReferenceRegistry<PoolThread>();
 
        /// <summary>
        /// The priority to assign when a thread gets moved to the long running operations pool.
        /// </summary>
        public ThreadPriority LongRunningOperationThreadsPriority { get; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="processingThreadAmount"></param>
        /// <param name="processingThreadPriority"></param>
        /// <param name="processingMethod"></param>
        /// <param name="dispatchLongRunningThreadTimeout"></param>
        /// <param name="longRunningOperationThreadsPriority"></param>
        /// <param name="threadDescription"></param>
        /// <param name="onShiftedToLongRunningOperationsPool"></param>
        /// <param name="longRunningOperationLimit"></param>
        /// <param name="longRunningOperationCancellationTimeout"></param>
        public QueuedThreadPoolWithLongRunningOperationsDispatchCapabilities(
            int processingThreadAmount,
            ThreadPriority processingThreadPriority,
            Action<TQueueTask> processingMethod,
            int dispatchLongRunningThreadTimeout,
            ThreadPriority longRunningOperationThreadsPriority,
            string threadDescription = null,
            Action<PoolThread> onShiftedToLongRunningOperationsPool = null, 
            int longRunningOperationLimit = 0,
            int longRunningOperationCancellationTimeout = 0) 
            : base(processingThreadAmount, processingThreadPriority, processingMethod, threadDescription)
        {
            LongRunningThreadDispatchTimeout = dispatchLongRunningThreadTimeout;
            LongRunningOperationThreadsPriority = longRunningOperationThreadsPriority;
            OnShiftedToLongRunningOperationsPool = onShiftedToLongRunningOperationsPool;
            LongRunningOperationLimit = longRunningOperationLimit;
            LongRunningOperationCancellationTimeout = longRunningOperationCancellationTimeout;
        }

        public override void ProcessingLoop()
        {
            // While were not disposing and thread is part of the pool
            while (!LoopShutdownReasonsApply())
            {
                if (Queue.TryDequeue(out TQueueTask task))
                {
                    PoolThread newThread = this.CreateNewThread(
                        () => ProcessWorkloadUnitInternal(task),
                        PoolThread.GetPoolThread(Thread.CurrentThread).AssignedCore);
                    
                    // start processing the unit of work.
                    newThread.Start();
                    
                    // Check if more workloads are on the queue and start to process them on the other threads.
                    this.TryKickOffProcessingThreads();
                    
                    this.AwaitThreadRunningPotentialLongRunningOperationAndReplaceThreadInPool(
                        newThread,
                        () => ProcessWorkloadUnitInternal(task));
                }
                else
                {
                    ProcessIncomingWorkSemaphore.WaitHere();
                }
            }
        }

        /// <summary>
        /// Processes a single unit of work.
        /// </summary>
        /// <param name="task">The task to process.</param>
        private void ProcessWorkloadUnitInternal(TQueueTask task)
        {
            this.InvokeProcessWorkloadActionWithErrorHandling(
                () => ProcessWorkloadDelegate(task));
        }

        /// <summary>
        /// This should not be called by the custom loop.
        /// </summary>
        /// <exception cref="NotSupportedException">Thrown when called.</exception>
        public override void ProcessWorkloadUnit()
        {
            throw new NotSupportedException();
        }

        public override void Dispose()
        {
            base.Dispose();
            this.AwaitAllProcessingThreadsExit(LongRunningOperationsRegistry);
        }
    }
}