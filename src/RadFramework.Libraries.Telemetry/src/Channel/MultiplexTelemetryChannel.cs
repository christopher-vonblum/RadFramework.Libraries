using System;
using System.Collections.Generic;
using System.Threading;

namespace RadFramework.Libraries.Telemetry
{
    public class MultiplexTelemetryChannel : TelemetryChannelBase
    {
        public MultiplexTelemetryChannel(
            IContractSerializer contractSerializer,
            ProcessTelemetryRequest executeRequestDelegate,
            ProcessTelemetryEvent handleEventDelegate,
            ITelemetryStreamConnectionSource connectionSource,
            ITelemetryStreamConnectionSink connectionSink)
            : base(
                contractSerializer,
                connectionSource, 
                connectionSink, 
                new PackageProcessingDelegateRouter(new Dictionary<PackageKind, Func<(PackageKind packageKind, object payload), object>>
                {
                    {PackageKind.Request, tuple => executeRequestDelegate(tuple.packageKind, tuple.payload)},
                    {PackageKind.Event, tuple => handleEventDelegate(tuple.packageKind, tuple.payload)}
                }), 
                new PackageShedulerRouterMock(new StayInCurrentThreadSheduler()),
                new TelemetryPackageWrapper(contractSerializer),
                new QueuedThreadShedulerWithDispatchCapabilities(250, ThreadPriority.Highest),
                new QueuedThreadShedulerWithDispatchCapabilities(250, ThreadPriority.Highest))
        {
        }
    }
}