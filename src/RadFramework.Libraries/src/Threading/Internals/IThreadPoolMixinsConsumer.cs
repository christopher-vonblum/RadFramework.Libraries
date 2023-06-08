using System;
using System.Threading;
using RadFramework.Libraries.Threading.ObjectRegistries;

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