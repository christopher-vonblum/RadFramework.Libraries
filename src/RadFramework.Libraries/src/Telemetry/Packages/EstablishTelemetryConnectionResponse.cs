namespace RadFramework.Libraries.Telemetry.Packages;

public class EstablishTelemetryConnectionResponse
{
    public Guid ClientId { get; set; }
    public List<int> Ports { get; set; }
}