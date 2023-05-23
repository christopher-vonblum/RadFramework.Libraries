using System;

namespace RadFramework.Libraries.Telemetry
{
    public interface IRequest<T>
    {
        Guid ResponseToken { get; }
    }
}