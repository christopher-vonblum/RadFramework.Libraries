using System;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryPackageRouterSelector
    {
        Func<(PackageKind packageType, object payload), object> GetPackageKindRouterDelegate(PackageKind packageKind);
    }
}