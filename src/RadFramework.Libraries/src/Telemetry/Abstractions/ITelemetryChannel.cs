using System;
using System.Threading.Tasks;
using RadFramework.Libraries.Threading;
using RadFramework.Libraries.Threading.Tasks;

namespace RadFramework.Libraries.Telemetry
{
    public interface ITelemetryChannel
    {
        void NotifyEvent(object @event);
        void InvokeDispatched(object request);
        ThreadedTask Execute(object request);
        ThreadedTask<object> Request(object request);
    }
}