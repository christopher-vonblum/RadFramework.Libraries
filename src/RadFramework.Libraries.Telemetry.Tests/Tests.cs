using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MessagePack;
using NUnit.Framework;
using RadFramework.Libraries.Telemetry;
using RadFramework.Libraries.Telemetry.Encryption;

namespace Tests
{
    public partial class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var server = new TelemetryServer(
                new IPEndPoint(IPAddress.Any, 1234),
                (kind, payload) =>
                {
                    if (payload is PingRequest)
                    {
                        return payload;
                    }

                    throw new NotImplementedException();
                },
                (kind, payload) => throw new NotImplementedException(),
                new ContractSerializer());
            
            var client = new TelemetryClient(
                new IPEndPoint(IPAddress.Loopback, 1234),
                (kind, payload) => throw new NotImplementedException(),
                (kind, payload) => throw new NotImplementedException(),
                new ContractSerializer(),
                new NoCryptoProvider());
            
            Thread.Sleep(5000);
            
            var result = client.Channel.Request(new PingRequest());
            result.Await();
            ;
        }
        
        [MessagePackObject]
        public class PingRequest
        {
        }
    }
}