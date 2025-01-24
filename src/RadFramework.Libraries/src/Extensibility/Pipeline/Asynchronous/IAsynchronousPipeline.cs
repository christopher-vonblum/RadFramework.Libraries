namespace RadFramework.Libraries.Extensibility.Pipeline.Asynchronous
{
    public class IAsynchronousPipeline<TIn, TOut> : IPipeline<TIn, TOut>
    {
        private readonly IServiceProvider _serviceProvider;
        public LinkedList<IAsynchronousPipe> definitions;
        
        public IAsynchronousPipeline(PipelineDefinition definition, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            definitions = new LinkedList<IAsynchronousPipe>(definition.Definitions.Select(CreatePipe));
        }
        private IAsynchronousPipe CreatePipe(PipeDefinition def)
        {
            return (IAsynchronousPipe) _serviceProvider.GetService(def.Type);
        }

        public TOut Process(TIn input)
        {
            AsynchronousPipeContext pipelineContext = new AsynchronousPipeContext();
            
            List<AsynchronousPipeContext> contexts = new List<AsynchronousPipeContext>();
            
            Thread first = new Thread(() =>
            {
                AsynchronousPipeContext p = new AsynchronousPipeContext();
                if (definitions.First.Next != null)
                {
                    CreateThread(p, definitions.First.Next, pipelineContext, contexts);
                }
                else
                {
                    definitions.First.Value.Process(() => input,
                        o =>  { Return(pipelineContext, o); },
                        o => { Return(pipelineContext, o); });
                    return;
                }
                
                definitions.First.Value.Process(() => input,
                    o =>{ Return(p, o); },
                    o => { Return(pipelineContext, o); });
            });
            first.Start();

            pipelineContext.PreviousReturnedValue.WaitOne();
            
            foreach (var pipeContext in contexts.Where(t => t.WaitingForInput))
            {
                pipeContext.Thread.Abort();
            }
            
            return (TOut)pipelineContext.ReturnValue;
        }

        private static void Return(AsynchronousPipeContext pipelineContext, object o)
        {
            pipelineContext.ReturnValue = o;
            pipelineContext.PreviousReturnedValue.Set();
        }

        private void CreateThread(AsynchronousPipeContext previousContext, LinkedListNode<IAsynchronousPipe> current,
            AsynchronousPipeContext pipelineContext, List<AsynchronousPipeContext> contexts)
        {
            Thread pipeThread = new Thread(() =>
                {
                    AsynchronousPipeContext currentContext = new AsynchronousPipeContext();
                    
                    currentContext.Thread = Thread.CurrentThread;
                    
                    contexts.Add(currentContext);
                    
                    if (current.Next != null)
                    {
                        CreateThread(currentContext, current.Next, pipelineContext, contexts);
                    }
                    else
                    {
                        current.Value.Process(() =>
                            {
                                previousContext.PreviousReturnedValue.WaitOne();
                                currentContext.WaitingForInput = false;
                                return previousContext.ReturnValue;
                            },
                            o => { Return(pipelineContext, o); },
                            o => { Return(pipelineContext, o); });
                        return;
                    }

                    current.Value.Process(() =>
                        {
                            previousContext.PreviousReturnedValue.WaitOne();
                            currentContext.WaitingForInput = false;
                            return previousContext.ReturnValue;
                        },
                        o =>{ Return(currentContext, o); },
                        o => { Return(pipelineContext, o); });
                });
            
            pipeThread.Start();
        }
    }
}