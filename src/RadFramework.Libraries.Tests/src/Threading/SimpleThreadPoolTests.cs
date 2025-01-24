using System.Linq;
using System.Threading;
using NUnit.Framework;
using RadFramework.Libraries.Collections;
using RadFramework.Libraries.Threading.ThreadPools.Simple;

namespace RadFramework.Libraries.Threading.Tests
{
    public class SimpleThreadPoolTests
    {
        [Test]
        public void ThreadPool_CreatesThreadsThenCallsMethodBodyOnEachThreadThenDisposeTearsDownTheThreads()
        {
            // register calling threads here
            ObjectReferenceRegistry<Thread> threadBodyCalledByThreads = new ObjectReferenceRegistry<Thread>();
            
            // test thread body
            void WorkerThreadBody()
            {
                threadBodyCalledByThreads.Register(Thread.CurrentThread);
            }
            
            // create a pool with 4 threads
            SimpleThreadPool pool = new SimpleThreadPool(4, ThreadPriority.Normal, WorkerThreadBody);
            
            // validate that 8 threads got created
            Assert.True(pool.ProcessingThreadRegistry.Count == 4);
            
            // wait 3 seconds
            Thread.Sleep(4000);
            
            // 4 individual threads should be registered now.
            Assert.True(threadBodyCalledByThreads.Count == 4);
            
            // tear down the thread pool
            pool.Dispose();

            // validate that all processing threads have stopped
            Assert.True(threadBodyCalledByThreads.All(thread => thread.ThreadState == ThreadState.Stopped));
        }
    }
}