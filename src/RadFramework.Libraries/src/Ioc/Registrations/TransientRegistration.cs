using System;
using System.Collections.Concurrent;
using RadFramework.Libraries.Ioc.Factory;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public class TransientRegistration : RegistrationBase
    {
       
        private readonly Container container;

        private readonly Lazy<Func<Container, object>> construct;
        
        private static ConcurrentDictionary<(InjectionOptions o, Type t), Func<Container, object>> factoryCache = new ConcurrentDictionary<(InjectionOptions o, Type t), Func<Container, object>>();
        
        public TransientRegistration(CachedType tImplementation,
            ServiceFactoryLambdaGenerator lambdaGenerator, Container container)
        {
            this.container = container;

            this.construct = new Lazy<Func<Container, object>>(
                () => 
                    factoryCache.GetOrAdd((InjectionOptions, tImplementation),
                        tuple => lambdaGenerator.CreateInstanceFactory(tImplementation, container.injectionOptions, InjectionOptions)));
        }

        public override object ResolveService()
        {
            return construct.Value(container);
        }
    }
}