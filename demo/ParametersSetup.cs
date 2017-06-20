using System;
using Viki.LoadRunner.Engine.Parameters;
using Viki.LoadRunner.Engine.Strategies.Speed;
using Viki.LoadRunner.Engine.Strategies.Threading;


namespace LoadRunner.Demo
{
    public static class ParametersSetup
    {
        public static LoadRunnerParameters Build()
        {
            // For this example we will create a little bit modified of default preset (see this.DefaultParametersPreset below).
            LoadRunnerParameters parameters = new LoadRunnerParameters
            {
                // Stop execution after 30 secs.
                Limits = new ExecutionLimits
                {
                    MaxDuration = TimeSpan.FromSeconds(30)
                },

                // Incremental strategy here increases thread count every 10 seconds.
                // This can be aligned with TimeDimension used below in shown aggregator.
                ThreadingStrategy = new IncrementalThreadCount(20, TimeSpan.FromSeconds(10), 20)
            };

            return parameters;
        }

        // This demonstrates default parameters of LoadRunnerParameters object for reference.
        // Default presets will cause unlimited execution, so either [MaxDuration] or [MaxIterationsCount] must be defined. 
        private static readonly LoadRunnerParameters DefaultParametersPreset = new LoadRunnerParameters
        {
            Limits = new ExecutionLimits
            {
                // Maximum LoadTest duration threshold, after which test is stopped.
                MaxDuration = TimeSpan.MaxValue,

                // Maximum executed iterations count threshold, after which test is stopped.
                MaxIterationsCount = Int32.MaxValue,

                // Once LoadTest execution finishes because of [maxDuration] or [maxIterationsCount] limit
                // coordinating thread will wait [FinishTimeout] amount of time before 
                // terminating them with Thread.Abort()
                //
                // Aborted threads won't get the chance to call IterationTearDown() or ScenarioTearDown()
                // neither it will broadcast TestContextResultReceived() to aggregators with the state as it is after abort.
                FinishTimeout = TimeSpan.FromMinutes(3)
            },

            // [ISpeedStrategy] defines maximum allowed load by dampening executed Iterations per second count.
            // * Other existing version of [ISpeedStrategy]
            //    - IncremantalSpeed(initialRequestsPerSec: 1.0, increasePeriod: TimeSpan.FromSeconds(10), increaseStep: 3.0)
            //    - ListOfSpeed(period: TimeSpan.FromSeconds(10), iterationPerSecValues: new [] { 11.1, 5.5, 66.6 })
            SpeedStrategy = new FixedSpeed(maxIterationsPerSec: Double.MaxValue),

            // [IThreadingStrategy] controls allowed worker thread count
            // More info https://github.com/Vycka/LoadRunner/wiki/IThreadingStrategy
            ThreadingStrategy = new SemiAutoThreadCount(minThreadCount: 10, maxThreadCount: 10)
        };
    }
}