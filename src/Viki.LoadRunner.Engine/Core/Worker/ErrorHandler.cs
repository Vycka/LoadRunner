using System;
using System.Collections.Concurrent;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Worker
{
    public class ErrorHandler : IErrorHandler
    {
        private readonly ConcurrentBag<WorkerException> _errors = new ConcurrentBag<WorkerException>();

        public void Register(IThread worker)
        {
            worker.ThreadError += OnThreadError;
        }

        private void OnThreadError(IThread sender, Exception ex)
        {
            if (ex.GetType() != typeof(ThreadAbortException))
            {
                _errors.Add(new WorkerException(ex.Message, sender, ex));
            }
        }

        public void Assert()
        {
            if (_errors.Count != 0)
            {
                WorkerException resultError;
                _errors.TryTake(out resultError);

                if (resultError != null)
                    throw resultError;
            }

        }
    }
}