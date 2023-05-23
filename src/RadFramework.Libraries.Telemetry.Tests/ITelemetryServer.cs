using System;
using RadFramework.Libraries.Telemetry;

namespace Tests
{
    public interface ITelemetryServer
    {
        ITelemetryChannel GetChannel(Guid clientId);
        
    }
}