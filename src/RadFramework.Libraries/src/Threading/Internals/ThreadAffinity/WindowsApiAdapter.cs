using System.Diagnostics;

namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public class WindowsApiAdapter : IThreadAffinityApi
{
    public ulong GetCurrentThreadId()
    {
        throw new NotImplementedException();
    }

    public void AssignAffinity(ulong nativeThreadId, int core)
    {
        throw new NotImplementedException();
    }

    public void ResetAffinityAndCleanup(ulong nativeThreadId)
    {
        throw new NotImplementedException();
    }
}