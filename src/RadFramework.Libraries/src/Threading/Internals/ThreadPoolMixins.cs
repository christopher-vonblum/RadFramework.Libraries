using System;
using System.Threading;

namespace RadFramework.Libraries.Threading.Internals
{
    public static class ThreadPoolMixins
    {
        /// <summary>
        /// Creates the looping threads and registers them in the LoopThread collection.
        /// </summary>
        /// <param name="amount">Amount of Threads to create.</param>
        public static void CreateThreads(
            this IThreadPoolMixinsConsumer threadPool,
            int amount,
            Action threadBody)
        {
            int core = 1;
            for (int i = 0; i < amount; i++)
            {
                
                var thread = threadPool.CreateNewThread(threadBody);
                threadPool.ProcessingThreadRegistry.Register(new PoolThread(threadBody));
            }
        }

        /// <summary>
        /// Creates a loop thread and registeres it in the ProcessingThreads collection.
        /// </summary>
        /// <returns>The created thread.</returns>
        public static Thread CreateNewThread(
            this IThreadPoolMixinsConsumer threadPool,
            Action processingMethodDelegate)
        {
            Thread newThread = new Thread(s => processingMethodDelegate());
            newThread.Priority = threadPool.ProcessingThreadPriority;
            newThread.Name = threadPool.ThreadDescription;
            return newThread;
        }

        /// <summary>
        /// Starts all loop threads. Typically called once by the constructor of the derived type.
        /// </summary>
        public static void StartThreads(
            this IThreadPoolMixinsConsumer threadPool)
        {
            foreach (var processingThread in threadPool.ProcessingThreadRegistry)
            {
                processingThread.Start();
            }
        }
        
        /// <summary>
        /// Gets called to process a workload by Loop().
        /// Is secured by a try-catch-handler that reports errors that occur on the Thread that calls the handler action.
        /// </summary>
        public static void InvokeProcessWorkloadActionWithErrorHandling(
            this IThreadPoolMixinsConsumer threadPool,
            Action processWorkloadUnit)
        {
            // Try to process a workload
            try
            {
                processWorkloadUnit();
            }
            catch(Exception e)
            {
                // When processing fails report the error to the OnError handler if present.
                threadPool.OnError?.Invoke(Thread.CurrentThread, e);
            }
        }
    }
}