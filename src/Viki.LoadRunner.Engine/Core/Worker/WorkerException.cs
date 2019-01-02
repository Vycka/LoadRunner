using System;
using Viki.LoadRunner.Engine.Core.Worker.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Worker
{
    public class WorkerException : Exception
    {
        public IThread Sender { get; }

        public WorkerException(string message, IThread sender, Exception innerException)
            : base(message, innerException)
        {
            Sender = sender;
        }
    }
}