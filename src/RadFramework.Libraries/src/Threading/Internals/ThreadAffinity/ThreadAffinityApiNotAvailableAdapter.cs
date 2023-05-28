namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public class ThreadAffinityApiNotAvailableAdapter : IThreadAffinityApi
{
    public int GetNativeThreadId()
    {
        return Thread.CurrentThread.ManagedThreadId;
    }

    public void AssignAffinity(int nativeThreadId, BitMask processorMask)
    {
    }

    public void AssignAllProcessors(int nativeThreadId)
    {
    }
}