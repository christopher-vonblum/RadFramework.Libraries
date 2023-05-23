using System;
using System.Threading;
using RadFramework.Libraries.Telemetry.Encryption;
using Tests;

namespace RadFramework.Libraries.Telemetry
{
    public class SequentialInputSource : ITelemetryStreamConnectionSource
    {
        private readonly GetInputStream _resourceFactory;
        private readonly ITelemetryCryptoProvider _cryptoProvider;
        public Action<byte[]> OnPackageReceived { get; set; }

        private Thread readerThread;
        private bool _disposed;

        public SequentialInputSource(GetInputStream resourceFactory, ITelemetryCryptoProvider cryptoProvider)
        {
            _resourceFactory = resourceFactory;
            _cryptoProvider = cryptoProvider;
            readerThread = new Thread(ThreadBody);
            readerThread.Name = "SequentialInputSource";
            readerThread.Start();
        }
        
        private void ThreadBody()
        {
            while (!_disposed)
            {
                if (OnPackageReceived == null)
                {
                    Thread.Sleep(500);
                    continue;
                }
                
                OnPackageReceived?.Invoke(_cryptoProvider.Decrypt(BytePackageUtil.ReadPackage(_resourceFactory())));
            }
        }

        public void Dispose()
        {
            _disposed = true;
            readerThread.Join();
        }
    }
}