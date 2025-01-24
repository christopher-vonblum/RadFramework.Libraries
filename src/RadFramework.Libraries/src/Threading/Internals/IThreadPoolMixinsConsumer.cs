using RadFramework.Libraries.Collections;

namespace RadFramework.Libraries.Threading.Internals
{
    public interface IThreadPoolMixinsConsumer
    {
        Action<Thread, Exception> OnError { get; }
        ThreadPriority ProcessingThreadPriority { get; }
        string ThreadDescription { get; }
        ObjectReferenceRegistry<PoolThread> ProcessingThreadRegistry { get; }
    }
}