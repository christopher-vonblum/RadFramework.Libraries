namespace RadFramework.Libraries.Threading.Internals
{
    /// <summary>
    /// Shared methods to create queued ThreadPools.
    /// </summary>
    public static class QueuedThreadPoolMixins
    {
        /// <summary>
        /// If queue contains workloads trigger releases on the semaphore to release threads that will do the work. 
        /// </summary>
        /// <typeparam name="TQueueTask">Type of the workloads the pool processes.</typeparam>
        public static void TryKickOffProcessingThreads<TQueueTask>(
            this IQueuedThreadPoolMixinsConsumer<TQueueTask> threadPool)
        {
            if (threadPool.Queue.IsEmpty)
            {
                return;
            }
            
            threadPool.ProcessIncomingWorkSemaphore.Release(threadPool.Queue.Count);
        }
        
        /// <summary>
        /// Enqueues a workload and kicks off the processing threads.
        /// </summary>
        /// <param name="task">The workload</param>
        public static void Enqueue<TQueueTask>(
            this IQueuedThreadPoolMixinsConsumer<TQueueTask> threadPool, TQueueTask task)
        {
            threadPool.Queue.Enqueue(task);

            threadPool.TryKickOffProcessingThreads();
        }

        /// <summary>
        /// Enqueues a bunch of workload then kicks off the processing threads.
        /// </summary>
        /// <param name="task">The workload</param>
        public static void Enqueue<TQueueTask>(
            this IQueuedThreadPoolMixinsConsumer<TQueueTask> threadPool, IEnumerable<TQueueTask> tasks)
        {
            foreach (var request in tasks)
            {
                threadPool.Queue.Enqueue(request);
            }
            
            threadPool.TryKickOffProcessingThreads();
        }
    }
}