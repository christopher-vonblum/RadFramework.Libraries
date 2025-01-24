using System.Reflection;
using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;
using RadFramework.Libraries.Logging;
using RadFramework.Libraries.Net.Http;
using RadFramework.Libraries.Serialization.Json.ContractSerialization;
using RadFramework.Servers.Web.Config;
using RadFramework.Servers.Web.Pipes;

namespace RadFramework.Servers.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IocContainer iocContainer = new IocContainer();
            
            SetupIocContainer(iocContainer);

            PipelineDefinition httpPipelineDefinition = LoadHttpPipelineConfig();
            
            HttpServerWithPipeline pipelineDrivenHttpServer = new HttpServerWithPipeline(
                80, 
                httpPipelineDefinition, 
                iocContainer);
            
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            pipelineDrivenHttpServer.Dispose();
        }

        private static void SetupIocContainer(IocContainer iocContainer)
        {
            iocContainer.RegisterSingletonInstance<ISimpleCache>(new SimpleCache());
            iocContainer.RegisterSingletonInstance<ILogger>(
                new StandardLogger(
                    new ILoggerSink[]
                    {
                        new ConsoleLogger(),
                        new FileLogger("logs")
                    }));
        }

        private static PipelineDefinition LoadHttpPipelineConfig()
        {
            PipelineDefinition httpPipelineDefinition = new();

            HttpPipelineConfig config = (HttpPipelineConfig)JsonContractSerializer.Instance.Deserialize(
                typeof(HttpPipelineConfig),
                File.ReadAllBytes("Config/HttpPipelineConfig.json"));
            
            config.Pipes.ToList().ForEach(pipeType => httpPipelineDefinition.Append(Type.GetType(pipeType)));

            return httpPipelineDefinition;
        }
    }
}