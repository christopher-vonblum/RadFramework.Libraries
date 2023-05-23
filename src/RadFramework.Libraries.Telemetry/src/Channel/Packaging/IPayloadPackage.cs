namespace RadFramework.Libraries.Telemetry
{
    public interface IPayloadPackage
    {
        PackageKind PackageKind { get; }
        string PayloadType { get; }
        byte[] Payload { get; }
    }
}