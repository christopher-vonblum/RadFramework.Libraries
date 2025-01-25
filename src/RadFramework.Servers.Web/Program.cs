using System.Reflection;
using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;
using RadFramework.Libraries.Logging;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Net.Socket;
using RadFramework.Libraries.Serialization.Json.ContractSerialization;
using RadFramework.Servers.Web.Config;

namespace RadFramework.Servers.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IocContainer iocContainer = new IocContainer();
            
            SetupIocContainer(iocContainer);

            PipelineDefinition httpPipelineDefinition = LoadHttpPipelineConfig();

            ILogger logger = iocContainer.Resolve<ILogger>();

            iocContainer.RegisterSingleton<TelemetrySocketManager>();

            TelemetrySocketManager socketManager = iocContainer.Resolve<TelemetrySocketManager>();
            
            HttpServerWithPipeline pipelineDrivenHttpServer = new HttpServerWithPipeline(
                80,
                httpPipelineDefinition,
                (socket, thread, e) => logger.LogError($"Thread: {thread.ThreadId}, Exception: {e}"),
                iocContainer,
                socket => socketManager.RegisterNewClientSocket(socket));
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            pipelineDrivenHttpServer.Dispose();
        }

        private static void SetupIocContainer(IocContainer iocContainer)
        {
            IocContainerConfig config = (IocContainerConfig)JsonContractSerializer.Instance.Deserialize(
                typeof(IocContainerConfig),
                File.ReadAllBytes("Config/IocContainerConfig.json"));

            foreach (var iocRegistration in config.Registrations)
            {
                if (iocRegistration.Singleton)
                {
                    iocContainer.RegisterSingleton(
                        Type.GetType(iocRegistration.TKey), 
                        Type.GetType(iocRegistration.TImplementation));
                    continue;
                }
                
                iocContainer.RegisterTransient(
                    Type.GetType(iocRegistration.TKey), 
                    Type.GetType(iocRegistration.TImplementation));
            }
            
            iocContainer.RegisterSingleton<HttpServerContext>();
            iocContainer.RegisterSingletonInstance<ISimpleCache>(new SimpleCache());
            iocContainer.RegisterSingletonInstance<ILogger>(
                new StandardLogger(
                    new ILoggerSink[]
                    {
                        new ConsoleLogger(),
                        new FileLogger("Logs")
                    }));
        }

        private static PipelineDefinition LoadHttpPipelineConfig()
        {
            PipelineDefinition httpPipelineDefinition = new();

            HttpPipelineConfig config = (HttpPipelineConfig)JsonContractSerializer.Instance.Deserialize(
                typeof(HttpPipelineConfig),
                File.ReadAllBytes("Config/HttpPipelineConfig.json"));
            
            config
                .Pipes
                .ToList()
                .ForEach(pipeType => 
                    httpPipelineDefinition.
                        Append(Type.GetType(pipeType)));

            return httpPipelineDefinition;
        }
    }
}