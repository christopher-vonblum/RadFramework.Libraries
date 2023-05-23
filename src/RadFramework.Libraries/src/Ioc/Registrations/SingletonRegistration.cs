using System;
using RadFramework.Libraries.Ioc.Factory;
using RadFramework.Libraries.Reflection.Caching;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public class SingletonRegistration : TransientRegistration
    {
        private Lazy<object> singleton;
        public SingletonRegistration(CachedType tImplementation, ServiceFactoryLambdaGenerator lambdaGenerator, Container container) : base(tImplementation, lambdaGenerator, container)
        {
            singleton = new Lazy<object>(() => base.ResolveService());
        }
        
        public override object ResolveService()
        {
            return singleton.Value;
        }
    }
}