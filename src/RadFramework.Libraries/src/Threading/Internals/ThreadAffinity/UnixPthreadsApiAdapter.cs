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
        [DllImport("dep/libLinuxThreadAffinityAdapter.so")]
        internal static extern ulong GetCurrentThreadId();
        
        [DllImport("dep/libLinuxThreadAffinityAdapter.so")]
        internal static extern void AssignAffinity(ulong threadId, int core);
        
        [DllImport("dep/libLinuxThreadAffinityAdapter.so")]
        internal static extern void ResetAffinityAndCleanup(ulong threadId);
    }
}