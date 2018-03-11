using System;
using System.Diagnostics;
using System.Threading;
using Viki.LoadRunner.Engine.Core.Scenario.Interfaces;

namespace LoadRunner.Demo.Detailed
{
    // Extended information
    // https://github.com/Vycka/LoadRunner/wiki/ILoadTestScenario


    // To create scenario, either implement ILoadTestScenario or extend LoadTestBase
    //
    // [DebuggerStepThrough] prevents debugger from catching exceptions and breaking code execution.
    // Though for optimal performance (if high throughput is needed) debbuger shouldn't be attached while running the test.
    [DebuggerStepThrough] 
    public class Scenario : IScenario
    {
        private static readonly Random Random = new Random(42);

        public void ScenarioSetup(IIteration context)
        {
            // ScenarioSetup() gets executed once for each created instance
            //
            // Exceptions here are not handled and breaks the test!

            Console.WriteLine($"Created Thread {context.ThreadId}");
        }

        public void IterationSetup(IIteration testContext)
        {

            // IterationSetup is executed before each Iteration call

            if (Random.Next(100) % 50 == 0)
                throw new Exception("2% error chance for testing");
        }

        public void ExecuteScenario(IIteration context)
        {
            // ExecuteScenario defines single iteration for load test scenario,
            //
            // It is called after each successful IterationSetup call. 
            //
            // Execution time is measured only for this function
            //
            // [Advanced] context.Checkpoint() can be used to mark custom points, where time should be measured


            Thread.Sleep(Random.Next(200));

            if (Random.Next(100) % 10 == 0)
                throw new Exception("10% error chance for testing");

            Thread.Sleep(Random.Next(800));
        }


        public void IterationTearDown(IIteration context)
        {
            // IterationTearDown is executed each time after ExecuteScenario iteration is finished.
            // It is also executed even if IterationSetup or ExecuteScenario fails. (throws exception)

            if (Random.Next(100) % 25 == 0)
                throw new Exception("4% error chance for testing");
        }

        public void ScenarioTearDown(IIteration context)
        {
            // ScenarioTearDown gets executed once for each thread instance
            // which is about to be stopped (due to ending test or thread being stopped by threading strategy)
            //
            // Exceptions here are not handled and breaks the test!
        }
    }
}