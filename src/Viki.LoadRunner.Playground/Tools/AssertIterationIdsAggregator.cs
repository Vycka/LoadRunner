using System;
using System.Collections.Generic;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Playground.Tools
{
    public class AssertIterationIdsAggregator : IAggregator
    {

        private readonly SortedSet<int> _missedOrderIds = new SortedSet<int>();
        private int _nextId;

        public void Begin()
        {
            _missedOrderIds.Clear();
            _nextId = 0;
        }

        public void Aggregate(IResult result)
        {
            if (_nextId == result.GlobalIterationId)
                _nextId++;
            else
            {
                _missedOrderIds.Add(result.GlobalIterationId);
            }
        }

        public void End()
        {
            while (_missedOrderIds.Contains(_nextId))
            {
                _missedOrderIds.Remove(_nextId);
                _nextId++;
            }
        }

        public void PrintResults()
        {
            Console.WriteLine($@"IdsValidator: _nextId: {_nextId}, _missedOrderIds:{_missedOrderIds.Count}");
        }
    }
}