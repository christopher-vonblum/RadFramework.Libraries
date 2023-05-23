using System;
using System.Threading.Tasks;
using RadFramework.Libraries.Threading;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryChannel
    {
        void NotifyEvent(object @event);
        void InvokeDispatched(object request);
        SlimTask Execute(object request);
        SlimTask<object> Request(object request);
    }
}