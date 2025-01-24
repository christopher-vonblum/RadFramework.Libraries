using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;

namespace RadFramework.Libraries.Extensibility.Pipeline;

public class ExtensionPipeline<TContext> : IPipeline<TContext, TContext>
{
    private readonly IIocContainer _serviceProvider;
    public LinkedList<ISynchronousPipe> pipes;

    public ExtensionPipeline(PipelineDefinition definition, IIocContainer serviceProvider)
    {
        _serviceProvider = serviceProvider;
        this.pipes = new LinkedList<ISynchronousPipe>(definition.Definitions.Select(CreatePipe));
    }
        
    public ExtensionPipeline(IEnumerable<ISynchronousPipe> pipes)
    {
        this.pipes = new LinkedList<ISynchronousPipe>(pipes);
    }

    private ISynchronousPipe CreatePipe(PipeDefinition def)
    {
        return (ISynchronousPipe) _serviceProvider.Activate(def.Type);
    }

    public TContext Process(TContext input)
    {
        foreach (var pipe in pipes)
        {
            pipe.Process(input);
        }

        return input;
    }
}