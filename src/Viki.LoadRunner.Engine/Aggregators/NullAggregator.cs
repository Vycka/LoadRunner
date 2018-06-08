﻿using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Engine.Aggregators
{
    public class NullAggregator : IAggregator
    {
        public void Begin()
        {
        }

        public void Aggregate(IResult result)
        {
        }

        public void End()
        {
        }
    }
}