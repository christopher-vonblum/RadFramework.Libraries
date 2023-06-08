using RadFramework.Libraries.Threading.Internals.ThreadAffinity;

namespace RadFramework.Libraries.Threading;

public class PoolThread : IDisposable
{
    private readonly Action _threadStart;
    private readonly int _core;
    private ManualResetEventSlim onThreadStart = new ManualResetEventSlim(false);
    
    public Thread ThreadingThread { get; }
    
    public ulong ThreadId { get; private set; }

    public PoolThread(Action threadStart, int core, ThreadPriority threadPriority, string threadDescription)
    {
        _threadStart = threadStart;
        _core = core;
        ThreadingThread = new Thread(ThreadStart);
        ThreadingThread.Priority = threadPriority;
        ThreadingThread.Name = threadDescription;
        ThreadingThread.Start();
    }

    private void ThreadStart()
    {
        ThreadId = ThreadAffinityApi.GetCurrentThreadId();
        
        ThreadAffinityApi.AssignAffinity(ThreadId, _core);

        onThreadStart.Wait();

        _threadStart();
    }

    public void Start()
    {
        onThreadStart.Set();
    }

    public void Dispose()
    {
        onThreadStart.Dispose();
    }
}