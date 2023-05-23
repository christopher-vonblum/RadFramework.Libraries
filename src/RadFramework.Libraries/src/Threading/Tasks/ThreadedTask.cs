using System;
using System.Threading;

namespace RadFramework.Libraries.Threading.Tasks
{
    public class ThreadedTask
        <TIn, TOut>
    {
        private readonly Func<TIn, TOut> task;
        private ManualResetEvent Awaiter { get; set; }
        public Thread Thread { get; private set; }
        
        public TOut Result { get; private set; }
        
        public ThreadedTask(Func<TIn, TOut> task)
        {
            this.task = task;
            Awaiter = new ManualResetEvent(false);
            Thread = new Thread(o => ProcessInternal(o));
        }
        
        public ThreadedTask<TIn, TOut> Start()
        {
            Thread.Start();
            return this;
        }

        public TOut Await()
        {
            Awaiter.WaitOne();
            return Result;
        }
        
        void ProcessInternal(object o)
        {
            var result = task((TIn)o);
            Awaiter.Set();
            Result = result;
        }
    }

    public class ThreadedTask
        <TOut>
    {
        private readonly Func<TOut> task;
        private ManualResetEvent Awaiter { get; set; }
        public Thread Thread { get; private set; }

        public TOut Result { get; private set; }
        
        public ThreadedTask(Func<TOut> task)
        {
            this.task = task;
            Awaiter = new ManualResetEvent(false);
            Thread = new Thread(o => ProcessInternal(o));
        }
        
        public ThreadedTask(TOut task)
        {
            Result = task;
        }

        public ThreadedTask<TOut> Start()
        {
            Thread.Start();
            return this;
        }

        public TOut Await()
        {
            Awaiter.WaitOne();
            return Result;

        }
        
        void ProcessInternal(object o)
        {
            Result = task();
            Awaiter.Set();
        }
    }

    public class ThreadedTask

    {
        private readonly Action task;
        private ManualResetEvent Awaiter { get; set; }
        public Thread Thread { get; private set; }

        public ThreadedTask(Action task)
        {
            this.task = task;
            Awaiter = new ManualResetEvent(false);
            Thread = new Thread(o => ProcessInternal());
        }

        public ThreadedTask Start()
        {
            Thread.Start();
            return this;
        }

        public void Await()
        {
            Awaiter.WaitOne();
        }
        
        void ProcessInternal()
        {
            task();
            Awaiter.Set();
        }
    }
}