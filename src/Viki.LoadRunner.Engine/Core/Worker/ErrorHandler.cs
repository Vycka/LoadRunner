using System;
using System.Collections.Concurrent;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Worker
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ConcurrentBag<Exception> _errors = new ConcurrentBag<Exception>();

        public void Register(IThread worker)
        {
            worker.ThreadError += OnThreadError;
        }

        private void OnThreadError(IThread sender, Exception ex)
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