namespace RadFramework.Libraries.Ioc.Registrations
{
    public class SingletonInstanceRegistration : RegistrationBase
    {
        private object instance;

        public SingletonInstanceRegistration(object instance)
        {
            this.instance = instance;
        }
        
        public override object ResolveService()
        {
            return instance;
        }
    }
}