using System;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public class SingletonFactoryRegistration : TransientFactoryRegistration
    {
        private Lazy<object> singleton;

        public SingletonFactoryRegistration(Func<Container, object> factoryFunc, Container container) : base(factoryFunc, container)
        {
            singleton = new Lazy<object>(() => base.ResolveService());
        }

        public override object ResolveService()
        {
            return singleton.Value;
        }
    }
}