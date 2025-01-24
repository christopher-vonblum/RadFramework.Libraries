namespace RadFramework.Servers.Web.Config;

public interface IocContainerConfig
{
    IEnumerable<IocRegistration> Registrations { get; set; }
}