using System.Text;

namespace RadFramework.Libraries.Net.Http;

public class HttpDocument
{
    public Encoding Encoding { get; set; }
    public byte[] Body { get; set; }

    public string GetText() => Encoding.GetString(Body);
}