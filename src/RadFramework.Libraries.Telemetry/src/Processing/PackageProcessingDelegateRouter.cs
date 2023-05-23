using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace RadFramework.Libraries.Telemetry
{
    public class PackageProcessingDelegateRouter : ITelemetryPackageRouterSelector
    {
        private readonly ConcurrentDictionary<PackageKind, Func<(PackageKind packageKind, object payload), object>> _routers;

        public PackageProcessingDelegateRouter(IReadOnlyDictionary<PackageKind, Func<(PackageKind packageKind, object payload), object>> requestPackageProcessingRouter)
        {
            _routers = new ConcurrentDictionary<PackageKind, Func<(PackageKind packageKind, object payload), object>>(requestPackageProcessingRouter);
        }
        
        public Func<(PackageKind packageType, object payload), object> GetPackageKindRouterDelegate(PackageKind packageKind)
        {
            if (!packageKind.HasFlag(PackageKind.Request))
            {
                return _routers[packageKind];
            }
            
            return _routers[PackageKind.Request];
        }
    }
}