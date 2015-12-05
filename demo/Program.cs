namespace LoadRunner.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            // ProTip: If planning to use HttpClient, WebRequest or similar tools to make API Load-Tests
            // Make sure to tweak App.config accordingly to lift up connection limit.
            // Check example in projects app config (<add address="*" maxconnection="100"/>)
            //
            // It is also prefered to run in 64bit so aggregations will perform better.
            //
            // Other then that, just follow QuickIntroLoadTest for simple setup example(or template)
            QuickIntroLoadTest.Run();
        }
    }
}
