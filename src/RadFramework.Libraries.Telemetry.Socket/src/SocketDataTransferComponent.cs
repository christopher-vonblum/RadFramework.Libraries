using System;
using System.Net.Sockets;
using System.Threading;

namespace RadFramework.Libraries.Telemetry.Socket;

public class SocketDataTransferComponent : ISequentialDataTransferComponent
{
    private System.Net.Sockets.Socket _socket;
    private readonly Func<System.Net.Sockets.Socket> _connect;
    private readonly object sequentialDataTranferLock = new object();

    public SocketDataTransferComponent(Func<System.Net.Sockets.Socket> connect)
    {
        _socket = connect();
        _connect = connect;
    }
    
    public void Send(byte[] data)
    {
        Monitor.Enter(sequentialDataTranferLock);
send:
        try
        {
            _socket.Send(BitConverter.GetBytes(data.Length));
            _socket.Send(data);
        }
        catch(SocketException)
        {
            Reconnect();
            goto send;
        }
        
        Monitor.Exit(sequentialDataTranferLock);
    }

    public byte[] Receive()
    {
        Monitor.Enter(sequentialDataTranferLock);
        
receive:
        try
        {
            byte[] sizeBuffer = new byte[sizeof(int)];
            _socket.Receive(sizeBuffer);

            byte[] data = new byte[BitConverter.ToInt32(sizeBuffer)];
            _socket.Receive(data);
            
            Monitor.Exit(sequentialDataTranferLock);
            
            return data;
        }
        catch(SocketException)
        {
            Reconnect();
            goto receive;
        }
    }

    private void Reconnect()
    {
        _socket.Dispose();
        _socket = _connect();
    }

    public void Dispose()
    {
        _socket?.Dispose();
    }
}