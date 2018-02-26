using System;
using System.Linq;
using Viki.LoadRunner.Engine.Core.Collector.Interfaces;
using Viki.LoadRunner.Engine.Core.Factory;
using Viki.LoadRunner.Engine.Core.Factory.Interfaces;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom;
using Viki.LoadRunner.Engine.Strategies.Custom.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Strategies
{
    /// <summary>
    /// Wizzard class for configuring and building custom test strategy.
    /// </summary>
    public class StrategyBuilder : ICustomStrategySettings
    {
        #region Settings builder

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <typeparam name="TScenario">Scenario class type</typeparam>
        public StrategyBuilder SetScenario<TScenario>()
            where TScenario : IScenario
        {
            return SetScenario(typeof(TScenario));
        }

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <param name="scenarioType">Scenario class type</param>
        public StrategyBuilder SetScenario(Type scenarioType)
        {
            return SetScenario(new ScenarioFactory(scenarioType));
        }

        /// <summary>
        /// Set own custom IScenario factory
        /// </summary>
        /// <param name="scenarioFactory">IScenario factory it self</param>
        public StrategyBuilder SetScenario(IScenarioFactory scenarioFactory)
        {
            ScenarioFactory = scenarioFactory;
            return this;
        }

        /// <summary>
        /// Sets rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies set</param>
        public StrategyBuilder SetLimit(params ILimitStrategy[] limits)
        {
            Limits = limits;
            return this;
        }

        /// <summary>
        /// Adds rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies add</param>
        public StrategyBuilder AddLimit(params ILimitStrategy[] limits)
        {
            Limits = Limits.Concat(limits).ToArray();
            return this;
        }

        /// <summary>
        /// Sets test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies set</param>
        public StrategyBuilder SetSpeed(params ISpeedStrategy[] speed)
        {
            Speeds = speed;
            return this;
        }

        /// <summary>
        /// Adds test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies add</param>
        public StrategyBuilder AddSpeed(params ISpeedStrategy[] speed)
        {
            Speeds = Speeds.Concat(speed).ToArray();
            return this;
        }

        /// <summary>
        /// Sets created worker-thread count controlling strategy.
        /// </summary>
        /// <param name="threadingStrategy">threading strategy to use</param>
        /// <returns></returns>
        public StrategyBuilder SetThreading(IThreadingStrategy threadingStrategy)
        {
            Threading = threadingStrategy;
            return this;
        }

        /// <summary>
        /// Sets timeout time for stopping worker-threads.
        /// </summary>
        /// <param name="timeout">timeout duration</param>
        public StrategyBuilder SetFinishTimeout(TimeSpan timeout)
        {
            FinishTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets initial user data which will be passed to created thread contexts.
        /// </summary>
        /// <param name="userData">User-data object</param>
        public StrategyBuilder SetUserData(object userData)
        {
            InitialUserData = userData;
            return this;
        }
        /// <summary>
        /// Sets aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public StrategyBuilder SetAggregator(params IAggregator[] aggregagors)
        {
            Aggregators = aggregagors;
            return this;
        }

        /// <summary>
        /// Adds aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public StrategyBuilder AddAggregator(params IAggregator[] aggregagors)
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
            return new LoadRunnerEngine(new CustomStrategy(this));
        }

        #endregion

        #region ILoadRunnerSettings & Properties

        /// <summary>
        /// Scenario to execute, type must implement ILoadTestScenario.
        /// </summary>
        public IScenarioFactory ScenarioFactory { get; set; }

        /// <summary>
        /// Limits define when test execution will be scheduled to stop.
        /// Defaults are unlimited so at least one limit must be defined.
        /// Keep in mind that limits can't enforce stopping precisely how defined. E.g. you can't make it stop at exactly at 2000 iterations, but it will be close to it.
        /// Limits precision strongly correlate to LoadRunnerEngine.HeartBeatMs
        /// </summary>
        public ILimitStrategy[] Limits { get; set; } = { };

        /// <summary>
        /// SpeedStrategies defines limitations related to executed iteration-per-second capping.
        /// (Default: Unlimited, aka MaxSpeed())
        /// </summary>
        public ISpeedStrategy[] Speeds { get; set; } = { };

        /// <summary>
        /// Threading strategy defines Created worker-thread count throughout the test.
        /// (Default: 10 Threads)
        /// </summary>
        public IThreadingStrategy Threading { get; set; } = new FixedThreadCount(10);

        /// <summary>
        /// Time threshold how long engine should give worker-threads to finish gracefully once they are scheduled to stop.
        /// If threshold is reached, worker-threads will be killed with Thread.Abort() and collected iteration [IResult] value will be lost.
        /// Default value: 3 minutes
        /// </summary>
        public TimeSpan FinishTimeout { get; set; } = TimeSpan.FromMinutes(3);

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        public object InitialUserData { get; set; } = null;

        /// <summary>
        /// Aggregators to collect the data
        /// </summary>
        public IAggregator[] Aggregators { get; set; } = { };

        #endregion
    }

    public static class CustomStrategySettingsExtensions
    {
        /// <summary>
        /// Duplicates configuration builder having own configuration lists. But registered configuration instances will still be the same.
        /// </summary>
        /// <param name="settings">Settings instance to clone</param>
        public static StrategyBuilder ShallowClone(this ICustomStrategySettings settings)
        {
            return new StrategyBuilder
            {
                Limits = settings.Limits.ToArray(),
                Speeds = settings.Speeds.ToArray(),
                Threading = settings.Threading,
                FinishTimeout = settings.FinishTimeout,
                ScenarioFactory = settings.ScenarioFactory,
                InitialUserData = settings.InitialUserData,
                Aggregators = settings.Aggregators.ToArray()
            };
        }
    }
}

