namespace RadFramework.Libraries.Ioc
{
    public interface IContainer : IServiceProvider
    {
        IEnumerable<(Type serviceType, Func<object> resolve)> Services { get; }

        object Resolve(Type t);
    }
}