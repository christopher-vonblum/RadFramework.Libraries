using RadFramework.Libraries.Caching;

namespace RadFramework.Libraries.Logging;

public interface ILogger
{
    void Log(string message);
    void LogWarning(string message);
    void LogError(string message);
}