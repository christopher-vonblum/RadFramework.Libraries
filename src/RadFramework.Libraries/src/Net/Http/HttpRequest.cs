using System.Text;

namespace RadFramework.Libraries.Net.Http;

public class HttpRequest
{
    public string Method { get; set; }
    public string HttpVersion { get; set; }
    public string Url { get; set; }
    public string UrlPath { get; set; }
    public string QueryString { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
    
    public Encoding Encoding { get; set; }

}