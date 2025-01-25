namespace RadFramework.Libraries.Core.Patterns.DisposableContext
{
    public abstract class ContextBoundBase : IExecutionContextBound
    {
        public Action<IExecutionContextBound> OnDispose { get; set; }
        
        public virtual void Dispose()
        {
            OnDispose?.Invoke(this);
        }
    }
}