using System;
using System.Threading;
using RadFramework.Libraries.Threading.ObjectRegistries;

namespace RadFramework.Libraries.Threading.Internals
{
    public abstract class ThreadPoolBase
        : IThreadPoolMixinsConsumer, IDisposable
    {
        /// <summary>
        /// Gets called when an error occurs on one of the pool threads.
        /// </summary>
        public Action<Thread, Exception> OnError { get; set; }
        
        /// <summary>
        /// The method called from the loop threads.
        /// </summary>
        protected Action ProcessWorkloadDelegate { get; }
        
        /// <summary>
        /// String description of the looping threads. Makes it easy to identify pool threads when listing threads with the debugger.
        /// </summary>
        public string ThreadDescription { get; }

        /// <summary>
        /// The ThreadPriority the pool threads have.
        /// </summary>
        public ThreadPriority ProcessingThreadPriority { get; }
        
        /// <summary>
        /// Holds the references to the looping threads.
        /// </summary>
        public ObjectReferenceRegistry<Thread> ProcessingThreadRegistry { get; } = new ObjectReferenceRegistry<Thread>();
        
        /// <summary>
        /// Is true when the processor gets teared down.
        /// All loops get stopped by Dispose() when set to true.
        /// </summary>
        private bool isDisposed;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processingThreadAmount">The amount of loop threads to create.</param>
        /// <param name="processingThreadPriority">The priority of the loop threads.</param>
        /// <param name="processingDelegate">The method that the looping threads call.</param>
        /// <param name="threadDescription">String description of the looping threads. Makes it easy to identify pool threads when listing threads with the debugger.</param>
        protected ThreadPoolBase(int processingThreadAmount, ThreadPriority processingThreadPriority, Action processingDelegate, string threadDescription = null)
        {
            ThreadDescription = threadDescription
                                     // Default loop thread description consisting of the method name and the implementation type that the MultiThreadProcessor derives from.
                                     ?? $"loopedProcessingMethod:{processingDelegate.Method.DeclaringType?.FullName}.{processingDelegate.Method.Name}, ThreadPoolType:{GetType().FullName}";
            ProcessWorkloadDelegate = processingDelegate;
            ProcessingThreadPriority = processingThreadPriority;
            this.CreateThreads(processingThreadAmount, ProcessingLoop);
        }

        /// <summary>
        /// This wrapper ensures that the thread is only registered while it is actually running.
        /// Calls the InvokeProcessWorkloadAction() in a loop.
        /// </summary>
        public virtual void ProcessingLoop()
        {
            // While were not disposing and thread is part of the pool
            while (!LoopShutdownReasonsApply())
            {
                this.InvokeProcessWorkloadActionWithErrorHandling(ProcessWorkloadUnit);
            }
        }

        /// <summary>
        /// Processes a single unit of work.
        /// </summary>
        public virtual void ProcessWorkloadUnit()
        {
            ProcessWorkloadDelegate();
        }
        
        /// <summary>
        /// Checks if the loop thread should end.
        /// Ends when disposing or when the calling thread is not part of the pool.
        /// </summary>
        /// <returns>true when the threads should end.</returns>
        protected virtual bool LoopShutdownReasonsApply()
        {
            // stop processing workloads when disposed or the thread is not in the pool anymore
            return isDisposed || !this.ProcessingThreadRegistry.IsRegistered(Thread.CurrentThread);
        }

        /// <summary>
        /// Tears everything down. Waits for all pool threads to end
        /// </summary>
        public virtual void Dispose()
        {
            isDisposed = true;
            AwaitAllProcessingThreadsExit(ProcessingThreadRegistry);
        }

        /// <summary>
        /// Waits for all threads in the pool collection to join.
        /// </summary>
        /// <param name="pool"></param>
        protected void AwaitAllProcessingThreadsExit(ObjectReferenceRegistry<Thread> pool)
        {
            foreach (Thread thread in ProcessingThreadRegistry)
            {
                thread.Join();
            }
        }
    }
}