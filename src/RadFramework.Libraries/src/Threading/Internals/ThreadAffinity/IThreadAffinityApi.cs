namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public interface IThreadAffinityApi
{
    ulong GetCurrentThreadId();
    void AssignAffinity(ulong nativeThreadId, int core);
    void ResetAffinityAndCleanup(ulong nativeThreadId);
}