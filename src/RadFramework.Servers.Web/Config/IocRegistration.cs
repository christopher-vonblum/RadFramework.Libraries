namespace RadFramework.Servers.Web.Config;

public interface IocRegistration
{
    bool Singleton { get; set; }
    string TKey { get; set; }
    string TImplementation { get; set; }
}