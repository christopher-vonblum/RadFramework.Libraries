using System;
using System.Threading;

namespace RadFramework.Libraries.Threading.Timers
{
    public class JobTimer : IDisposable
    {
        private bool isDisposed;
        private readonly int _interval;
        private readonly Action _onIntervalPassed;
        protected ThreadPriority HandlerThreadPriority { get; }
        private readonly Action<Thread, Exception> _onHandlerThreadException;
        private readonly Thread intervalLoopThread;

        public JobTimer(
            int interval,
            Action onIntervalPassed,
            ThreadPriority handlerThreadPriority,
            Action<Thread, Exception> onHandlerThreadException = null)
        {
            _interval = interval;
            _onIntervalPassed = onIntervalPassed;
            HandlerThreadPriority = handlerThreadPriority;
            _onHandlerThreadException = onHandlerThreadException;
            intervalLoopThread = new Thread(o => IntervalLoop());
            intervalLoopThread.Priority = handlerThreadPriority;
            intervalLoopThread.Start();
        }

        private void IntervalLoop()
        {
            Thread.Sleep(_interval);
            
            while (!isDisposed)
            {
                this.OnIntervalPassedInternal();

                Thread.Sleep(_interval);
            }
        }

        protected virtual void OnIntervalPassedInternal()
        {
            try
            {
                _onIntervalPassed();
            }
            catch (Exception e)
            {
                _onHandlerThreadException(Thread.CurrentThread, e);
            }
        }

        public void Dispose()
        {
            isDisposed = true;
            intervalLoopThread.Join();
        }
    }
}
