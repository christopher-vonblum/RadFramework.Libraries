using System;
using System.Collections.Generic;
using RadFramework.Libraries.Threading.ThreadPools.Queued;

namespace RadFramework.Libraries.Threading.ThreadPools.DelegateShedulers
{
    /// <summary>
    /// Dummy class to execute everything on the current thread.
    /// </summary>
    public class StayInCurrentThreadDelegateSheduler : IDelegateSheduler
    {
        /// <summary>
        /// Executes the task delegate.
        /// </summary>
        /// <param name="task">The task delegate</param>
        public void Enqueue(Action task)
        {
            task();
        }

        /// <summary>
        /// Executes the task delegates.
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