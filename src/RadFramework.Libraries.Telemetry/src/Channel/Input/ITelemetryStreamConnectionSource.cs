using System;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryStreamConnectionSource  : IDisposable
    {
        Action<byte[]> OnPackageReceived { get; set; }
    }
}