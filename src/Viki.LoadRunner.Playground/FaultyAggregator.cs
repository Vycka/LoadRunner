﻿using Viki.LoadRunner.Engine.Core.Collector.Interfaces;

namespace Viki.LoadRunner.Playground
{
    public class FaultyAggregator : IAggregator
    {
        private readonly bool _begin, _receive, _end;

        public FaultyAggregator(bool begin, bool receive, bool end)
        {
            _begin = begin;
            _receive = receive;
            _end = end;
        }

        public void Begin()
        {
            if (_begin)
                throw new System.NotImplementedException();
        }

        public void Aggregate(IResult result)
        {
            if (_receive)
                throw new System.NotImplementedException();
        }

        public void End()
        {
            if (_end)
                throw new System.NotImplementedException();
        }
    }
}