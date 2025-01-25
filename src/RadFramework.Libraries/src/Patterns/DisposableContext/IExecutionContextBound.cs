namespace RadFramework.Libraries.Core.Patterns.DisposableContext
{
    public interface IExecutionContextBound : IDisposable
    {
        Action<IExecutionContextBound> OnDispose { get; set; }
    }
}