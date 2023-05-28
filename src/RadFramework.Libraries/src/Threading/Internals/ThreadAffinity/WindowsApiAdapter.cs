namespace RadFramework.Libraries.Threading.Internals.ThreadAffinity;

public class WindowsApiAdapter : IThreadAffinityApi
{
    public int GetNativeThreadId()
    {
        throw new NotImplementedException();
    }

    public void AssignAffinity(int nativeThreadId, BitMask processorMask)
    {
        throw new NotImplementedException();
    }

    public void AssignAllProcessors(int nativeThreadId)
    {
        throw new NotImplementedException();
    }
}