using System;
using System.Threading;

namespace RadFramework.Libraries.Threading.Internals
{
    public static class ThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixins
    {
        public static void AwaitThreadRunningPotentialLongRunningOperationAndReplaceThreadInPool(
            this IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer threadPool,
            Thread potentialLongRunningThread,
            Action processingMethodDelegate)
        {
            // start the thread
            potentialLongRunningThread.Start();
                
            // TODO: timer vs Join(ms)
            // wait until the dispatch timeout is reached in case its long running
            potentialLongRunningThread.Join(threadPool.LongRunningThreadDispatchTimeout);
                
            // if processing thread is long running it from the worker thread pool
            // and try to remove it from the pool to turn it a long running operation
            if (potentialLongRunningThread.ThreadState == ThreadState.Running 
             && threadPool.TryDispatchLongRunningOperationThread(Thread.CurrentThread, processingMethodDelegate))
            {
                // register the long running thread
                threadPool.LongRunningOperationsRegistry.Register(potentialLongRunningThread);
                
                // notify that a thread was dispatched from the pool
                threadPool.OnShiftedToLongRunningOperationsPool?.Invoke(potentialLongRunningThread);

                // if a cancellation timeout is defined
                if (threadPool.LongRunningOperationCancellationTimeout != 0)
                {
                    // wait until the cancellation timeout is reached
                    potentialLongRunningThread.Join(threadPool.LongRunningOperationCancellationTimeout);

                    // if the processing thread is still running
                    if (potentialLongRunningThread.ThreadState == ThreadState.Running)
                    {
                        try
                        {
                            // abort the thread
                            potentialLongRunningThread.Abort();
                        }
                        catch
                        {
                        }
                    }
                }
                // no cancellationTimeout?
                else
                {
                    // wait for the thread to join
                    potentialLongRunningThread.Join();
                }

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
            Thread thread,
            Action processingMethodDelegate)
        {
            if (threadPool.LongRunningOperationsRegistry.Count < threadPool.LongRunningOperationLimit)
            {
                threadPool.MoveToLongRunningOperationThreadPoolAndCreateReplacementThread(thread, processingMethodDelegate);
                return true;
            }

            return false;
        }
        
        public static void MoveToLongRunningOperationThreadPoolAndCreateReplacementThread(
            this IThreadPoolWithLongRunningOperationsDispatchCapabilitiesMixinsConsumer threadPool,
            Thread longRunningThread,
            Action processingMethodDelegate)
        {
            lock (threadPool.LongRunningOperationsRegistry)
            lock (threadPool.ProcessingThreadRegistry)
            {
                // remove long running thread from the pool
                if (threadPool.LongRunningOperationsRegistry.Unregister(longRunningThread))
                {
                    // create, start and register a new worker thread as a replacement
                    Thread newPoolThread = threadPool.CreateNewThread(processingMethodDelegate);
                    
                    threadPool.ProcessingThreadRegistry.Register(newPoolThread);

                    // register long running operation
                    threadPool.LongRunningOperationsRegistry.Register(longRunningThread);

                    // assign the thread priority for long running operations
                    longRunningThread.Priority = threadPool.LongRunningOperationThreadsPriority;
                }
            }
        }
    }
}