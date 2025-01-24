namespace RadFramework.Libraries.Logging;

public class StandardLogger : ILogger
{
    private readonly List<ILoggerSink> loggers;

    public StandardLogger(IEnumerable<ILoggerSink> loggers)
    {
        this.loggers = loggers.ToList();
    }

    public void Log(string message)
    {
        loggers.ForEach(l => l.Log("Log: " + message));
    }

    public void LogWarning(string message)
    {
        loggers.ForEach(l => l.Log("Warning: " + message));
    }

    public void LogError(string message)
    {
        loggers.ForEach(l => l.Log("Error: " + message));
    }
}