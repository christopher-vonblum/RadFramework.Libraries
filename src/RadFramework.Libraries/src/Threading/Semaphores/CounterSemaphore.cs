using System;
using System.Collections.Generic;
using System.Threading;
using RadFramework.Libraries.Threading.ObjectPools;

namespace RadFramework.Libraries.Threading.Semaphores
{
    /// <summary>
    /// A simple Semaphore that is able to trap threads and release them.
    /// </summary>
    public class CounterSemaphore : IDisposable
    {
        /// <summary>
        /// Object pool for wait events sized by the thread count of the semaphore.
        /// </summary>
        private ObjectPoolWithFactory<AutoResetEvent> _waitEventPoolWithFactory;
        
        /// <summary>
        /// Maps thread id to wait event
        /// </summary>
        private Dictionary<AutoResetEvent, AutoResetEvent> eventsInTrapCollection
            = new Dictionary<AutoResetEvent, AutoResetEvent>();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="estimatedTrapThreadCount">A raw estimate amount of threads that will WaitHere().</param>
        public CounterSemaphore(int estimatedTrapThreadCount)
        {
            _waitEventPoolWithFactory = new ObjectPoolWithFactory<AutoResetEvent>(
                () => new AutoResetEvent(false),
                @event => @event.Dispose(),
                estimatedTrapThreadCount
            );
        }
        
        /// <summary>
        /// Traps threads until the Release(int) method gets called.
        /// </summary>
        public void WaitHere()
        {
            // Use a pooled AutoResetEvent
            AutoResetEvent waitEvent = _waitEventPoolWithFactory.Reserve();
            
            // store the AutoResetEvent so that it can be a candidate for the Release(int) method
            eventsInTrapCollection[waitEvent] = waitEvent;
            
            // trap the caller thread here
            waitEvent.WaitOne();
            
            // remove the event because a Release(int) call caused the trap to end
            eventsInTrapCollection.Remove(waitEvent);
            
            // tell the object pool to adopt the AutoResetEvent again
            _waitEventPoolWithFactory.Release(waitEvent);
        }

        /// <summary>
        /// Releases a specified amount of threads that are trapped in WaitHere().
        /// </summary>
        /// <param name="threadCountToRelease">Amount of threads to release</param>
        /// <returns>Amount of threads that were actually released</returns>
        public int Release(int threadCountToRelease)
        {
            // counts how many threads were actually released.
            int released = 0;

            // foreach registered thread
            foreach(var e in eventsInTrapCollection)
            {
                // if we released enough threads to handle the workload
                if (released >= threadCountToRelease)
                {
                    break;
                }
                
                // release the thread trapped in WaitHere()
                e.Key.Set();
                
                // Increment the relesed counter
                released++;
            }

            // Return the ampount of released threads
            return released;
        }

        /// <summary>
        /// Disposes the semaphore.
        /// </summary>
        public void Dispose()
        {
            _waitEventPoolWithFactory.Dispose();
        }
    }
}