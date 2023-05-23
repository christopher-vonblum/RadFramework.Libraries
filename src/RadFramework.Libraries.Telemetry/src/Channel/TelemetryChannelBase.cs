using System;
using System.Collections.Concurrent;
using System.Threading;
using RadFramework.Libraries.Threading;

namespace RadFramework.Libraries.Telemetry
{
    public abstract class TelemetryChannelBase : ITelemetryChannel
    {
        private readonly ITelemetryStreamConnectionSink _connectionSink;
        private readonly ITelemetryPackageWrapper _packageWrapper;
        private readonly IContractSerializer _modelSerializer;
        private readonly ITelemetryPackageRouterSelector _packageRouterSelector;
        private readonly ITelemtryPackageThreadShedulerRouter _packageThreadShedulerRouter;
        
        // used to shift serialization of packages that only invoke the remote site out of the thread (events only does not make sense for requests)
        private IThreadSheduler _outputSerializationSheduler;
        // ensures that deserialization does not block the reader thread
        private IThreadSheduler _inputDeserializationSheduler;
        
        ConcurrentDictionary<Guid, (ManualResetEvent Callback, object Response)> requestCallbackRegistry = new ConcurrentDictionary<Guid, (ManualResetEvent Callback, object Response)>();

        public TelemetryChannelBase(
            IContractSerializer modelSerializer,
            ITelemetryStreamConnectionSource connectionSource,
            ITelemetryStreamConnectionSink connectionSink,
            ITelemetryPackageRouterSelector packageRouterSelector,
            ITelemtryPackageThreadShedulerRouter packageThreadShedulerRouter, 
            ITelemetryPackageWrapper packageWrapper,
            IThreadSheduler outputSerializationSheduler,
            IThreadSheduler inputDeserializationSheduler)
        {
            requestCallbackRegistry = new ConcurrentDictionary<Guid, (ManualResetEvent Callback, object Response)>();
            _connectionSink = connectionSink;
            _modelSerializer = modelSerializer;
            _packageRouterSelector = packageRouterSelector;
            _packageThreadShedulerRouter = packageThreadShedulerRouter;
            _packageWrapper = packageWrapper;
            _outputSerializationSheduler = outputSerializationSheduler;
            _inputDeserializationSheduler = inputDeserializationSheduler;
            connectionSource.OnPackageReceived = OnPackageReceived;
        }
        
        public void Dispose()
        {
            _outputSerializationSheduler.Dispose();
            _connectionSink.Dispose();
        }

        private void OnPackageReceived(byte[] package)
        {
            _inputDeserializationSheduler.Enqueue(() =>
            {
                (PackageKind packageType, object payload, Guid? packageId) unwrapped = _packageWrapper.Unwrap(package);

                if (unwrapped.packageType == PackageKind.Response)
                {
                    if (!unwrapped.packageId.HasValue)
                    {
                        // log
                        return;
                    }
                
                    if (requestCallbackRegistry.TryGetValue(unwrapped.packageId.Value, out (ManualResetEvent Callback, object Response) openRequest))
                    {
                        requestCallbackRegistry[unwrapped.packageId.Value] = (null, unwrapped.payload);
                        openRequest.Callback.Set();
                    }

                    return;
                }

                ShedulePackageEvaluation(unwrapped);
            });
        }

        private void ShedulePackageEvaluation((PackageKind packageType, object payload, Guid? packageId) unwrapped)
        {
            _packageThreadShedulerRouter
                .GetShedulerByPackageKind(unwrapped.packageType)
                .Enqueue(() =>
                {
                    object response = _packageRouterSelector
                        .GetPackageKindRouterDelegate(unwrapped.packageType)
                        ((unwrapped.packageType, unwrapped.payload));
                    
                    if(!(response is NoResponse))
                    {
                        _connectionSink.SendPackage(
                            _modelSerializer.Serialize(
                                response.GetType(),
                                response));
                    }
                });
        }

        public void NotifyEvent(object @event)
        {
            _outputSerializationSheduler.Enqueue(() =>
            {
                IPayloadPackage wrappedPackage = _packageWrapper.Wrap(PackageKind.Event, @event);
                _connectionSink.SendPackage(_modelSerializer.Serialize(wrappedPackage.GetType(), wrappedPackage));
            });
        }

        public void InvokeDispatched(object request)
        {
            _outputSerializationSheduler.Enqueue(() =>
            {
                IPayloadPackage wrappedPackage = _packageWrapper.Wrap(PackageKind.DispatchedRequestInvocation, request);
                _connectionSink.SendPackage(_modelSerializer.Serialize(request.GetType(), wrappedPackage));
            });
        }

        public SlimTask Execute(object request)
        {
            Guid requestId = Guid.NewGuid();
            ManualResetEvent callback = new ManualResetEvent(false);
            requestCallbackRegistry[requestId] = (callback, null);
            
            IPayloadPackage wrappedPackage = _packageWrapper.Wrap(PackageKind.AwaitableRequestInvocation, request, requestId);
            _connectionSink.SendPackage(_modelSerializer.Serialize(request.GetType(), wrappedPackage));

            return new SlimTask(() => AwaitRemoteResponse(requestId, callback));
        }

        public SlimTask<object> Request(object request)
        {
            Guid requestId = Guid.NewGuid();
            ManualResetEvent callback = new ManualResetEvent(false);
            requestCallbackRegistry[requestId] = (callback, null);
            
            IPayloadPackage wrappedPackage = _packageWrapper.Wrap(PackageKind.Request, request, requestId);

            var serialized = _modelSerializer.Serialize(wrappedPackage.GetType(), wrappedPackage);
            
            _connectionSink.SendPackage(serialized);
            
            return AwaitRemoteResponse(requestId, callback);
        }
        
        private SlimTask<object> AwaitRemoteResponse(Guid requestId, ManualResetEvent callback)
        {
            callback.WaitOne();
            
            requestCallbackRegistry.TryRemove(requestId, out var response);

            if (response.Response is RemoteExceptionResponse exceptionResponse)
            {
                throw exceptionResponse.Exception;
            }

            return new SlimTask<object>(response.Response);
        }
    }
}