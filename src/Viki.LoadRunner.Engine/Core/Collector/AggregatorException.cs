using System;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Core.Collector
{
    public class AggregatorException : Exception
    {
        public IAggregator Aggregator { get; }

        public IResult Data { get; }

        public AggregatorException(string message, IAggregator sender, IResult data, Exception innerException) : base(message, innerException)
        {
            Aggregator = sender ?? throw new ArgumentNullException(nameof(sender));
            Data = data;
        }
    }
}