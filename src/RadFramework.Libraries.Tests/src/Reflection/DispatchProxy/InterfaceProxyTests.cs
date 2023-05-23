using System;
using System.Reflection;
using NUnit.Framework;

namespace RadFramework.Libraries.Reflection.Tests.DispatchProxy
{
    public class InterfaceProxyTests
    {
        [Test]
        public void TestInterfaceProxy()
        {
            TestInterface proxy = (TestInterface)Reflection.DispatchProxy.DispatchProxy.Create(typeof(TestInterface), typeof(Reflection.DispatchProxy.InterfaceProxy));

            proxy.TestProp = "test";
            
            Assert.AreEqual(proxy.TestProp, "test");
        }
    }

    public interface TestInterface
    {
        string TestProp { get; set; }
    }
}