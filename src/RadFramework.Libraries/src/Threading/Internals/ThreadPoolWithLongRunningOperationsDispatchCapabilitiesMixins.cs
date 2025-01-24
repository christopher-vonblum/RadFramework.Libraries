namespace RadFramework.Libraries.Threading.Internals
{
    public static class ThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixins
    {
        public static void AwaitThreadRunningPotentialLongRunningOperationAndReplaceThreadInPool(
            this IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer threadPool,
            PoolThread potentialLongRunningThread,
            Action processingMethodDelegate)
        {
            // start the thread
            potentialLongRunningThread.Start();
                
            // TODO: timer vs Join(ms)
            // wait until the dispatch timeout is reached in case its long running
            potentialLongRunningThread.ThreadingThread.Join(threadPool.LongRunningThreadDispatchTimeout);
                
            // if processing thread is long running it from the worker thread pool
            // and try to remove it from the pool to turn it a long running operation
            if (potentialLongRunningThread.ThreadingThread.ThreadState == ThreadState.Running 
             && threadPool.TryDispatchLongRunningOperationThread(PoolThread.GetPoolThread(Thread.CurrentThread), processingMethodDelegate))
            {
                // register the long running thread
                threadPool.LongRunningOperationsRegistry.Register(potentialLongRunningThread);
                
                // notify that a thread was dispatched from the pool
                threadPool.OnShiftedToLongRunningOperationsPool?.Invoke(potentialLongRunningThread);

                // wait for the thread to join
                potentialLongRunningThread.ThreadingThread.Join();

                // Remove the thread from long running operations if present
                threadPool.LongRunningOperationsRegistry.Unregister(potentialLongRunningThread);
            }
        }
        
        /// <summary>
        /// Attemps to move the processing thread to the long running operations pool.
        /// </summary>
        /// <param name="thread">The thread that should be moved to the long running operations pool.</param>
        /// <returns>True if enough slots for long running operations are available</returns>
        public static bool TryDispatchLongRunningOperationThread(
            this IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer threadPool,
            PoolThread thread,
            Action processingMethodDelegate)
        {
            if (threadPool.LongRunningOperationsRegistry.Count < threadPool.LongRunningOperationLimit)
            {
                threadPool.MoveToLongRunningOperationThreadPoolAndCreateReplacementThread(thread);
                return true;
            }

            return false;
        }
        
        public static void MoveToLongRunningOperationThreadPoolAndCreateReplacementThread(
            this IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer threadPool,
            PoolThread longRunningThread)
        {
            lock (threadPool.LongRunningOperationsRegistry)
            lock (threadPool.ProcessingThreadRegistry)
            {
                // remove long running thread from the pool
                if (threadPool.LongRunningOperationsRegistry.Unregister(longRunningThread))
                {
                    // create, start and register a new worker thread as a replacement
                    PoolThread newPoolThread = threadPool.CreateNewThread(longRunningThread.ThreadBody, longRunningThread.AssignedCore);
                    
                    threadPool.ProcessingThreadRegistry.Register(newPoolThread);

                    // register long running operation
                    threadPool.LongRunningOperationsRegistry.Register(longRunningThread);
                }
            }
        }
    }
}