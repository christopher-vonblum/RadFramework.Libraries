namespace RadFramework.Libraries.Core.Patterns.DisposableContext
{
    public static class ExecutionContextProvider<TContext>
        where TContext : class, IExecutionContextBound
    {
        public static TContext Current => GetCurrent();

        public static void Override(TContext context)
        {
            ContextOverrides.Push(context);   
        }


        [ThreadStatic] 
        private static Stack<TContext> contextOverrides;

        private static Stack<TContext> ContextOverrides => contextOverrides ?? (contextOverrides = new Stack<TContext>());

        private static TContext GetCurrent()
        {
            if (ContextOverrides.Count > 0)
            {
                TContext context = ContextOverrides.Peek();

                context.OnDispose = ContextOnDispose;

                return context;
            }

            throw new InvalidOperationException();
        }

        private static void ContextOnDispose(IExecutionContextBound executionContextBound)
        {
            if (ContextOverrides.Count > 0 && executionContextBound == ContextOverrides.Peek())
            {
                ContextOverrides.Pop();
                return;
            }

            throw new InvalidOperationException();
        }
    }
}
