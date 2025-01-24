namespace RadFramework.Libraries.Extensibility.Pipeline.Extension;

public class ExtensionPipeContext
{
    public bool ShouldReturn { get; set; }

    public void Return()
    {
        ShouldReturn = true;
    }
}