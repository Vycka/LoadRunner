using System;
using System.Linq;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Settings
{
    /// <summary>
    /// LoadRunner custom settings builder & default ILoadRunnerSettings implementation template.
    /// </summary>
    public class LoadRunnerSettings : ILoadRunnerSettings
    {
        #region Constructors & Static creators

        /// <summary>
        /// Create new settings instance and sets scenario to execute from this constructor
        /// </summary>
        /// <typeparam name="TLoadTestScenario">Scenario to execute</typeparam>
        public static LoadRunnerSettings Create<TLoadTestScenario>()
            where TLoadTestScenario : ILoadTestScenario
        {
            return new LoadRunnerSettings(typeof(TLoadTestScenario));
        }

        /// <summary>
        /// Create new settings instance and sets scenario to execute from this constructor
        /// </summary>
        /// <param name="scenarioType">Scenario to execute</param>
        public LoadRunnerSettings(Type scenarioType)
        {
            TestScenarioType = scenarioType;
        }

        /// <summary>
        /// Create new settings instance.
        /// </summary>
        public LoadRunnerSettings()
        {
        }

        #endregion

        #region Settings builder

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <typeparam name="TScenario">Scenario class type</typeparam>
        public LoadRunnerSettings SetScenario<TScenario>()
            where TScenario : ILoadTestScenario
        {
            TestScenarioType = typeof(TScenario);
            return this;
        }

        /// <summary>
        /// Set scenario to execute
        /// </summary>
        /// <param name="scenarioType">Scenario class type</param>
        public LoadRunnerSettings SetScenario(Type scenarioType)
        {
            TestScenarioType = scenarioType;
            return this;
        }

        /// <summary>
        /// Sets rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies set</param>
        public LoadRunnerSettings SetLimits(params ILimitStrategy[] limits)
        {
            Limits = limits;
            return this;
        }

        /// <summary>
        /// Adds rules which decide when test execution should finish.
        /// </summary>
        /// <param name="limits">list of strategies add</param>
        public LoadRunnerSettings AddLimits(params ILimitStrategy[] limits)
        {
            Limits = Limits.Concat(limits).ToArray();
            return this;
        }

        /// <summary>
        /// Sets test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies set</param>
        public LoadRunnerSettings SetSpeed(params ISpeedStrategy[] speed)
        {
            Speed = speed;
            return this;
        }

        /// <summary>
        /// Adds test iterations per time limiting strategies.
        /// </summary>
        /// <param name="speed">list of strategies add</param>
        public LoadRunnerSettings AddSpeed(params ISpeedStrategy[] speed)
        {
            Speed = Speed.Concat(speed).ToArray();
            return this;
        }

        /// <summary>
        /// Sets created worker-thread count controlling strategy.
        /// </summary>
        /// <param name="threadingStrategy">threading strategy to use</param>
        /// <returns></returns>
        public LoadRunnerSettings SetThreading(IThreadingStrategy threadingStrategy)
        {
            Threading = threadingStrategy;
            return this;
        }

        /// <summary>
        /// Sets timeout time for stopping worker-threads.
        /// </summary>
        /// <param name="timeout">timeout duration</param>
        public LoadRunnerSettings SetFinishTimeout(TimeSpan timeout)
        {
            FinishTimeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets initial user data which will be passed to created thread contexts.
        /// </summary>
        /// <param name="userData">User-data object</param>
        public LoadRunnerSettings SetUserData(object userData)
        {
            InitialUserData = userData;
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

        #endregion
    }
}

