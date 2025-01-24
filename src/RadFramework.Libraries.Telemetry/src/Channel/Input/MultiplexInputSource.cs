using System;
using System.Threading;
using RadFramework.Libraries.Telemetry.Encryption;
using Tests;

namespace RadFramework.Libraries.Telemetry
{
    
    
    /// <summary>
    /// Reads input from a connection consisting of multiple transmission resources. (file streams, network streams, named pipes)
    /// </summary>
    public class MultiplexInputSource : ITelemetryStreamConnectionSource
    {
        public Action<byte[]> OnPackageReceived { get; set; }
        
        private readonly GetInputStream _endpointConnection;
        private readonly ITelemetryCryptoProvider _cryptoProvider;
        private QueuedMultiThreadProcessorWithDispatchCapabilities<byte[]> packageProcessor;
        private MultiThreadProcessor packageReader;
        private bool disposed;

        public MultiplexInputSource(GetInputStream endpointConnection, ITelemetryCryptoProvider cryptoProvider)
        {
            _endpointConnection = endpointConnection;
            _cryptoProvider = cryptoProvider;

            packageProcessor = new QueuedMultiThreadProcessorWithDispatchCapabilities<byte[]>(
                Environment.ProcessorCount,
                250,
                ThreadPriority.Highest,
                (package) => OnPackageReceived?.Invoke(package),
                dispatchLimit : Environment.ProcessorCount * 2,
                threadDescription : "MultiplexInputSource_Read");
            
            packageReader = new MultiThreadProcessor(ThreadPriority.Highest, ReadPackages);
        }
        
        private void ReadPackages()
        {
            while (!disposed)
            {
                try
                {
                    byte[] package = BytePackageUtil.ReadPackage(_endpointConnection());

                    package = _cryptoProvider.Decrypt(package);
                    
                    if (package == null)
                    {
                        OnReadError();
                        return;
                    }

                    this.packageProcessor.Enqueue(package);
                }
                catch (Exception)
                {
                    OnReadError();
                }
            }
        }

        private void OnReadError()
        {
        }

        public void Dispose()
        {
            packageReader.Dispose();
            packageProcessor.Dispose();
            disposed = true;
        }
    }
}