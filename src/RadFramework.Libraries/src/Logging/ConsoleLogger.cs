namespace RadFramework.Libraries.Logging;

public class ConsoleLogger : ILoggerSink
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}