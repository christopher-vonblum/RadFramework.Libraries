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
            int amountPerCore,
            Action threadBody,
            ThreadPriority threadPriority,
            string poolDescription)
        {
            for (int core = 0; core < Environment.ProcessorCount; core++)
            {
                for (int i = 0; i < amountPerCore; i++)
                {
                    threadPool.ProcessingThreadRegistry.Register(threadPool.CreateNewThread(threadBody, core));
                }                
            }
        }

        /// <summary>
        /// Creates a loop thread and registeres it in the ProcessingThreads collection.
        /// </summary>
        /// <returns>The created thread.</returns>
        public static PoolThread CreateNewThread(
            this IThreadPoolMixinsConsumer threadPool,
            Action processingMethodDelegate,
            int core)
        {
            return new PoolThread(processingMethodDelegate, core, threadPool.ProcessingThreadPriority, threadPool.ThreadDescription);
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