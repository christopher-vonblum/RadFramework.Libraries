using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Threading.ThreadPools.DelegateShedulers.Queued
{
    /// <summary>
    /// Adapter class for the built in ThreadPool of .NET Framework.
    /// </summary>
    public class MicrosoftThreadPoolDelegateSheduler : IDelegateSheduler
    {
        /// <summary>
        /// Enqueues the task delegate on the thread pool.
        /// </summary>
        /// <param name="task">The task delegate</param>
        public void Enqueue(Action task)
        {
            System.Threading.ThreadPool.QueueUserWorkItem(state => task());
        }

        /// <summary>
        /// Enqueues the task delegates on the thread pool.
        /// </summary>
        /// <param name="tasks">The task delegates</param>
        public void Enqueue(IEnumerable<Action> tasks)
        {
            foreach (var task in tasks)
            {
                Enqueue(task);
            }
        }
        
        public void Dispose()
        {
        }
    }
}