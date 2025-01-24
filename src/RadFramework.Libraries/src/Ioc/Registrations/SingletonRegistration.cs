using RadFramework.Libraries.Ioc.Factory;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public class SingletonRegistration : TransientRegistration
    {
        private Lazy<object> singleton;
        public SingletonRegistration(CachedType tImplementation, ServiceFactoryLambdaGenerator lambdaGenerator, IocContainer iocContainer) : base(tImplementation, lambdaGenerator, iocContainer)
        {
            singleton = new Lazy<object>(() => base.ResolveService());
        }
        
        public override object ResolveService()
        {
            return singleton.Value;
        }
    }
}