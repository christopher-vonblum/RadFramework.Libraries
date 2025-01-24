namespace RadFramework.Libraries.Ioc.Registrations
{
    public class SingletonFactoryRegistration : TransientFactoryRegistration
    {
        private Lazy<object> singleton;

        public SingletonFactoryRegistration(Func<IocContainer, object> factoryFunc, IocContainer iocContainer) : base(factoryFunc, iocContainer)
        {
            singleton = new Lazy<object>(() => base.ResolveService());
        }

        public override object ResolveService()
        {
            return singleton.Value;
        }
    }
}