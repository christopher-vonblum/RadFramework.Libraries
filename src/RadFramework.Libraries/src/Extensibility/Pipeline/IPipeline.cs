namespace RadFramework.Libraries.Extensibility.Pipeline
{
    public interface IPipeline<TIn, TOut>
    {
        TOut Process(TIn input);
    }
}