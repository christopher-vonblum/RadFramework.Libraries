using System;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryStreamConnectionSink : IDisposable
    {
        void SendPackage(byte[] package);
    }
}