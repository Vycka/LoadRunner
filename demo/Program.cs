using System;
using LoadRunner.Demo.Guides.Aggregation;
using LoadRunner.Demo.Guides.QuickStart;
using LoadRunner.Demo.Theoretical;

namespace LoadRunner.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // #0 Quick-Start
            // - One-file setup/execution to get acquainted with framework.
            Run("#0 QuickStart setup", QuickStartDemo.Run);

            // #1 StrategyBuilder
            // - Controlling thread count
            // - Limiting speed
            // - IScenarioFactory for spawning own IScenario instances.
            // - Providing IAggregator's to collect/aggregate results
            // - Misc (UserData, Timeout)
            // - IValidator asserting scenario on the main thread.
            Run("#1.3 Custom IScenarioFactory to create IScenario instances for workers", RawDataMeasurementsDemo.Run);

            // #2 Data Aggregation (IAggregator)

            // #2.1 Histogram
            // - Existing Dimensions & Metrics
            // - Creating own IDimension or IMetric

            // #2.2 RawData stream  
            // Take raw measurements from execution without aggregation
            // - Allows writting own simple aggregations
            // - Doing aggregations post-test.
            //   - Persisting raw measurements so new aggregations could be run later.
            Run("#2.2 Raw-Data (StreamAggregator, JsonStreamAggregator)", RawDataMeasurementsDemo.Run);

            // #5 Replay Strategy
            // - IReplayDataReader
            //

            // #6 Customization 
            // - IAggregator
            // - ISpeedStrategy
            // - IThreadingStrategy
            // - ILimitStrategy


            WriteLinePause("WARNING: READ BEFORE CONTINUING:",
                "Next tests show theoretical speeds but they can also easily eat 5+GB's of memory.",
                "Press enter if only have enough resources to continue, otherwise pagefile will start crying and system may freeze...",
                "", "For maximum throughput a Release build without debugger attached must be executed...",
                "Press ENTER to continue..."
            );

            // #7 Theoretical throughput
            // - Blank scenario
            //   - Without IAggregator
            //   - With IAggregator
            Run("#7.1 Theoretical speed (Blank scenario without aggregation)  (~10secs)", TheoreticalSpeedDemo.Run);

            Run("#7.2 Theoretical speed (Blank scenario with aggregation hooked) (~30secs)", AggregationImpactDemo.Run);

            // Additional tips: 
            // * If planning to use HttpClient, WebRequest or network related tools to make Load-Tests
            //   - Make sure to tweak App.config accordingly to lift up .NET connection limit.
            //   - Check example in projects app config (<add address="*" maxconnection="4000"/>)
            //   - Also .NET built-in HTTP clients lack blazing performance, specialized client might be required for doing 2k+/s speeds.
            //
            // * Avoid running real test with debugger attached, as it can catch exceptions halting test execution and measurements wont be accurate for that time period.
            //
            // * It is also preferred to run in 64bit.

            // Probably drop these below.


            // #2 Contains more detailed information and advanced features
            //Console.WriteLine("Demo #2");
            //DetailedDemo.Run();


            // Optional but useful advanced feature worth checking out before running real test.
            //RawDataAggregationDemo.Aggregate();

            WriteLinePause("End of examples", "Press ENTER to exit...");

        }

        private static void Run(string title, Action action)
        {
            Console.WriteLine(title);
            action.Invoke();
            Console.Write("\r\nPress enter to execute next test...");
            Console.ReadLine();
        }

        private static void WriteLinePause(params string[] text)
        {
            foreach (var line in text)
            {
                Console.WriteLine(line);
            }
            
            Console.ReadLine();
        }
    }
}
