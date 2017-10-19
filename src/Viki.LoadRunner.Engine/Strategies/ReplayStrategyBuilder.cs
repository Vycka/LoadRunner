using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies
{
    public class ReplayStrategyBuilder : IReplayStrategySettings
    {
        /// <summary>
        /// Sets timeout time for stopping worker-threads.
        /// </summary>
        /// <param name="timeout">timeout duration</param>
        public ReplayStrategyBuilder SetFinishTimeout(TimeSpan timeout)
        {
            FinishTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets initial user data which will be passed to created thread contexts.
        /// </summary>
        /// <param name="userData">User-data object</param>
        public ReplayStrategyBuilder SetUserData(object userData)
        {
            InitialUserData = userData;
            return this;
        }
        /// <summary>
        /// Sets aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public ReplayStrategyBuilder Set(params IResultsAggregator[] aggregagors)
        {
            Aggregators = aggregagors;
            return this;
        }

        /// <summary>
        /// Adds aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public ReplayStrategyBuilder Add(params IResultsAggregator[] aggregagors)
        {
            Aggregators = Aggregators.Concat(aggregagors).ToArray();
            return this;
        }

        public int ThreadCount { get; set; } = 50;
        public IReplayDataReader DataReader { get; set; }
        public double SpeedMultiplier { get; set; } = 1;
        public IResultsAggregator[] Aggregators { get; set; } = { };

        public Type ScenarioType { get; set; }
        public object InitialUserData { get; set; }
        public TimeSpan FinishTimeout { get; set; } = TimeSpan.FromMinutes(3);
    }
}