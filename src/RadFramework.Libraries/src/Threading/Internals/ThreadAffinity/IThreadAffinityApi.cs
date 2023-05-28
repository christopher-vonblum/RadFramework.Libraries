namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public interface IThreadAffinityApi
{
    int GetNativeThreadId();
    void AssignAffinity(int nativeThreadId, BitMask processorMask);
    void AssignAllProcessors(int nativeThreadId);
}