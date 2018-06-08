using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Data;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Data.Readers;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory;
using Viki.LoadRunner.Engine.Strategies.Replay.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;

namespace Viki.LoadRunner.Engine.Strategies
{
    /// <summary>
    /// Wizzard class for configuring and building replay test strategy.
    /// </summary>
    /// <typeparam name="TData">Replay strategy SetData() type</typeparam>
    public class ReplayStrategyBuilder<TData> : IReplayStrategySettings<TData>
    {
        #region Builder methods

        /// <summary>
        /// Set scenario to execute.
        /// </summary>
        /// <typeparam name="TScenario">Scenario class type</typeparam>
        public ReplayStrategyBuilder<TData> SetScenario<TScenario>()
            where TScenario : IReplayScenario<TData>
        {
            return SetScenario(typeof(TScenario));
        }

        /// <summary>
        /// Set scenario to execute.
        /// </summary>
        /// <param name="scenarioType">Scenario class type</param>
        public ReplayStrategyBuilder<TData> SetScenario(Type scenarioType)
        {
            return SetScenario(new ReplayScenarioFactory<TData>(scenarioType));
        }

        /// <summary>
        /// Set own custom IReplayScenario&lt;TData&gt; factory
        /// </summary>
        /// <param name="scenarioFactory">IReplayScenario&lt;TData&gt; factory it self</param>
        public ReplayStrategyBuilder<TData> SetScenario(IReplayScenarioFactory<TData> scenarioFactory)
        {
            ScenarioFactory = scenarioFactory;
            return this;
        }

        /// <summary>
        /// Set thread count to use.
        /// Recommended minimum value depends on test case. 
        /// But it always should be at least bigger than expected maximum concurrency. 
        /// E.g. [Max expected concurrency] * 2.5
        /// </summary>
        /// <param name="threadCount">Count of threads to create</param>
        public ReplayStrategyBuilder<TData> SetThreadCount(int threadCount)
        {
            ThreadCount = threadCount;

            return this;
        }

        /// <summary>
        /// Set fixed list of timestamps and data to execute test.
        /// It must be already sorted by timestamp or you will starve the threads.
        /// </summary>
        /// <param name="data">Fixed list of data</param>
        public ReplayStrategyBuilder<TData> SetData(DataItem[] data)
        {
            DataReader = new ReplayDataReader(data);

            return this;
        }

        /// <summary>
        /// Set custom data source for ReplayScenario
        /// </summary>
        /// <param name="dataReader"></param>
        /// <returns></returns>
        public ReplayStrategyBuilder<TData> SetData(IReplayDataReader dataReader)
        {
            DataReader = dataReader;

            return this;
        }

        /// <summary>
        /// Set speed multiplier on how fast to replay test data
        /// </summary>
        /// <param name="speedMultiplier">Speed multiplier, default is 1x </param>
        public ReplayStrategyBuilder<TData> SetSpeed(double speedMultiplier)
        {
            SpeedMultiplier = speedMultiplier;
            return this;
        }

        #endregion

        #region IReplayStrategySettings

        /// <summary>
        /// Fixed count of threads to use
        /// </summary>
        public int ThreadCount { get; set; } = 50;

        /// <summary>
        /// Datasource which defines timeline of replay execution
        /// </summary>
        public IReplayDataReader DataReader { get; set; }

        /// <summary>
        /// Speed multiplier at which Replay strategy will run
        /// </summary>
        public double SpeedMultiplier { get; set; } = 1;

        /// <summary>
        /// Aggregators to collect the data
        /// </summary>
        public IAggregator[] Aggregators { get; set; } = { };

        /// <summary>
        /// Class type of Scenario to be executed, type must implement IReplayScenario.
        /// </summary>
        public IReplayScenarioFactory<TData> ScenarioFactory { get; set; }

        /// <summary>
        /// Initial user data which will be passed to created thread contexts. (context.UserData)
        /// </summary>
        public object InitialUserData { get; set; }

        /// <summary>
        /// Timeout for strategy threads to stop and cleanup.
        /// This does not affect result IAggregator and execution will still hold indefinetely until its finished.
        /// </summary>
        public TimeSpan FinishTimeout { get; set; } = TimeSpan.FromMinutes(3);


        /// <summary>
        /// Initialize IStrategy from this configuration.
        /// </summary>
        /// <returns>Configured IStrategy instance</returns>
        IStrategy IStrategyBuilder.Build()
        {
            IStrategy strategy = new ReplayStrategy<TData>(this);
            return strategy;
        }

        /// <summary>
        /// Duplicates configuration builder having own configuration lists. But registered configuration instances will still be the same.
        /// </summary>
        /// <returns>New instance of IStrategyBuilder</returns>
        IStrategyBuilder IStrategyBuilder.ShallowCopy()
        {
            return new ReplayStrategyBuilder<TData>
            {
                ThreadCount = ThreadCount,
                DataReader = DataReader,
                SpeedMultiplier = SpeedMultiplier,
                Aggregators = Aggregators.ToArray(),

                ScenarioFactory = ScenarioFactory,

                FinishTimeout = FinishTimeout,
                InitialUserData = InitialUserData,
            };
        }

        #endregion
    }
}