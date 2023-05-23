using System;
using System.Threading;
using NUnit.Framework;

namespace RadFramework.Libraries.Threading.Tests
{
    public class ThreadAssumptions
    {
        [Test]
        public void JoinOnExitedThread()
        {
            Thread t = new Thread(() => Console.WriteLine("Executed"));
            
            t.Start();

            while (t.ThreadState == ThreadState.Running)
            {
                Thread.Sleep(100);
            }
            
            Assert.True(t.ThreadState == ThreadState.Stopped);

            // shall not throw otherwise test fails
            t.Join();
        }
    }
}