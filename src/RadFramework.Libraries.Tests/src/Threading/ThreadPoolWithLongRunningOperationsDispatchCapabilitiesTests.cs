using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using RadFramework.Libraries.Threading.ObjectRegistries;
using RadFramework.Libraries.Threading.ThreadPools.Simple;

namespace RadFramework.Libraries.Threading.Tests
{
    public class ThreadPoolWithLongRunningOperationsDispatchCapabilitiesTests
    {
        [Test]
        public void ThreadPool_CreatesThreadsThenCallsMethodBodyOnEachThreadThenDisposeTearsDownTheThreads()
        {
            // register calling threads here
            ObjectReferenceRegistry<Thread> processingThreads = new ObjectReferenceRegistry<Thread>();
            ObjectReferenceRegistry<Thread> longRunningThreads = new ObjectReferenceRegistry<Thread>();

            int i = 0;
            
            // test thread body
            void WorkerThreadBody()
            {
                // produce seven normal calls
                if (i < 7)
                {
                    processingThreads.Register(Thread.CurrentThread);
                    i++;
                    return;
                }
                
                longRunningThreads.Register(Thread.CurrentThread);
                
                // produce only long running operations
                Thread.Sleep(2000);
                
                i++;
            }
            
            // create a pool with 4 threads
            SimpleThreadPoolWithLongRunningOperationsDispatchCapabilities pool =
                new SimpleThreadPoolWithLongRunningOperationsDispatchCapabilities(
                    4,
                    ThreadPriority.Normal,
                    WorkerThreadBody,
                    1000,
                    ThreadPriority.Normal,
                    longRunningOperationLimit:20);
            
            // validate that 4 threads got created
            Assert.True(pool.ProcessingThreadRegistry.Count == 4);
            
            // wait 5 seconds
            Thread.Sleep(10000);

            // calculate the overall thread amount
            int overallManagedThreadsCount = pool.ProcessingThreadRegistry.Count + pool.LongRunningOperationsRegistry.Count;

            // tear down the thread pool
            pool.Dispose();

            // validate that all processing threads have stopped
            processingThreads.Concat(longRunningThreads).All(thread => thread.ThreadState == ThreadState.Stopped);
        }
        
        [Test]
        public void TestMultiThreadProcessorWithLongRunningOperationsDispatchCapabilities_ThreadBody()
        {
            Random rnd = new Random();

            int path = rnd.Next(0, 4);

            if (path == 0)
            {
                Thread.Sleep(15000);
                Console.WriteLine($"dispatched: {Thread.CurrentThread.ManagedThreadId}");
            }
            else
            {
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId);
            }

        }
    }
}