namespace RadFramework.Libraries.GenericUi.Console.Abstractions
{
    public class CommandLineProvider : IConsole
    {
        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void WriteLine(string value)
        {
            System.Console.WriteLine(value);
        }
    }
}