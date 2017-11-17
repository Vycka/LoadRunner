using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay;
using Viki.LoadRunner.Engine.Strategies.Replay.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader;
using Viki.LoadRunner.Engine.Strategies.Replay.Reader.Interfaces;

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
            return SetScenario(new ScenarioFactory<IReplayScenario<TData>>(scenarioType));
        }

        /// <summary>
        /// Set own custom IReplayScenario&lt;TData&gt; factory
        /// </summary>
        /// <param name="scenarioFactory">IReplayScenario&lt;TData&gt; factory it self</param>
        public ReplayStrategyBuilder<TData> SetScenario(IScenarioFactory scenarioFactory)
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
            DataReader = new ArrayDataReader(data);

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
        /// Sets timeout time for stopping worker-threads.
        /// </summary>
        /// <param name="timeout">timeout duration</param>
        public ReplayStrategyBuilder<TData> SetFinishTimeout(TimeSpan timeout)
        {
            FinishTimeout = timeout;
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

        /// <summary>
        /// Sets initial user data which will be passed to created thread contexts.
        /// </summary>
        /// <param name="userData">User-data object</param>
        public ReplayStrategyBuilder<TData> SetUserData(object userData)
        {
            InitialUserData = userData;
            return this;
        }

        /// <summary>
        /// Sets aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        public ReplayStrategyBuilder<TData> SetAggregator(params IAggregator[] aggregagors)
        {
            Aggregators = aggregagors;
            return this;
        }

        /// <summary>
        /// Adds aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        public ReplayStrategyBuilder<TData> AddAggregator(params IAggregator[] aggregagors)
        {
            Aggregators = Aggregators.Concat(aggregagors).ToArray();
            return this;
        }

        /// <summary>
        /// Initialize IStrategy from this configuration and then LoadRunnerEngine it self using it.
        /// </summary>
        /// <returns>LoadRunnerEngine instance with configured strategy</returns>
        public LoadRunnerEngine Build()
        {
            IStrategy strategy = new ReplayStrategy<TData>(this);
            LoadRunnerEngine engine = new LoadRunnerEngine(strategy);

            return engine;
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
        public IScenarioFactory ScenarioFactory { get; set; }

        /// <summary>
        /// Initial user data which will be passed to created thread contexts. (context.UserData)
        /// </summary>
        public object InitialUserData { get; set; }

        /// <summary>
        /// Timeout for strategy threads to stop and cleanup.
        /// This does not affect result IAggregator and execution will still hold indefinetely until its finished.
        /// </summary>
        public TimeSpan FinishTimeout { get; set; } = TimeSpan.FromMinutes(3);

        #endregion
    }

    public static class ReplayStrategySettingsExtensions
    {

        /// <summary>
        /// Initialize IStrategy from this configuration and then LoadRunnerEngine it self using it. 
        /// </summary>
        /// <typeparam name="TData">Replay strategy SetData() type</typeparam>
        /// <param name="settings">Strategy settings</param>
        /// <returns></returns>
        public static LoadRunnerEngine Build<TData>(this IReplayStrategySettings<TData> settings)
        {
            IStrategy strategy = new ReplayStrategy<TData>(settings);
            LoadRunnerEngine engine = new LoadRunnerEngine(strategy);

            return engine;
        }

        /// <summary>
        /// Duplicates configuration builder having own configuration lists. But registered configuration instances will still be the same.
        /// </summary>
        /// <typeparam name="TData">Replay strategy SetData() type</typeparam>
        /// <param name="settings">Settings instance to clone</param>
        /// <returns>New instance of ReplayStrategyBuilder</returns>
        public static ReplayStrategyBuilder<TData> ShallowClone<TData>(this IReplayStrategySettings<TData> settings)
        {
            return new ReplayStrategyBuilder<TData>
            {
                ThreadCount = settings.ThreadCount,
                DataReader = settings.DataReader,
                SpeedMultiplier = settings.SpeedMultiplier,
                Aggregators = settings.Aggregators.ToArray(),

                ScenarioFactory = settings.ScenarioFactory,

                FinishTimeout = settings.FinishTimeout,
                InitialUserData = settings.InitialUserData,
            };
        }
    }
}