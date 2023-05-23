using System;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryPackageWrapper
    {
        IPayloadPackage Wrap(PackageKind packageKind, object payload, Guid? packageId = null);
        (PackageKind packageType, object payload, Guid? packageId) Unwrap(byte[] package);
    }
}