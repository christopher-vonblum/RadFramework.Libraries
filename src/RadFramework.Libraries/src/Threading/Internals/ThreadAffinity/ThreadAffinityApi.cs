namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public static class ThreadAffinityApi
{
    private static IThreadAffinityApi api;
    static ThreadAffinityApi()
    {
        if (OperatingSystem.IsWindows())
        {
            api = new WindowsApiAdapter();
        }
        else if (OperatingSystem.IsLinux())
        {
            api = new UnixPthreadsApiAdapter();
        }
        else
        {
            api = new ThreadAffinityApiNotAvailableAdapter();
        }
    }
    
    public static ulong GetCurrentThreadId()
    {
        return api.GetCurrentThreadId();
    }

    public static void AssignAffinity(ulong nativeThreadId, int core)
    {
        api.AssignAffinity(nativeThreadId, core);
    }

    public static void ResetAffinityAndCleanup(ulong nativeThreadId)
    {
        api.ResetAffinityAndCleanup(nativeThreadId);
    }
}