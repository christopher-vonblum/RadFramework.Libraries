namespace RadFramework.Libraries.Net.Http;

public struct HttpRequest
{
    public HttpRequest()
    {
        Method = null;
        HttpVersion = null;
        Url = null;
    }

    public string Method { get; set; }
    public string HttpVersion { get; set; }
    public string Url { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
}