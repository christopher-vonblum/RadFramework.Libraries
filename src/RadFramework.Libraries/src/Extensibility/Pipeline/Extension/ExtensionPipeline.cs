using RadFramework.Libraries.Ioc;

namespace RadFramework.Libraries.Extensibility.Pipeline.Extension;

public class ExtensionPipeline<TContext>
{
    private readonly IIocContainer _serviceProvider;
    public LinkedList<IExtensionPipe<TContext>> pipes;

    public ExtensionPipeline(PipelineDefinition definition, IIocContainer serviceProvider)
    {
        _serviceProvider = serviceProvider;
        this.pipes = new LinkedList<IExtensionPipe<TContext>>(definition.Definitions.Select(CreatePipe));
    }
        
    public ExtensionPipeline(IEnumerable<IExtensionPipe<TContext>> pipes)
    {
        this.pipes = new LinkedList<IExtensionPipe<TContext>>(pipes);
    }

    private IExtensionPipe<TContext> CreatePipe(PipeDefinition def)
    {
        return (IExtensionPipe<TContext>) _serviceProvider.Activate(def.Type);
    }

    public bool Process(TContext input)
    {
        ExtensionPipeContext pipeContext = new ExtensionPipeContext();
        
        foreach (var pipe in pipes)
        {
            pipe.Process(input, pipeContext);

            if (pipeContext.ShouldReturn)
            {
                break;
            }
        }

        return pipeContext.ShouldReturn;
    }
}