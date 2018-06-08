using LoadRunner.Demo.Detailed;
using LoadRunner.Demo.Features;
using LoadRunner.Demo.Minimum;

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
            // * It is also prefered to run in 64bit so aggregations will perform better.
            //
            // Other then that, just follow BareMinimum/DetailedDemo for setup example

            // #1
            MinimalDemo.Run();


            // #2
            // DetailedDemo.Run();


            // Optional but useful advanced feature worth checking out before running real test.
            RawDataAggregationDemo.Aggregate();
        }
    }
}
