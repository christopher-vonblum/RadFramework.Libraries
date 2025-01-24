namespace RadFramework.Libraries.Net.Http;

public struct HttpRequest
{
    public HttpRequest()
    {
    }
    
    public string Method { get; set; }
    public string HttpVersion { get; set; }
    public string Url { get; set; }
    public string UrlPath { get; set; }
    public string QueryString { get; set; }
    public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

}