using System.Text;

namespace RadFramework.Libraries.Net.Http;

public class HttpResponse : IDisposable
{
    private readonly HttpConnection connection;

    private readonly StreamWriter writer;
    
    public HttpResponse(HttpConnection connection)
    {
        this.connection = connection;
        writer = new StreamWriter(connection.UnderlyingStream);
    }

    public void Send200(HttpDocument document)
    {
        writer.WriteLine(connection.Request.HttpVersion + " 200 OK");
        
        SendHtmlDocument(document);
        
        writer.Flush();
    }

    public void Send404(HttpDocument document = null)
    {
        writer.WriteLine(connection.Request.HttpVersion + " 404 Not Found");

        if (document != null)
        {
            SendHtmlDocument(document);
        }
        
        writer.Flush();
    }
    
    public void SendHtmlDocument(HttpDocument document)
    {
        SendHeader("Content-type", $"text/html; charset={document.Encoding.WebName}");
        SendHeader("Content-Length", document.Body.Length.ToString());
        writer.WriteLine();
        writer.Flush();
        
        SendFile(document.Body);
    }
    
    public void SendFile(byte[] file)
    {
        connection.UnderlyingStream.Flush();
        connection.UnderlyingStream.Write(file);
        connection.UnderlyingStream.Flush();
    }
    
    public void SendHeader(string headerName, string headerValue)
    {
        writer.WriteLine(headerName + ": " + headerValue);
    }

    public void TryServeStaticHtmlFile(string path, string notFoundPath = null)
    {
        if (!File.Exists(path))
        {
            if (notFoundPath == null || !File.Exists(notFoundPath))
            {
                Send404();
            }
            
            Send404(GetFileFromCacheOrDisk(notFoundPath));
        }
        
        connection.Response.Send200(
            GetFileFromCacheOrDisk(path));
    }

    public HttpDocument GetFileFromCacheOrDisk(string path)
    {
        return connection.ServerContext.Cache.GetOrSet(
            path, 
            () => 
                new HttpDocument
                {
                    Encoding = GetFileEncoding(path), 
                    Body = File.ReadAllBytes(path)
                });
    }

    private Encoding GetFileEncoding(string filePath)
    {
        // Read the BOM
        var bom = new byte[4];
        using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            file.Read(bom, 0, 4);
        }

        // Analyze the BOM
        if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
        if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
        if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0) return Encoding.UTF32; //UTF-32LE
        if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
        if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
        if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return new UTF32Encoding(true, true);  //UTF-32BE

        // We actually have no idea what the encoding is if we reach this point, so
        // you may wish to return null instead of defaulting to ASCII
        return Encoding.ASCII;
    }
    
    public void Dispose()
    {
        writer.Flush();
        writer.Close();
        writer.Dispose();
    }
}