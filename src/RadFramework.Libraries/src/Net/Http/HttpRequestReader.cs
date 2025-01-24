using System.Text;

namespace RadFramework.Libraries.Net.Http;

public class HttpRequestReader
{
    static readonly byte CarriageReturn = Encoding.ASCII.GetBytes("\r")[0];
    static readonly byte NewLine = Encoding.ASCII.GetBytes("\n")[0];
    
    /*public static string ReadRequestLine(NetworkStream stream)
    {
        byte[] buffer = ReceiveBuffer(socketConnection);
        
        string line = "";

        byte current = stream.;
        byte next = 0;

        int i = requestByteIndex;
        
        while (!(current == CarriageReturn && next == NewLine))
        {
            
        }
        
        
        
    }

    private static byte[] ReceiveBuffer(Socket socketConnection)
    {
        byte[] buffer = new byte[1024];

        socketConnection.Receive(buffer);

        return buffer;
    }*/
}