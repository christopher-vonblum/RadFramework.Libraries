using System.Collections.Concurrent;
using System.Threading;
using RadFramework.Libraries.Telemetry.Encryption;
using Tests;

namespace RadFramework.Libraries.Telemetry
{
    public class SequentialOutputSink : ITelemetryStreamConnectionSink
    {
        private readonly GetOutputStream _resourceFactory;
        private readonly ITelemetryCryptoProvider _cryptoProvider;
        private Thread writerThread;
        
        private ConcurrentQueue<byte[]> outgoingPackageQueue = new ConcurrentQueue<byte[]>();
        
        private AutoResetEvent flushEvent = new AutoResetEvent(false);
        
        public SequentialOutputSink(GetOutputStream resourceFactory, ITelemetryCryptoProvider cryptoProvider)
        {
            _resourceFactory = resourceFactory;
            _cryptoProvider = cryptoProvider;
            writerThread = new Thread(ThreadBody);
            writerThread.Name = "SequentialOutputSink";
            writerThread.Priority = ThreadPriority.Highest;
            writerThread.Start();
        }
        
        private void ThreadBody()
        {
            while (!Disposed)
            {
                byte[] package;

                while (this.outgoingPackageQueue.TryDequeue(out package))
                {
                    BytePackageUtil.WritePackage(_resourceFactory(), _cryptoProvider.Encrypt(package));
                }
                
                this.flushEvent.WaitOne(5000);
            }
        }

        private bool Disposed { get; set; }

        public void Dispose()
        {
            Disposed = true;
            writerThread.Join();
        }

        public void SendPackage(byte[] package)
        {
            outgoingPackageQueue.Enqueue(package);
            flushEvent.Set();
        }
    }
}