using RadFramework.Libraries.Threading.Internals;

namespace RadFramework.Libraries.Threading.ThreadPools.Simple
{
    /// <summary>
    /// The simplest form of a MultiThreadProcessor.
    /// </summary>
    public class SimpleThreadPool : ThreadPoolBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="processingThreadPoolSize">Amount of processing threads to use</param>
        /// <param name="processingThreadPriority">ThreadPriority of the processingThreads</param>
        /// <param name="processWorkloadAction">The delegate that gets called to process a workload</param>
        /// <param name="threadDescription">The thread name property of all processing threads will be set to this</param>
        public SimpleThreadPool(int processingThreadPoolSize, ThreadPriority processingThreadPriority, Action processWorkloadAction, string threadDescription = null) : base(processingThreadPoolSize, processingThreadPriority, processWorkloadAction, threadDescription)
        {
            this.StartThreads();
        }
        
        // pointer as dictionary key
        // custom timer
        // instance pool that extends
        // pthread- affinity
    }
}