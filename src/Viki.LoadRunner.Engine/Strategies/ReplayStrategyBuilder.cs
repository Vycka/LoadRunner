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
    public class ReplayStrategyBuilder<TData> : IReplayStrategySettings
    {
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
            ScenarioFactory = new ScenarioFactory<IReplayScenario<TData>>(scenarioType);
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

        public LoadRunnerEngine Build()
        {
            IStrategy strategy = new ReplayStrategy<TData>(this);
            LoadRunnerEngine engine = new LoadRunnerEngine(strategy);

            return engine;
        }

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
    }

    public static class ReplayStrategyBuilderExtensions
    {
        public static ReplayStrategyBuilder<TData> Clone<TData>(this ReplayStrategyBuilder<TData> settings)
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