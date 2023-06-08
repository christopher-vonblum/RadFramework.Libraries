using System.Runtime.InteropServices;

namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

class UnixPthreadsApiAdapter : IThreadAffinityApi
{
    public ulong GetCurrentThreadId()
    {
        return Imports.GetCurrentThreadId();
    }

    public void AssignAffinity(ulong nativeThreadId, int core)
    {
        Imports.AssignAffinity(nativeThreadId, core);
    }

    public void ResetAffinityAndCleanup(ulong nativeThreadId)
    {
        Imports.ResetAffinityAndCleanup(nativeThreadId);
    }

    private class Imports
    {
        [DllImport("libLinuxThreadAffinityAdapter.so")]
        internal static extern ulong GetCurrentThreadId();
        
        [DllImport("libLinuxThreadAffinityAdapter.so")]
        internal static extern void AssignAffinity(ulong threadId, int core);
        
        [DllImport("libLinuxThreadAffinityAdapter.so")]
        internal static extern void ResetAffinityAndCleanup(ulong threadId);
    }
}