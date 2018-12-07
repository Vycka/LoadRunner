using System;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Engine.Analytics;
using Viki.LoadRunner.Engine.Strategies.Custom.Strategies.Threading;
using Viki.LoadRunner.Playground.Replay;

namespace Viki.LoadRunner.Playground
{
    class Program
    {

        static void Main()
        {
            BlankScenario.Run();

            AssertPipeline.Run();

            TheoreticalSpeedDemo.Run();

            //BlankStressScenarioMemoryStream.Run();

            ReplayDemo.Run();

            //BatchStrategyDemo.Run();

            //new BlankFromBase().Validate();

            DemoSetup.Run();

            //LimitConcurrencyAndTpsDemo.Run();

            // Warning! Hdd/sdd intensive usage with this one
            //BlankStressScenarioJsonStream.Run();

            Console.ReadKey();
        }
    }
}
