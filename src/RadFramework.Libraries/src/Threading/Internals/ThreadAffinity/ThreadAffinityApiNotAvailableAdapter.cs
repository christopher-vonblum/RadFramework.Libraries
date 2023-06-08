namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public class ThreadAffinityApiNotAvailableAdapter : IThreadAffinityApi
{
    public ulong GetCurrentThreadId()
    {
        return (ulong)Thread.CurrentThread.ManagedThreadId;
    }

    public void AssignAffinity(ulong nativeThreadId, int core)
    {
    }

    public void ResetAffinityAndCleanup(ulong nativeThreadId)
    {
    }
}