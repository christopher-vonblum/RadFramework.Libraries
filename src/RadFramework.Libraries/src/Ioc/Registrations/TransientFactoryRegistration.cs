namespace RadFramework.Libraries.Ioc.Registrations
{
    public class TransientFactoryRegistration : RegistrationBase
    {
        private readonly Func<IocContainer, object> factoryFunc;
        private readonly IocContainer iocContainer;

        public TransientFactoryRegistration(Func<IocContainer, object> factoryFunc, IocContainer iocContainer)
        {
            this.factoryFunc = factoryFunc;
            this.iocContainer = iocContainer;
        }
        
        public override object ResolveService()
        {
            return factoryFunc(iocContainer);
        }
    }
}