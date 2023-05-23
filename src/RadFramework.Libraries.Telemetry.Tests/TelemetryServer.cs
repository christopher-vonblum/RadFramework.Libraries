using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RadFramework.Libraries.Telemetry;
using RadFramework.Libraries.Telemetry.Encryption;
using RadFramework.Libraries.Threading;

namespace Tests
{
    public class TelemetryServer
    {
        private readonly ProcessTelemetryRequest _processTelemetryRequest;
        private readonly ProcessTelemetryEvent _processTelemetryEvent;
        private readonly IContractSerializer _contractSerializer;
        private readonly ITelemetryPackageWrapper _packageWrapper;
        private ThreadPool incomingConnectionListener;
        private Socket listener;
        private ConcurrentDictionary<Guid, TelemetryChannelBase> channels = new ConcurrentDictionary<Guid, TelemetryChannelBase>();
        private ConcurrentDictionary<Guid, ResourcePool> resourcePools = new ConcurrentDictionary<Guid, ResourcePool>();
        
        public TelemetryServer(IPEndPoint listenerEndpoint,
            ProcessTelemetryRequest processTelemetryRequest,
            ProcessTelemetryEvent processTelemetryEvent,
            IContractSerializer contractSerializer)
        {
            _processTelemetryRequest = processTelemetryRequest;
            _processTelemetryEvent = processTelemetryEvent;
            _contractSerializer = contractSerializer;
            _packageWrapper = new TelemetryPackageWrapper(contractSerializer);

            listener = new Socket(listenerEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(listenerEndpoint);
            listener.Listen(Environment.ProcessorCount*2);
            
            incomingConnectionListener = new MultiThreadProcessor(ThreadPriority.Highest, HandleSocketConnection);
        }
        
        private void HandleSocketConnection()
        {
            var socket = listener.Accept();
            var stream = new NetworkStream(socket);
            byte[] package = BytePackageUtil.ReadPackage(stream);
            var unwrapped = _packageWrapper.Unwrap(package);
            if (unwrapped.payload is ConnectionRequest connectionRequest)
            {
                var pool = resourcePools[connectionRequest.ConnectionId] = new ResourcePool();
                channels[connectionRequest.ConnectionId] = new MultiplexTelemetryChannel(
                    _contractSerializer, 
                    _processTelemetryRequest, 
                    _processTelemetryEvent,
                    new MultiplexInputSource(pool.GetResource, new NoCryptoProvider()), 
                    new MultiplexOutputSink(pool.GetResource, new NoCryptoProvider()));
                
                pool.RegisterResource(stream);
            }
            else if (unwrapped.payload is ResourceConnectionRequest clientResource)
            {
                var pool = resourcePools[clientResource.ConnectionId];
                pool.RegisterResource(stream);
            }
        }
    }
}