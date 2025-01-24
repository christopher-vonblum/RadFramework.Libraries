namespace RadFramework.Libraries.Threading.ThreadPools.Queued
{
    /// <summary>
    /// Abstraction for everything that queues delegates as workload.
    /// </summary>
    public interface IDelegateSheduler : IDisposable
    {
        /// <summary>
        /// Add a task to the queue.
        /// </summary>
        /// <param name="task">Action that will be executed on the thread pool.</param>
        void Enqueue(Action task);
        
        /// <summary>
        /// Add a bunch of tasks to the queue.
        /// </summary>
        /// <param name="tasks">Actions that will be executed on the thread pool.</param>
        void Enqueue(IEnumerable<Action> tasks);
    }
}