using System;
using System.Collections.Concurrent;
using System.Threading;
using Viki.LoadRunner.Engine.Executor.Threads.Interfaces;
using Viki.LoadRunner.Engine.Executor.Threads.Workers.Interfaces;

namespace Viki.LoadRunner.Engine.Executor.Threads.Workers
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ConcurrentBag<Exception> _errors = new ConcurrentBag<Exception>();

        public void Register(IWorkerThread worker)
        {
            worker.ThreadError += OnThreadError;
        }

        private void OnThreadError(IWorkerThread sender, Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                _errors.Add(ex);
            }
        }

        public void Assert()
        {
            if (_errors.Count != 0)
            {
                Exception resultError;
                _errors.TryTake(out resultError);

                if (resultError != null)
                    throw resultError;
            }

        }
    }
}