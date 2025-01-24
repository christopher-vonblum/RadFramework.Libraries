using RadFramework.Libraries.Utils;

namespace RadFramework.Libraries.Logging;

public class FileLogger : ILoggerSink
{
    private string logFilePath;
    
    public FileLogger(string logPath)
    {
        if (!Directory.Exists(logPath))
        {
            Directory.CreateDirectory(logPath);
        }
        
        logFilePath = logPath.EnsureSuffix("/") + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".log";
    }
    
    public void Log(string message)
    {
        File.AppendAllText(logFilePath, message + Environment.NewLine);
    }
}