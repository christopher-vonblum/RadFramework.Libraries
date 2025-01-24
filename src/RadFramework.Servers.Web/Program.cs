using RadFramework.Libraries.Caching;
using RadFramework.Libraries.Extensibility.Pipeline;
using RadFramework.Libraries.Extensibility.Pipeline.Synchronous;
using RadFramework.Libraries.Ioc;
using RadFramework.Libraries.Logging;
using RadFramework.Libraries.Net.Http;
using RadFramework.Servers.Web.Pipes;

namespace RadFramework.Servers.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            IocContainer iocContainer = new IocContainer();
            
            SetupIocContainer(iocContainer);
            
            PipelineDefinition<HttpConnection, byte[]> httpPipelineDefinition = new PipelineDefinition<HttpConnection, byte[]>();
            
            httpPipelineDefinition.Append<StaticHtmlPipe>();
            
            HttpServerWithPipeline pipelineDrivenHttpServer = new HttpServerWithPipeline(
                80, 
                new SynchronousPipeline<HttpConnection, byte[]>(httpPipelineDefinition, iocContainer));
            
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
    }
}