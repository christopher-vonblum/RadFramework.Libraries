using RadFramework.Libraries.Ioc.Factory;

namespace RadFramework.Libraries.Ioc.Registrations
{
    public abstract class RegistrationBase
    {
        public InjectionOptions InjectionOptions { get; set; }
        public abstract object ResolveService();
    }
}