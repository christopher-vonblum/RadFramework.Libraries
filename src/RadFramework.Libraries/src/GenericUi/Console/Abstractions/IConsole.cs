namespace RadFramework.Libraries.GenericUi.Console.Abstractions
{
    public interface IConsole
    {
        string ReadLine();
        void WriteLine(string value);
    }
}