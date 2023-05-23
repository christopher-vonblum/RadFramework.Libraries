using System;
using System.Net;
using System.Net.Sockets;
using RadFramework.Libraries.Telemetry;
using RadFramework.Libraries.Telemetry.Encryption;

namespace Tests
{
    class TelemetryClient
    {
        private readonly IPEndPoint _endPoint;
        public ITelemetryChannel Channel { get; private set; }
        private ResourcePool channelResources = new ResourcePool();
        private ProcessTelemetryRequest _processTelemetryRequest;
        private ProcessTelemetryEvent _processTelemetryEvent;
        private IContractSerializer _contractSerializer;
        private readonly ITelemetryCryptoProvider _cryptoProvider;

        public TelemetryClient(IPEndPoint endPoint,
            ProcessTelemetryRequest processTelemetryRequest,
            ProcessTelemetryEvent processTelemetryEvent,
            IContractSerializer contractSerializer,
            ITelemetryCryptoProvider cryptoProvider)
        {
            _processTelemetryRequest = processTelemetryRequest;
            _processTelemetryEvent = processTelemetryEvent;
            _contractSerializer = contractSerializer;
            _cryptoProvider = cryptoProvider;
            _endPoint = endPoint;
            
            Channel = new MultiplexTelemetryChannel(
                contractSerializer,
                processTelemetryRequest,
                processTelemetryEvent,
                new MultiplexInputSource(channelResources.GetResource, cryptoProvider),
                new MultiplexOutputSink(channelResources.GetResource, cryptoProvider));
            
            Connect();
        }

        void Connect()
        {
            Guid clientId = Guid.NewGuid();
            
            Socket introduceConnection = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            introduceConnection.Connect(_endPoint);
            
            var initStream = new NetworkStream(introduceConnection);
            var wrapper = new TelemetryPackageWrapper(_contractSerializer);
            BytePackageUtil.WritePackage(initStream, 
                _contractSerializer.Serialize(typeof(PayloadPackage), 
                    wrapper.Wrap(PackageKind.Request, new ConnectionRequest
            {
                ConnectionId = clientId,
                ResourceCount = Environment.ProcessorCount
            })));
            
            channelResources.RegisterResource(initStream);
            var resourceCount = Environment.ProcessorCount * 2;
            for (int i = 1; i < resourceCount; i++)
            {
                Socket clientSocket = new Socket(_endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.Connect(_endPoint);
                var stream = new NetworkStream(clientSocket);
                
                BytePackageUtil.WritePackage(stream, 
                    _contractSerializer.Serialize(typeof(PayloadPackage), 
                        wrapper.Wrap(PackageKind.Request, new ResourceConnectionRequest()
                        {
                            ConnectionId = clientId
                        })));
                
                channelResources.RegisterResource(stream);
            }
        }
        
    }
}