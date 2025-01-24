namespace RadFramework.Libraries.Extensibility.Pipeline.Asynchronous;

public class AsynchronousPipeContext
{
    public ManualResetEvent PreviousReturnedValue { get; } = new ManualResetEvent(false);
    public object ReturnValue { get; set; }
    public bool WaitingForInput { get; set; } = true;
    public Thread Thread { get; set; }
}