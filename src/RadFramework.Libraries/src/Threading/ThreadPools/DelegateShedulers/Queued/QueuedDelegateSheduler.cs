using RadFramework.Libraries.Threading.Internals;
using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Threading.ThreadPools.DelegateShedulers.Queued
{
    /// <summary>
    /// A thread pool featuring a queue for workloads.
    /// </summary>
    public class QueuedDelegateSheduler : QueuedThreadPool<Action>, IDelegateSheduler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="threadAmountPerCore"></param>
        /// <param name="priority"></param>
        /// <param name="threadDescription"></param>
        public QueuedDelegateSheduler(
            int threadAmountPerCore,
            ThreadPriority priority,
            Action<Action, PoolThread, Exception> processingWorkloadYieldedError,
            string threadDescription = null)
            : base(
                threadAmountPerCore,
                priority,
                (a) => a(),
                processingWorkloadYieldedError,
                threadDescription)
        {
        }

        public void Enqueue(Action task)
        {
            QueuedThreadPoolMixins.Enqueue(this, task);
        }

        public void Enqueue(IEnumerable<Action> tasks)
        {
            QueuedThreadPoolMixins.Enqueue(this, tasks);
        }
    }
}