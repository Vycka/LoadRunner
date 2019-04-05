using System;
using Viki.LoadRunner.Playground.Replay;

namespace Viki.LoadRunner.Playground
{
    class Program
    {
        static void Main()
        {
            BatchAndWaitDemo.Run();
            
            BlankScenario.Run();
            
            AssertPipeline.Run();
            
            TheoreticalSpeedDemo.Run();

            BlankStressScenarioMemoryStream.Run();

            ReplayDemo.Run();

            BatchStrategyDemo.Run();

            DemoSetup.Run();

            LimitConcurrencyAndTpsDemo.Run();

            // TODO: Validate ThreadId, ThreadIterartionId, GlobalIterationId counters
            // TODO: Validate validators
            
            // Warning! Hdd/sdd intensive usage with this one
            //BlankStressScenarioJsonStream.Run();

            Console.ReadKey();
        }
    }
}
