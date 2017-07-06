using System;
using Viki.LoadRunner.Engine.Settings;
using Viki.LoadRunner.Engine.Strategies;
using Viki.LoadRunner.Engine.Strategies.Limit;
using Viki.LoadRunner.Engine.Strategies.Threading;


namespace LoadRunner.Demo
{
    public static class SettingsSetup
    {
        public static ILoadRunnerSettings Build()
        {
            // For this example we will create a little bit modified default preset (see this.DefaultParametersPreset below).
            LoadRunnerSettings settings = new LoadRunnerSettings()

                // Set test scenario to use - DemoTestScenario
                .SetScenario<DemoTestScenario>()

                // Stop execution after 30 secs.
                .SetLimits(new TimeLimit(TimeSpan.FromSeconds(30)))

                // Incremental strategy here increases thread count every 10 seconds.
                // This can be aligned with TimeDimension in HistogramAggregator as shown later in this example.
                .SetThreading(new IncrementalThreadCount(20, TimeSpan.FromSeconds(10), 20));

            return settings;
        }

        // This demonstrates default parameters of LoadRunnerSettings object for reference.
        // Default presets will cause unlimited execution, so at least one ILimitStrategy must be defined. 
        //
        // All properties can be initialized using either builder methods Set*/Add* or by directly setting parameters.
        private static readonly LoadRunnerSettings DefaultParametersPreset = new LoadRunnerSettings
        {
            // Scenario to execute.
            //
            // No scenario is defined by default and will cause undefined behavior, so it must be set.
            // SetScenario()/SetScenario<>()/Create<>()/Create()
            //
            TestScenarioType = null,

            // Limits define when test execution will be scheduled to stop.
            //
            // By default, no limiting strategies are defined, so to prevent running test indefinetely,
            // at least one limit must be defined using AddLimits()/SetLimits() or directly through Limits parameter.
            //
            // Available ILimitStrategy in namespace Viki.LoadRunner.Engine.Strategies.Limit:
            //
            // - Stop after specified iterations count
            //   IterationLimit(int iterationsLimit)
            //
            // - Stop after specified duration
            //   TimeLimit(TimeSpan timeLimit)
            Limits = new ILimitStrategy[0],

            // Speed strategies will limit executed iteration per second.
            //
            // By default, no speed limits are defined and it will try to execute as many iterations as fast as possible.
            // AddSpeed()/SetSpeed()
            //
            // Available ISpeedStrategy in namespace Viki.LoadRunner.Engine.Strategies.Speed:
            //
            // - Limits executed iterations to defined iterations per second count 
            //   FixedSpeed(double maxIterationsPerSec)
            //   
            // - Define manual list of iterations per sec. values for period.
            //   ListOfSpeed(TimeSpan period, params double[] iterationPerSecValues)
            //
            // - Periodically changes limit by defined periods
            //   IncrementalSpeed(double initialRequestsPerSec, TimeSpan increasePeriod, double increaseStep)
            //
            // - Limits count of concurrently working threads.
            //   LimitWorkingThreads(int workingThreads)
            //
            // - Periodically changes working thread count.
            //   IncrementalLimitWorkingThreads(int initialWorkingThreadCount, TimeSpan increaseTimePeriod, int increaseBatchSize)
            Speed = new ISpeedStrategy[0],

            // Threading strategy defines Created worker-thread count throughout the test.
            // 
            // By default, thread count will be fixed to 10
            //
            // Available IThreadingStrategy in namespace Viki.LoadRunner.Engine.Strategies.Threading:
            //
            // - Keep thread count always fixed
            //   FixedThreadCount(int threadCount)
            //
            // - Periodically change created thread count by defined periods
            //   IncrementalThreadCount(int initialThreadcount, TimeSpan increaseTimePeriod, int increaseBatchSize) 
            //
            // - Define manual list of created thread counts for time periods.
            //   ListOfCounts(TimeSpan period, params int[] threadCountValues)
            Threading = new FixedThreadCount(10),

            // How much time allow threads to finish started iteration gracefully if they are scheduled to be stoppped (e.g. due to ILimitStrategy ending the test)
            FinishTimeout = TimeSpan.FromMinutes(3),

            // UserData object, which will be passed to every newely created thread.
            InitialUserData = null
        };
    }
}