using System;
using System.Linq;
using Viki.LoadRunner.Engine.Aggregators.Interfaces;
using Viki.LoadRunner.Engine.Executor.Scenario.Interfaces;
using Viki.LoadRunner.Engine.Presets.Interfaces;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Interfaces;
using Viki.LoadRunner.Engine.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Presets
{
    /// <summary>
    /// LoadRunner custom settings builder & default ILoadRunnerSettings implementation template.
    /// </summary>
    public class StrategyBuilder : ICustomStrategySettings
    {
        #region Constructors & Static creators

        /// <summary>
        /// Create new settings instance and sets scenario to execute from this constructor
        /// </summary>
        /// <typeparam name="TLoadTestScenario">Scenario to execute</typeparam>
        public static StrategyBuilder Create<TLoadTestScenario>()
            where TLoadTestScenario : IScenario
        {
            return new StrategyBuilder(typeof(TLoadTestScenario));
        }

        /// <summary>
        /// Create new settings instance and sets scenario to execute from this constructor
        /// </summary>
        /// <param name="scenarioType">Scenario to execute</param>
        public StrategyBuilder(Type scenarioType)
        {
            TestScenarioType = scenarioType;
        }

        /// <summary>
        /// Create new settings instance.
        /// </summary>
        public StrategyBuilder()
        {
        }

        #endregion

        #region Settings builder

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <typeparam name="TScenario">Scenario class type</typeparam>
        public StrategyBuilder SetScenario<TScenario>()
            where TScenario : IScenario
        {
            TestScenarioType = typeof(TScenario);
            return this;
        }

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <param name="scenarioType">Scenario class type</param>
        public StrategyBuilder Set(Type scenarioType)
        {
            TestScenarioType = scenarioType;
            return this;
        }

        /// <summary>
        /// Sets rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies set</param>
        public StrategyBuilder Set(params ILimitStrategy[] limits)
        {
            Limits = limits;
            return this;
        }

        /// <summary>
        /// Adds rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies add</param>
        public StrategyBuilder Add(params ILimitStrategy[] limits)
        {
            Limits = Limits.Concat(limits).ToArray();
            return this;
        }

        /// <summary>
        /// Sets test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies set</param>
        public StrategyBuilder Set(params ISpeedStrategy[] speed)
        {
            Speed = speed;
            return this;
        }

        /// <summary>
        /// Adds test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies add</param>
        public StrategyBuilder Add(params ISpeedStrategy[] speed)
        {
            Speed = Speed.Concat(speed).ToArray();
            return this;
        }

        /// <summary>
        /// Sets created worker-thread count controlling strategy.
        /// </summary>
        /// <param name="threadingStrategy">threading strategy to use</param>
        /// <returns></returns>
        public StrategyBuilder Set(IThreadingStrategy threadingStrategy)
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
        public StrategyBuilder Set(params IResultsAggregator[] aggregagors)
        {
            Aggregators = aggregagors;
            return this;
        }

        /// <summary>
        /// Adds aggregators to use when collecting data
        /// </summary>
        /// <param name="aggregagors">aggregators</param>
        /// <returns></returns>
        public StrategyBuilder Add(params IResultsAggregator[] aggregagors)
        {
            Aggregators = Aggregators.Concat(aggregagors).ToArray();
            return this;
        }

        #endregion

        #region ILoadRunnerSettings & Properties

        /// <summary>
        /// Scenario to execute, type must implement ILoadTestScenario.
        /// </summary>
        public Type TestScenarioType { get; set; }

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
        public ISpeedStrategy[] Speed { get; set; } = { };

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
        /// Aggregators receive raw meassurements from each iteration and can either slice/dice the results using analytical HistogramAggregator, or get the raw stream using StreamAggregator.
        /// </summary>
        public IResultsAggregator[] Aggregators { get; set; } = { };

        #endregion
    }
}

