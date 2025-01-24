namespace RadFramework.Libraries.Extensibility.Pipeline;

public interface IExtensionPipe<TContext>
{
    void Process(TContext context);
}