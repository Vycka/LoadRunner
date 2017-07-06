using System;
using System.Linq;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;

namespace Viki.LoadRunner.Engine.Settings
{
    // TODO: Make builder with .Add .Set etc

    /// <summary>
    /// LoadRunner engine configuration root
    /// </summary>
    public interface ILoadRunnerSettings
    {
        /// <summary>
        /// Limits define when test execution will be scheduled to stop.
        /// Keep in mind that limits can't enforce stopping precisely how defined. E.g. you can't make it stop at exactly at 2000 iterations, but it will be close to it. 
        /// Limits precision strongly correlate to LoadRunnerEngine.HeartBeatMs
        /// </summary>
        ILimitStrategy[] Limits { get; }

        /// <summary>
        /// Speed strategy is like iteration scheduler. It defines limitations related to executed iteration-per-time capping.
        /// See this.SpeedPriority for prioritization.
        /// </summary>
        ISpeedStrategy[] Speed { get; }

        /// <summary>
        /// Threading strategy defines created and working parallel thread count throughout the LoadTest
        /// </summary>
        IThreadingStrategy Threading { get; }

        /// <summary>
        /// Time threshold how long engine should give worker-threads to finish gracefully once they are scheduled to stop.
        /// If threshold is reached, worker-threads will be killed with Thread.Abort() and collected iteration [IResult] value will be lost.
        /// </summary>
        TimeSpan FinishTimeout { get; }

        /// <summary>
        /// Scenario to execute, type must implement ILoadTestScenario.
        /// </summary>
        Type TestScenarioType { get; }

        /// <summary>
        /// This object-value will be set to testContext.UserData for each created test thread.
        /// </summary>
        object InitialUserData { get; }
    }

    /// <summary>
    /// LoadRunner custom settings builder & default ILoadRunnerSettings implementation template.
    /// </summary>
    public class LoadRunnerSettings : ILoadRunnerSettings
    {
        /// <summary>
        /// Create new settings instance
        /// </summary>
        /// <typeparam name="TLoadTestScenario">Scenario to execute</typeparam>
        /// <returns></returns>
        public static LoadRunnerSettings Create<TLoadTestScenario>()
            where TLoadTestScenario : ILoadTestScenario
        {
            return new LoadRunnerSettings(typeof(TLoadTestScenario));
        }

        public LoadRunnerSettings(Type scenarioType)
        {
            TestScenarioType = scenarioType;
        }

        public LoadRunnerSettings()
        {
        }

        public LoadRunnerSettings SetScenario<TScenario>()
            where TScenario : ILoadTestScenario
        {
            TestScenarioType = typeof(TScenario);
            return this;
        }

        public LoadRunnerSettings SetScenario(Type scenarioType)
        {
            TestScenarioType = scenarioType;
            return this;
        }

        public LoadRunnerSettings SetLimits(params ILimitStrategy[] limits)
        {
            Limits = limits;
            return this;
        }

        public LoadRunnerSettings AddLimits(params ILimitStrategy[] limits)
        {
            Limits = Limits.Concat(limits).ToArray();
            return this;
        }

        public LoadRunnerSettings SetSpeed(params ISpeedStrategy[] speed)
        {
            Speed = speed;
            return this;
        }

        public LoadRunnerSettings AddSpeed(params ISpeedStrategy[] speed)
        {
            Speed = Speed.Concat(speed).ToArray();
            return this;
        }

        public LoadRunnerSettings SetThreading(IThreadingStrategy threadingStrategy)
        {
            Threading = threadingStrategy;
            return this;
        }

        public LoadRunnerSettings SetFinishTimeout(TimeSpan timeout)
        {
            FinishTimeout = timeout;
            return this;
        }

        public LoadRunnerSettings SetUserData(object userData)
        {
            InitialUserData = userData;
            return this;
        }

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
        /// (Default: 10Threads)
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
    }
}

