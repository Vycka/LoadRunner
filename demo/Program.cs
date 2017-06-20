namespace LoadRunner.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // ProTips: 
            // * If planning to use HttpClient, WebRequest or similar tools to make API Load-Tests
            // Make sure to tweak App.config accordingly to lift up connection limit.
            // Check example in projects app config (<add address="*" maxconnection="100"/>)
            //
            // * Avoid running tests in PROD with debugger attached, as it can catch exceptions and halt test execution and meassurements wont be accurate for that time period.
            //
            // * It is also prefered to run in 64bit so aggregations will perform better.
            //
            // Other then that, just follow QuickIntroLoadTest for simple setup example(or template)
            QuickIntroLoadTest.Run();

            // Advanced but optional feature worth checking out before going to PROD.
            RawDataAggregationDemo.Aggregate();
        }
    }
}
