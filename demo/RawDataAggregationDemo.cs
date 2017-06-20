using System;
using System.IO;
using Newtonsoft.Json;
using Viki.LoadRunner.Engine.Aggregators;
using Viki.LoadRunner.Engine.Aggregators.Dimensions;
using Viki.LoadRunner.Engine.Aggregators.Metrics;
using Viki.LoadRunner.Tools.Aggregators;

namespace LoadRunner.Demo
{
    public class RawDataAggregationDemo
    {

        // Thats advanced undocummented feature-aggregator, which writes all raw meassurement data to json file. (Not fool-proof: if stopped abnormally, json array will get corrupted and requires hand-fixing)
        // It comes from other RnD nuget package:
        // * https://www.nuget.org/packages/Viki.LoadRunner.Tools
        // * Install-Package Viki.LoadRunner.Tools -Pre
        //
        // The main idea behind this is:
        // You save raw meassurements to the file, and then do aggregations from it after the tests.
        // Main big advantage behind this is, that you can slice all data again in any slice you will think after the test.
        //
        // To enable it: Append this aggregation to QuickIntroLoadTest.Run() [Line 35]
        // E.g.:  LoadRunnerEngine loadRunner = LoadRunnerEngine.Create<DemoTestScenario>(parameters, histogramAggregator, AggregationSetup.BuildJsonStreamAggregator());
        //
        // And this method will get called after pressing any key after the test finishes.
        public static void Aggregate()
        {
            // Feature disabled?
            if (!File.Exists("masterdata.json"))
                return;

            // Any random aggregator
            HistogramAggregator aggregator = new HistogramAggregator()
                .Add(new FuncDimension("Created Threads", i => i.CreatedThreads.ToString()))
                .Add(new CountMetric());

            // [TUserData] - Type of UserData object in your tests (If not using it, leave object)
            // Correct type is required to prevent deserialization problems.
            JsonStreamAggregator.Replay<object>("masterdata.json", aggregator);

            Console.WriteLine("## JsonStreamAggregator demo ##");
            Console.WriteLine(JsonConvert.SerializeObject(aggregator.BuildResultsObjects(), Formatting.Indented));
            Console.ReadKey();
        }
    }
}