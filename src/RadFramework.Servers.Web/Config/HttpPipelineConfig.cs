namespace RadFramework.Servers.Web.Config;

public interface HttpPipelineConfig
{
    IEnumerable<string> Pipes { get; set; }
}