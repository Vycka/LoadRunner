using System;
using LoadRunner.Demo.Detailed;
using LoadRunner.Demo.Features;
using LoadRunner.Demo.Minimum;
using LoadRunner.Demo.Theoretical;

namespace LoadRunner.Demo
{
    class Program
    {
        // Main() contains two examples
        // #1 is shorter version to see all moving parts in single place
        // #2 contains more detailed information and advanced features
    
        static void Main(string[] args)
        {
            // ProTips: 
            // * If planning to use HttpClient, WebRequest or similar tools to make API Load-Tests
            //   - Make sure to tweak App.config accordingly to lift up .NET connection limit.
            //   - Check example in projects app config (<add address="*" maxconnection="100"/>)
            //
            // * Avoid running real test with debugger attached, as it can catch exceptions halting test execution and meassurements wont be accurate for that time period.
            //
            // * It is also prefered to run in 64bit.
            //
            // Other then that, just follow BareMinimum/DetailedDemo for setup example


            // #0 Quick-Start
            // - One-file setup/execution to get acquainted with framework.
            Run("#0 QuickStart setup", QuickStartDemo.Run);

            // #1 StrategyBuilder
            // - Controlling thread count
            // - Limiting speed
            // - IScenarioFactory for spawning own IScenario instances.
            // - Providing aggregators
            // - Misc (UserData, Timeout)
            // - IValidator asserting scenario on the main thread.

            // #2 Data Aggregation (IAggregator)

            // #2.1 Histogram
            // - Existing Dimensions & Metrics
            // - Creating own IDimension or IMetric

            // #2.2 RawData stream  
            // Offload take raw meassurements from execution without aggregation
            // - Allows writting own simple aggregations
            // - Doing aggregations post-test.
            //   - Persisting raw meassurements so new aggregations could be run later.


            // #5 Replay Strategy
            // - IReplayDataReader
            //

            // #6 Customization 
            // - IAggregator
            // - ISpeedStrategy
            // - IThreadingStrategy
            // - ILimitStrategy

            // #7 Theoretical throughput
            // - Blank scenario
            //   - Without IAggregator
            //   - With IAggregator
            Console.WriteLine("Theoretical throughput");
            TheoreticalSpeedDemo.Run();

            Console.WriteLine("Theoretical with aggregation hooked");
            AggregationImpactDemo.Run();
        


            // Probably drop these below.


            // #2 Contains more detailed information and advanced features
            //Console.WriteLine("Demo #2");
            //DetailedDemo.Run();


            // Optional but useful advanced feature worth checking out before running real test.
            //RawDataAggregationDemo.Aggregate();
            
            Console.ReadKey();
        }

        private static void Run(string title, Action action)
        {
            Console.WriteLine(title);
            action.Invoke();
            Console.Write("\r\n Press enter to continue...");
            Console.ReadLine();
        }
    }
}
