using System.Collections.Concurrent;
using System.Net.Sockets;

namespace RadFramework.Libraries.Net.Http;

public class HttpSocketQueue : IDisposable
{
    private readonly Action<Socket> processRequest;
    private ConcurrentQueue<Socket> queue = new ConcurrentQueue<Socket>();

    private const int concurrentSocketProcessingThreadsLimit = 100;

    private volatile int currentSocketProcessingThreadsAmount = 0;

    private Thread queueProcessingLauncherThread;

    private AutoResetEvent socketGotQueued = new AutoResetEvent(false);
    
    private bool disposed = false;
    
    public HttpSocketQueue(Action<Socket> processRequest)
    {
        this.processRequest = processRequest;

        queueProcessingLauncherThread = new Thread(LaunchProcessingThreadLoop);
        queueProcessingLauncherThread.Start();
    }
        
    public void Enqueue(Socket clientConnectionSocket)
    {  
        queue.Enqueue(clientConnectionSocket);
        socketGotQueued.Set();
    }

    private void LaunchProcessingThreadLoop()
    {
        while (!disposed)
        {
            socketGotQueued.WaitOne();

            TrySpawnProcessingThreads();
        }
    }

    private void TrySpawnProcessingThreads()
    {
        while (currentSocketProcessingThreadsAmount < concurrentSocketProcessingThreadsLimit
            && queue.TryDequeue(out Socket clientConnectionSocket))
        {
            StartProcessingThread(clientConnectionSocket);
        }
    }

    private void StartProcessingThread(Socket clientConnectionSocket)
    {
        Thread processingThread = new Thread(() =>
        {
            processRequest(clientConnectionSocket);
            
            currentSocketProcessingThreadsAmount--;
        });
                
        processingThread.Start();
                
        currentSocketProcessingThreadsAmount++;
    }

    public bool CanShutdown
    {
        get
        {
            return queue.IsEmpty 
                   && currentSocketProcessingThreadsAmount == 0;
        }
    }

    public void Dispose()
    {
        disposed = true;
        queueProcessingLauncherThread.Join();
    }
}