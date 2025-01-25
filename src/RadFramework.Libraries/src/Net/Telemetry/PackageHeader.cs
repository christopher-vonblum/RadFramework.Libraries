namespace RadFramework.Libraries.Net.Socket;

public class PackageHeader
{
    public string PayloadSerializerType { get; set; }
    public string PayloadType { get; set; }
    public byte[] ResponseToken { get; set; }
    public ulong PayloadSize { get; set; }
}