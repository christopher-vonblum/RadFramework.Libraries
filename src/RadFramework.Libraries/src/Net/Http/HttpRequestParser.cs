namespace RadFramework.Libraries.Net.Http;

public static class HttpRequestParser
{
    public static string ExtractHttpMethod(string firstLine)
    {
        return ReadUntilChar(firstLine, ' ');
    }
    
    public static string ExtractUrl(string firstLine)
    {
        string urlString = "";

        int firstSpaceIndex = firstLine.IndexOf(' ') + 1;
        int lastSpaceIndex = firstLine.LastIndexOf(' ');
        
        for (int i = firstSpaceIndex; i < lastSpaceIndex; i++)
        {
            urlString += firstLine[i];
        }

        return urlString;
    }
    
    public static string ExtractQueryString(string urlString)
    {
        int firstQuestionmarkIndex = urlString.IndexOf('?');

        if (firstQuestionmarkIndex == -1)
        {
            return string.Empty;
        }
        
        return urlString.Substring(firstQuestionmarkIndex);
    }
    
    public static string ExtractHttpVersion(string firstLine)
    {
        string versionString = "";

        IEnumerable<char> firstLineReverse = firstLine.Reverse();
        
        foreach (char character in firstLineReverse)
        {
            if (character == ' ')
                break;
            
            versionString += character;
        }

        versionString = string.Concat(versionString.Reverse());

        return versionString;
    }

    public static (string header, string value) ReadHeader(string headerLine)
    {
        string header = ReadUntilChar(headerLine, ':');
        string value = headerLine
                        .Substring(header.Length + 1)
                        .TrimStart();

        return (header, value);
    }

    private static string ReadUntilChar(string str, char stopChar)
    {
        string extractedString = "";
        
        foreach (char character in str)
        {
            if (character == stopChar)
                break;
            
            extractedString += character;
        }

        return extractedString;
    }
}