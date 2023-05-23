namespace RadFramework.Libraries.Telemetry
{
    public static class TelemetryChannelExtensions
    {
        public static TResponse Request<TRequest, TResponse>(this ITelemetryChannel channel, IRequest<TResponse> request) where TRequest : IRequest<TResponse>
        {
            var task = channel.Request(request);
            task.Await();
            return (TResponse) task.Result;
        }
    }
}