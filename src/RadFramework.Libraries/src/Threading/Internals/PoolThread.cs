using RadFramework.Libraries.Threading.Internals.ThreadAffinity;

namespace RadFramework.Libraries.Threading;

public class PoolThread : IDisposable
{
    private static List<PoolThread> GlobalPoolThreadRegistry = new List<PoolThread>();

    internal static PoolThread GetPoolThread(Thread thread)
    {
        return GlobalPoolThreadRegistry.Single(t => t.ThreadingThread == thread);
    }
    
    public Action ThreadBody { get; private set; }
    
    private ManualResetEventSlim onThreadStart = new ManualResetEventSlim(false);
    
    public Thread ThreadingThread { get; }
    
    public ulong ThreadId { get; private set; }
    public int AssignedCore { get; set; }

    public PoolThread(Action threadBody, int core, ThreadPriority threadPriority, string threadDescription)
    {
        ThreadBody = threadBody;
        AssignedCore = core;
        ThreadingThread = new Thread(ThreadStart);
        ThreadingThread.Priority = threadPriority;
        ThreadingThread.Name = threadDescription;
        GlobalPoolThreadRegistry.Add(this);
        ThreadingThread.Start();
    }

    private void ThreadStart()
    {
        ThreadId = ThreadAffinityApi.GetCurrentThreadId();
        
        ThreadAffinityApi.AssignAffinity(ThreadId, AssignedCore);

        onThreadStart.Wait();

        ThreadBody();
    }

    public void Start()
    {
        onThreadStart.Set();
    }

    public void Dispose()
    {
        onThreadStart.Dispose();
        GlobalPoolThreadRegistry.Remove(this);
    }
}