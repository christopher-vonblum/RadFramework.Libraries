using System;

namespace RadFramework.Libraries.Telemetry
{
    [Flags]
    public enum PackageKind
    {
        Response = 0,
        Event = 2,
        Request = 4,
        DispatchedRequestInvocation = 8 | Request,
        AwaitableRequestInvocation = 16 | Request
    }
}