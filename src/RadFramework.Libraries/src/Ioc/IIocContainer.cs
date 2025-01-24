using RadFramework.Libraries.Ioc.Factory;

namespace RadFramework.Libraries.Ioc
{
    public interface IIocContainer : IServiceProvider
    {
        IEnumerable<(Type serviceType, Func<object> resolve)> Services { get; }

        object Resolve(Type t);

        public T Activate<T>(InjectionOptions injectionOptions = null);

        public object Activate(Type t, InjectionOptions injectionOptions = null);
    }
}