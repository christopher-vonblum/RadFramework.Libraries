namespace RadFramework.Libraries.Extensibility.Pipeline.Extension;

public interface IExtensionPipe<TContext>
{
    void Process(TContext context, ExtensionPipeContext pipeContext);
}