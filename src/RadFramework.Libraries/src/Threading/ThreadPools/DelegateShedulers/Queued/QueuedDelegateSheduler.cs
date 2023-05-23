using System;
using System.Collections.Generic;
using System.Threading;
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
        /// <param name="processingPoolSize"></param>
        /// <param name="priority"></param>
        /// <param name="threadDescription"></param>
        public QueuedDelegateSheduler(
            int processingPoolSize,
            ThreadPriority priority,
            string threadDescription)
            : base(
                processingPoolSize,
                priority,
                (a) => a(),
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