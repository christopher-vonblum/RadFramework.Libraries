namespace RadFramework.Libraries.Extensibility.Pipeline.Asynchronous
{
    public interface IAsynchronousPipe
    {
        void Process(Func<object> input, Action<object> @continue, Action<object> @return);
    }
}