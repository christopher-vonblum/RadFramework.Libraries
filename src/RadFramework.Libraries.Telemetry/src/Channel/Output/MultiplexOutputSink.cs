using System;
using System.Threading;
using RadFramework.Libraries.Telemetry.Encryption;
using RadFramework.Libraries.Threading;
using Tests;

namespace RadFramework.Libraries.Telemetry
{
    /// <summary>
    /// Writes input to a connection consisting of multiple transmission resources. (file streams, network streams, named pipes)
    /// Packages that can not be written to a resource get queued in memory.
    /// </summary>
    public class MultiplexOutputSink : ITelemetryStreamConnectionSink
    {
        private readonly GetOutputStream _streamFactory;
        private readonly ITelemetryCryptoProvider _cryptoProvider;
        private QueuedMultiThreadProcessor<byte[]> processor;
        public MultiplexOutputSink(GetOutputStream streamFactory, ITelemetryCryptoProvider cryptoProvider)
        {
            _streamFactory = streamFactory;
            _cryptoProvider = cryptoProvider;
            processor = new QueuedMultiThreadProcessorWithDispatchCapabilities<byte[]>(
                // worker thread count 
                250,
                
                ThreadPriority.Highest,
                Write,
                // worker thread dispatch slots count
                dispatchLimit: Environment.ProcessorCount * 2,
                threadDescription:"MultiplexOutputSink_Write");
        }

        private void Write(byte[] package)
        {
            BytePackageUtil.WritePackage(_streamFactory(), _cryptoProvider.Encrypt(package));
        }

        public void Dispose()
        {
            processor?.Dispose();
        }

        public void SendPackage(byte[] package)
        {
            processor.Enqueue(package);
        }
    }
}