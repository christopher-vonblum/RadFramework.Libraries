using System.Collections.Concurrent;
using RadFramework.Libraries.Ioc.Factory;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public class TransientRegistration : RegistrationBase
    {
       
        private readonly IocContainer iocContainer;

        private readonly Lazy<Func<IocContainer, object>> construct;
        
        private static ConcurrentDictionary<(InjectionOptions o, Type t), Func<IocContainer, object>> factoryCache = new ConcurrentDictionary<(InjectionOptions o, Type t), Func<IocContainer, object>>();
        
        public TransientRegistration(CachedType tImplementation,
            ServiceFactoryLambdaGenerator lambdaGenerator, IocContainer iocContainer)
        {
            this.iocContainer = iocContainer;

            this.construct = new Lazy<Func<IocContainer, object>>(
                () => 
                    factoryCache.GetOrAdd((InjectionOptions, tImplementation),
                        tuple => lambdaGenerator.CreateInstanceFactory(tImplementation, iocContainer.injectionOptions, InjectionOptions)));
        }

        public override object ResolveService()
        {
            return construct.Value(iocContainer);
        }
    }
}