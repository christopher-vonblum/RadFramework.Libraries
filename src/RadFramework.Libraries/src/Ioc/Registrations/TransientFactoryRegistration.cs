namespace RadFramework.Libraries.Ioc.Registrations
{
    public class TransientFactoryRegistration : RegistrationBase
    {
        private readonly Func<Container, object> factoryFunc;
        private readonly Container container;

        public TransientFactoryRegistration(Func<Container, object> factoryFunc, Container container)
        {
            this.factoryFunc = factoryFunc;
            this.container = container;
        }
        
        public override object ResolveService()
        {
            return factoryFunc(container);
        }
    }
}